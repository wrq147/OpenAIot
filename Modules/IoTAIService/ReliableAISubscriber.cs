using ChannelUtility;
using ChannelUtility.Message;
using IoTAIService.AIProject;
using MessagePack;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService
{
    public class ReliableAISubscriber : IDisposable
    {
        // 连接池：存储所有订阅连接
        private readonly List<SubscriberConnection> _connectionPool = new List<SubscriberConnection>();
        private readonly string[] _connectAddressArr;
        private CancellationTokenSource _cts;
        private ITAServiceProvider _provider;

        public ReliableAISubscriber(ITAServiceProvider serviceProvider, IOptions<IoTAIOption> option)
        {
            _provider = serviceProvider;
            _connectAddressArr = option.Value.FramePushConns ?? Array.Empty<string>();
            _cts = new CancellationTokenSource();

            // 初始化连接池
            InitializeConnectionPool();
        }

        /// <summary>
        /// 初始化连接池，为每个地址创建连接对象
        /// </summary>
        private void InitializeConnectionPool()
        {
            foreach (var address in _connectAddressArr)
            {
                if (string.IsNullOrWhiteSpace(address)) continue;

                var connection = new SubscriberConnection
                {
                    Address = address,
                    IsConnected = false,
                    Socket = null,
                    ReceiveThread = null
                };
                _connectionPool.Add(connection);
            }
        }

        /// <summary>
        /// 启动所有订阅连接（每个连接一个独立线程）
        /// </summary>
        public void StartReceiving()
        {
            foreach (var connection in _connectionPool)
            {
                // 防止重复启动
                if (connection.ReceiveThread != null && connection.ReceiveThread.IsAlive)
                    continue;

                // 为每个连接创建独立的接收线程
                connection.ReceiveThread = new Thread(() => ReceiveLoop(connection));
                connection.ReceiveThread.IsBackground = true;
                connection.ReceiveThread.Start();
            }
        }

        /// <summary>
        /// 单个连接的消息接收循环
        /// </summary>
        /// <param name="connection">当前连接</param>
        private void ReceiveLoop(SubscriberConnection connection)
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    // 检查连接状态，断开则重连
                    if (!connection.IsConnected)
                    {
                        // 释放旧Socket
                        connection.Socket?.Dispose();

                        // 创建新Socket并连接
                        connection.Socket = new SubscriberSocket();
                        connection.Socket.Connect(connection.Address);
                        connection.Socket.SubscribeToAnyTopic();

                        connection.IsConnected = true;
                        Console.WriteLine($"订阅连接重连成功: {connection.Address}");
                    }

                    // 非阻塞接收消息（超时2秒）
                    if (connection.IsConnected &&
                        connection.Socket.TryReceiveFrameBytes(TimeSpan.FromSeconds(2), out byte[] frameData) &&
                        frameData != null)
                    {
                        // 反序列化消息
                        var options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
                        var aiFrame = MessagePackSerializer.Deserialize<AIDetectRequestMeesage>(frameData, options);

                        List<AIConfigData> configList = null;
                        if (!string.IsNullOrEmpty(aiFrame.Configs))
                        {
                            configList = System.Text.Json.JsonSerializer.Deserialize<List<AIConfigData>>(aiFrame.Configs, JsonMessageSerializerConfig.DefaultOptions);
                        }

                        // 处理消息（同步调用异步方法）
                        _provider.GetService<AIProjectManager>()
                                 .MessageHandler(aiFrame, configList)
                                 .GetAwaiter()
                                 .GetResult();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"订阅连接异常 [{connection.Address}]: {ex.ToString()}");
                    connection.IsConnected = false;

                    // 休眠1秒避免高频重连（响应取消令牌）
                    if (!_cts.Token.WaitHandle.WaitOne(1000))
                        break;
                }
            }

            Console.WriteLine($"订阅连接接收循环退出: {connection.Address}");
        }

        /// <summary>
        /// 停止所有订阅连接（优雅退出）
        /// </summary>
        public void Stop()
        {
            // 取消所有线程的令牌
            _cts.Cancel();

            // 等待每个连接的线程退出（最多等待5秒）
            foreach (var connection in _connectionPool)
            {
                connection.IsConnected = false;
                if (connection.ReceiveThread != null && connection.ReceiveThread.IsAlive)
                {
                    connection.ReceiveThread.Join(TimeSpan.FromSeconds(5));
                }
                Console.WriteLine($"停止订阅连接: {connection.Address}");
            }
            Console.WriteLine("所有订阅连接已停止");
        }

        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void Dispose()
        {
            Stop();

            // 释放连接池中的所有连接资源
            foreach (var connection in _connectionPool)
            {
                connection.Dispose();
            }
            _connectionPool.Clear();

            _cts?.Dispose();
        }

    }

    /// <summary>
    /// 连接池项，封装单个订阅连接的信息
    /// </summary>
    internal class SubscriberConnection : IDisposable
    {
        /// <summary>
        /// 订阅Socket
        /// </summary>
        public SubscriberSocket Socket { get; set; }
        /// <summary>
        /// 连接地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        /// 接收线程
        /// </summary>
        public Thread ReceiveThread { get; set; }

        public void Dispose()
        {
            Socket?.Dispose();
            ReceiveThread = null;
        }
    }
}