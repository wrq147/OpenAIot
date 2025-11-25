using StackExchange.Redis;
using System;
using System.Net;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ChannelUtility.Redis
{
    /// <summary>
    /// Redis 连接管理器，提供线程安全的 Redis 连接管理
    /// </summary>
    public sealed class RedisConnectionManager : IDisposable
    {
        private static readonly ConcurrentDictionary<string, Lazy<RedisConnectionManager>> _instances
            = new ConcurrentDictionary<string, Lazy<RedisConnectionManager>>();

        private readonly string _connectionString;
        private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _reconnectWindow = TimeSpan.FromMinutes(1);
        private readonly ReaderWriterLockSlim _connectionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private ConnectionMultiplexer _connection;
        private DateTime _lastReconnectTime = DateTime.MinValue;
        private DateTime _firstErrorTime = DateTime.MinValue;
        private long _reconnectCount = 0;
        private bool _disposed = false;
        private volatile bool _isConnectionHealthy = false;

        /// <summary>
        /// 获取指定连接字符串的 Redis 连接管理器实例
        /// </summary>
        public static RedisConnectionManager GetInstance(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return _instances.GetOrAdd(connectionString,
                new Lazy<RedisConnectionManager>(() => new RedisConnectionManager(connectionString))).Value;
        }

        /// <summary>
        /// 私有构造函数，确保单例模式
        /// </summary>
        private RedisConnectionManager(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            Connect();
        }

        /// <summary>
        /// 初始化 Redis 连接
        /// </summary>
        private void Connect()
        {
            _connectionLock.EnterWriteLock();
            try
            {
                // 关闭可能存在的旧连接
                DisposeConnection();

                // 创建新连接
                _connection = ConnectInternal();
                RegisterConnectionEvents();

                _firstErrorTime = DateTime.MinValue;
                _lastReconnectTime = DateTime.UtcNow;
                _reconnectCount = 0;
                _isConnectionHealthy = true;

                Console.WriteLine($"Redis 连接初始化成功: {_connectionString}");
            }
            catch (Exception ex)
            {
                _isConnectionHealthy = false;
                Console.WriteLine($"Redis 初始化连接失败: {ex.Message}");
                LogConnectionError(ex, "初始化连接失败");
                throw;
            }
            finally
            {
                _connectionLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取基础 Redis 连接实例
        /// </summary>
        private ConnectionMultiplexer GetConnection()
        {
            bool needRecc = false;
            _connectionLock.EnterReadLock();
            try
            {
                if (_connection == null)
                {
                    throw new RedisConnectionException(ConnectionFailureType.SocketFailure, "Redis 连接未初始化");
                }
                if (!_isConnectionHealthy || !_connection.IsConnected)
                {
                    needRecc = true;
                }
            }
            finally
            {
                _connectionLock.ExitReadLock();
            }

            if (needRecc)
            {
                ForceReconnect();
            }
            return _connection;
        }

        private async Task<ConnectionMultiplexer> GetConnectionAsync()
        {
            bool needRecc = false;
            _connectionLock.EnterReadLock();
            try
            {
                if (_connection == null)
                {
                    throw new RedisConnectionException(ConnectionFailureType.SocketFailure, "Redis 连接未初始化");
                }
                if (!_isConnectionHealthy || !_connection.IsConnected)
                {
                    needRecc = true;
                }
            }
            finally
            {
                _connectionLock.ExitReadLock();
            }

            if (needRecc)
            {
                await ForceReconnectAsync();
            }
            return _connection;
        }

        /// <summary>
        /// 获取 Redis 数据库实例
        /// </summary>
        public IDatabase GetDatabase(int db = -1)
        {
            try
            {
                return GetConnection().GetDatabase(db);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 数据库失败: {ex.Message}");
                LogConnectionError(ex, $"获取数据库失败，db={db}");

                throw;
            }
        }
        public async Task<IDatabase> GetDatabaseAsync(int db = -1)
        {
            try
            {
                return (await GetConnectionAsync()).GetDatabase(db);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 数据库失败: {ex.Message}");
                LogConnectionError(ex, $"获取数据库失败，db={db}");

                throw;
            }
        }
        /// <summary>
        /// 获取 Redis 服务器实例
        /// </summary>
        public IServer GetServer(string host, int port)
        {
            try
            {
                return GetConnection().GetServer(host, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 服务器失败: {ex.Message}");
                LogConnectionError(ex, $"获取服务器失败，host={host}, port={port}");

                ForceReconnect();
                throw;
            }
        }

        public IServer GetServer(string hostport)
        {
            try
            {
                return GetConnection().GetServer(hostport);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 服务器失败: {ex.Message}");
                LogConnectionError(ex, $"获取服务器失败，hostport={hostport}");
                throw;
            }
        }
        public async Task<IServer> GetServerAsync(string hostport)
        {
            try
            {
                return (await GetConnectionAsync()).GetServer(hostport);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 服务器失败: {ex.Message}");
                LogConnectionError(ex, $"获取服务器失败，hostport={hostport}");
                throw;
            }
        }
        public IServer GetServer(EndPoint ep)
        {
            try
            {
                return GetConnection().GetServer(ep);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 服务器失败: {ex.Message}");
                LogConnectionError(ex, $"获取服务器失败，endpoint={ep}");

                ForceReconnect();
                throw;
            }
        }
        public async Task<IServer> GetServerAsync(EndPoint ep)
        {
            try
            {
                return (await GetConnectionAsync()).GetServer(ep);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 服务器失败: {ex.Message}");
                LogConnectionError(ex, $"获取服务器失败，endpoint={ep}");

                ForceReconnect();
                throw;
            }
        }

        public ISubscriber GetSubscriber()
        {
            try
            {
                return GetConnection().GetSubscriber();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 订阅器失败: {ex.Message}");
                LogConnectionError(ex, "获取订阅器失败");

                throw;
            }
        }
        public async Task<ISubscriber> GetSubscriberAsync()
        {
            try
            {
                return (await GetConnectionAsync()).GetSubscriber();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 订阅器失败: {ex.Message}");
                LogConnectionError(ex, "获取订阅器失败");

                throw;
            }
        }
        /// <summary>
        /// 获取所有连接端点
        /// </summary>
        public EndPoint[] GetEndPoints()
        {
            try
            {
                return GetConnection().GetEndPoints();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取 Redis 端点失败: {ex.Message}");
                LogConnectionError(ex, "获取端点失败");

                throw;
            }
        }

        /// <summary>
        /// 强制重新连接 Redis（线程安全）
        /// </summary>
        public void ForceReconnect()
        {
            if (_firstErrorTime == DateTime.MinValue)
            {
                _firstErrorTime = DateTime.UtcNow;
            }

            var now = DateTime.UtcNow;

            // 检查是否在重试窗口内且重试次数过多
            if (now < _lastReconnectTime + _retryDelay)
            {
                Console.WriteLine($"Redis 重连请求被拒绝，上次重连时间: {_lastReconnectTime:yyyy-MM-dd HH:mm:ss}");
                return;
            }


            _connectionLock.EnterWriteLock();
            try
            {
                now = DateTime.UtcNow;

                // 再次检查，避免竞态条件
                if (now < _lastReconnectTime + _retryDelay)
                {
                    Console.WriteLine($"Redis 重连请求被拒绝（双重检查），上次重连时间: {_lastReconnectTime:yyyy-MM-dd HH:mm:ss}");
                    return;
                }

                Interlocked.Increment(ref _reconnectCount);
                _lastReconnectTime = now;
                _isConnectionHealthy = false;

                Console.WriteLine($"开始 Redis 重连 - 重连次数: {_reconnectCount}，自上次重连: {now - _lastReconnectTime:g}");

                // 关闭旧连接
                DisposeConnection();

                // 创建新连接
                try
                {
                    _connection = ConnectInternal();
                    RegisterConnectionEvents();
                    _isConnectionHealthy = true;

                    Console.WriteLine($"Redis 重连成功 - 重连次数: {_reconnectCount}");

                    // 如果超过重连窗口，重置计数器
                    if (now > _firstErrorTime + _reconnectWindow)
                    {
                        _firstErrorTime = DateTime.MinValue;
                        Interlocked.Exchange(ref _reconnectCount, 0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Redis 重连失败: {ex.Message}");
                    LogConnectionError(ex, $"重连失败，尝试次数: {_reconnectCount}");
                    throw;
                }
            }
            finally
            {
                _connectionLock.ExitWriteLock();
            }
        }
        public async Task ForceReconnectAsync()
        {
            if (_firstErrorTime == DateTime.MinValue)
            {
                _firstErrorTime = DateTime.UtcNow;
            }

            var now = DateTime.UtcNow;

            // 检查是否在重试窗口内且重试次数过多
            if (now < _lastReconnectTime + _retryDelay)
            {
                Console.WriteLine($"Redis 重连请求被拒绝，上次重连时间: {_lastReconnectTime:yyyy-MM-dd HH:mm:ss}");
                return;
            }


            bool lockAcquired = false;
            try
            {
                _connectionLock.EnterWriteLock();
                lockAcquired = true;
            }
            catch (LockRecursionException)
            {
                Console.WriteLine("Redis 异步重连时获取写锁失败，可能存在递归调用");
                return;
            }
            try
            {
                now = DateTime.UtcNow;

                // 再次检查，避免竞态条件
                if (now < _lastReconnectTime + _retryDelay)
                {
                    Console.WriteLine($"Redis 重连请求被拒绝（双重检查），上次重连时间: {_lastReconnectTime:yyyy-MM-dd HH:mm:ss}");
                    return;
                }

                Interlocked.Increment(ref _reconnectCount);
                _lastReconnectTime = now;
                _isConnectionHealthy = false;

                Console.WriteLine($"开始 Redis 重连 - 重连次数: {_reconnectCount}，自上次重连: {now - _lastReconnectTime:g}");

                // 关闭旧连接
                await DisposeConnectionAsync();

                // 创建新连接
                try
                {
                    _connection = await ConnectInternalAsync();
                    RegisterConnectionEvents();
                    _isConnectionHealthy = true;

                    Console.WriteLine($"Redis 重连成功 - 重连次数: {_reconnectCount}");

                    // 如果超过重连窗口，重置计数器
                    if (now > _firstErrorTime + _reconnectWindow)
                    {
                        _firstErrorTime = DateTime.MinValue;
                        Interlocked.Exchange(ref _reconnectCount, 0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Redis 重连失败: {ex.Message}");
                    LogConnectionError(ex, $"重连失败，尝试次数: {_reconnectCount}");
                    throw;
                }
            }
            finally
            {
                if (lockAcquired && _connectionLock.IsWriteLockHeld)
                {
                    _connectionLock.ExitWriteLock();
                }
            }
        }
        /// <summary>
        /// 初始化 Redis 连接（内部实现）
        /// </summary>
        private ConnectionMultiplexer ConnectInternal()
        {
            Exception lastException = null;

            Console.WriteLine($"开始尝试连接 Redis: {_connectionString}");

            try
            {
                var config = ConfigurationOptions.Parse(_connectionString);
                config.AbortOnConnectFail = false;
                config.ConnectTimeout = 2000;
                config.SyncTimeout = 2000;
                config.AsyncTimeout = 2000;
                config.KeepAlive = 60;
                config.DefaultDatabase = 0;
                config.ConnectRetry = 10;
                config.ReconnectRetryPolicy = new ExponentialRetry(5000);

                Console.WriteLine($"尝试连接 Redis : {_connectionString}");

                var connection = ConnectionMultiplexer.Connect(config);

                Console.WriteLine($"Redis 连接成功: {connection.Configuration}");
                Console.WriteLine($"Redis 服务器版本: {connection.GetServer(connection.GetEndPoints().FirstOrDefault())?.Version}");

                return connection;
            }
            catch (Exception ex)
            {
                lastException = ex;

                Console.WriteLine($"Redis 连接尝试失败: {ex.Message}");
                LogConnectionError(ex, $"连接尝试失败");
            }

            throw new InvalidOperationException("无法连接到 Redis 服务器，已达到最大重试次数", lastException);
        }
        private async Task<ConnectionMultiplexer> ConnectInternalAsync()
        {
            Exception lastException = null;

            Console.WriteLine($"开始尝试连接 Redis: {_connectionString}");

            try
            {
                var config = ConfigurationOptions.Parse(_connectionString);
                config.AbortOnConnectFail = false;
                config.ConnectTimeout = 2000;
                config.SyncTimeout = 2000;
                config.AsyncTimeout = 2000;
                config.KeepAlive = 60;
                config.DefaultDatabase = 0;
                config.ConnectRetry = 10;
                config.ReconnectRetryPolicy = new ExponentialRetry(5000);

                Console.WriteLine($"尝试连接 Redis : {_connectionString}");

                var connection = await ConnectionMultiplexer.ConnectAsync(config);

                Console.WriteLine($"Redis 连接成功: {connection.Configuration}");
                Console.WriteLine($"Redis 服务器版本: {connection.GetServer(connection.GetEndPoints().FirstOrDefault())?.Version}");

                return connection;
            }
            catch (Exception ex)
            {
                lastException = ex;

                Console.WriteLine($"Redis 连接尝试失败: {ex.Message}");
                LogConnectionError(ex, $"连接尝试失败");
            }

            throw new InvalidOperationException("无法连接到 Redis 服务器，已达到最大重试次数", lastException);
        }
        /// <summary>
        /// 注册 Redis 连接事件处理程序
        /// </summary>
        private void RegisterConnectionEvents()
        {
            if (_connection == null)
            {
                return;
            }

            _connection.ConnectionFailed += (sender, args) =>
            {
                Console.WriteLine($"Redis 连接失败: {args.Exception.Message}，类型: {args.FailureType}");
                _isConnectionHealthy = false;
                LogConnectionError(args.Exception, $"连接失败，类型: {args.FailureType}");
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    await ForceReconnectAsync();
                });
            };

            _connection.ConnectionRestored += (sender, args) =>
            {
                Console.WriteLine($"Redis 连接已恢复，类型: {args.FailureType}");
                _isConnectionHealthy = true;
                LogConnectionError(null, $"连接恢复，类型: {args.FailureType}");
            };

            _connection.ErrorMessage += (sender, args) =>
            {
                Console.WriteLine($"Redis 错误消息: {args.Message}");
                _isConnectionHealthy = false;
                LogConnectionError(new Exception(args.Message), "错误消息");
            };

            _connection.ConfigurationChanged += (sender, args) =>
            {
                Console.WriteLine($"Redis 配置已更改: {args.EndPoint}");
                LogConnectionError(null, $"配置更改: {args.EndPoint}");
            };
        }

        /// <summary>
        /// 安全释放 Redis 连接
        /// </summary>
        private void DisposeConnection()
        {
            try
            {
                Console.WriteLine("正在释放 Redis 连接...");
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"关闭 Redis 连接时出错: {ex.Message}");
                LogConnectionError(ex, "释放连接失败");
            }
            finally
            {
                _connection = null;
                _isConnectionHealthy = false;
                Console.WriteLine("Redis 连接已释放");
            }
        }
        private async Task DisposeConnectionAsync()
        {
            try
            {
                Console.WriteLine("正在释放 Redis 连接...");

                _connection?.CloseAsync();
                _connection?.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"关闭 Redis 连接时出错: {ex.Message}");
                LogConnectionError(ex, "释放连接失败");
            }
            finally
            {
                _connection = null;
                _isConnectionHealthy = false;
                Console.WriteLine("Redis 连接已释放");
            }
        }
        /// <summary>
        /// 记录连接错误日志
        /// </summary>
        private void LogConnectionError(Exception ex, string context)
        {
            try
            {
                // 这里可以替换为实际的日志记录实现
                Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [Redis] [{context}] {ex?.Message ?? "无异常"}");

                if (ex != null && ex.InnerException != null)
                {
                    Console.WriteLine($"  内部异常: {ex.InnerException.Message}");
                }

                if (ex != null)
                {
                    Console.WriteLine($"  堆栈跟踪: {ex.StackTrace}");
                }
            }
            catch
            {
                // 日志记录失败时的保护措施
            }
        }

        /// <summary>
        /// 释放 Redis 连接管理器占用的资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放 Redis 连接管理器占用的资源
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine("正在释放 RedisConnectionManager 资源...");

                    _connectionLock.EnterWriteLock();
                    try
                    {
                        DisposeConnection();
                    }
                    finally
                    {
                        _connectionLock.ExitWriteLock();
                        _connectionLock.Dispose();
                    }

                    // 从实例字典中移除
                    _instances.TryRemove(_connectionString, out _);

                    Console.WriteLine("RedisConnectionManager 资源已释放");
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Redis 连接管理器析构函数
        /// </summary>
        ~RedisConnectionManager()
        {
            Dispose(false);
        }
    }
}
