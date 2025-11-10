using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;


namespace ChannelUtility.Redis
{
    /// <summary>
    /// Redis操作
    /// </summary>
    public class RedisHelper
    {
        private int DbNum { get; }
        private string _prefixKey;
        private string _connStr;

        public RedisHelper(string connStr, int dbNum = 0)
        {
            DbNum = dbNum;
            _connStr = connStr;
        }

        /// <summary>
        /// 连接 Redis 数据库
        /// </summary>
        /// <returns>Redis 数据库连接实例</returns>
        private RedisConnectionManager GetConnectRedis()
        {
            return RedisConnectionManager.GetInstance(_connStr);
        }
        public async Task ReConnectAsync()
        {
            await GetConnectRedis().ForceReconnectAsync();
        }
        static RedisValue Token = Environment.MachineName;

        public RedisResult Execute(string command, params object[] args)
        {
            return Do(db => db.Execute(command, args));
        }
        public async Task<RedisResult> ExecuteAsync(string command, params object[] args)
        {
            return await DoAsync(async db => await db.ExecuteAsync(command, args));
        }

        #region 加锁
        /// <summary>
        /// 加锁处理
        /// </summary>
        /// <param name="lockkey"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public bool LockToDo(string lockkey, Action<IDatabase> ac)
        {
            return Do<bool>((IDatabase db) =>
            {
                bool flag = false;
                //设置timespan避免死锁
                if (db.LockTake(lockkey, Token, TimeSpan.FromSeconds(5)))
                {
                    try
                    {
                        ac(db);
                        flag = true;
                    }
                    finally
                    {
                        db.LockRelease(lockkey, Token);
                    }
                }
                return flag;
            });

        }

        /// <summary>
        /// 加锁
        /// </summary>
        /// <param name="lockkey"></param>
        /// <returns></returns>
        public bool LockTake(string lockkey)
        {
            return Do<bool>((IDatabase db) =>
            {
                return db.LockTake(lockkey, Token, TimeSpan.FromSeconds(5));
            });
        }
        /// <summary>
        /// 等待加锁
        /// </summary>
        /// <param name="lockkey"></param>
        /// <returns></returns>
        public bool WaitLockTake(string lockkey)
        {
            bool hasLock = false;
            int lockCounter = 0;
            while (lockCounter < 10)
            {
                if (!LockTake(lockkey))
                {
                    lockCounter++;
                    Thread.Sleep(100);
                    continue;
                }
                hasLock = true;
                break;
            }
            return hasLock;
        }
        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="lockkey"></param>
        /// <returns></returns>
        public bool LockRelease(string lockkey)
        {
            return Do<bool>((IDatabase db) =>
            {
                return db.LockRelease(lockkey, Token);
            });
        }

        /// <summary>
        /// 加锁处理
        /// </summary>
        /// <param name="lockkey"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public async Task<bool> LockToDoAsync(string lockkey, Action<IDatabase> ac)
        {
            return await DoAsync(async (IDatabase db) =>
            {
                bool flag = false;
                //设置timespan避免死锁
                if (await db.LockTakeAsync(lockkey, Token, TimeSpan.FromSeconds(5)))
                {
                    try
                    {
                        ac(db);
                        flag = true;
                    }
                    finally
                    {
                        await db.LockReleaseAsync(lockkey, Token);
                    }
                }
                return flag;
            });

        }
        /// <summary>
        /// 加锁
        /// </summary>
        /// <param name="lockkey"></param>
        /// <returns></returns>
        public async Task<bool> LockTakeAsync(string lockkey)
        {
            return await DoAsync(async db => await db.LockTakeAsync(lockkey, Token, TimeSpan.FromSeconds(5)));
        }
        /// <summary>
        /// 异步等待加锁
        /// </summary>
        /// <param name="lockkey"></param>
        /// <returns></returns>
        public async Task<bool> WaitLockTakeAsync(string lockkey)
        {
            bool hasLock = false;
            int lockCounter = 0;
            while (lockCounter < 10)
            {
                if (!await LockTakeAsync(lockkey))
                {
                    lockCounter++;
                    await Task.Delay(100);
                    continue;
                }
                hasLock = true;
                break;
            }
            return hasLock;
        }
        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="lockkey"></param>
        /// <returns></returns>
        public async Task<bool> LockReleaseAsync(string lockkey)
        {
            return await DoAsync(async db => await db.LockReleaseAsync(lockkey, Token));
        }

        #endregion

        #region String

        #region 同步方法


        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefixKey(key);
            return Do(db => db.StringSet(key, value, expiry));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddPrefixKey(p.Key), p.Value)).ToList();
            return Do(db => db.StringSet(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefixKey(key);
            string json = ConvertJson(obj);
            return Do(db => db.StringSet(key, json, expiry));
        }
        public bool StringSet<T>(IDatabase db, string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefixKey(key);
            string json = ConvertJson(obj);
            return db.StringSet(key, json, expiry);
        }
        public bool KeyDelete(IDatabase db, string key)
        {
            key = AddPrefixKey(key);
            return db.KeyDelete(key);
        }
        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            key = AddPrefixKey(key);
            return Do(db => db.StringGet(key));
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public RedisValue[] StringGet(List<string> listKey)
        {
            List<string> newKeys = listKey.Select(AddPrefixKey).ToList();
            return Do(db => db.StringGet(ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db => ConvertObj<T>(db.StringGet(key)));
        }
        public T StringGet<T>(IDatabase db, string key)
        {
            key = AddPrefixKey(key);
            return ConvertObj<T>(db.StringGet(key));
        }
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return Do(db => db.StringIncrement(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return Do(db => db.StringDecrement(key, val));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.StringSetAsync(key, value, expiry));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddPrefixKey(p.Key), p.Value)).ToList();
            return await DoAsync(async db => await db.StringSetAsync(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefixKey(key);
            string json = ConvertJson(obj);
            return await DoAsync(async db => await db.StringSetAsync(key, json, expiry));
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public async Task<string> StringGetAsync(string key)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.StringGetAsync(key));
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public async Task<RedisValue[]> StringGetAsync(List<string> listKey)
        {
            List<string> newKeys = listKey.Select(AddPrefixKey).ToList();
            return await DoAsync(async db => await db.StringGetAsync(ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> StringGetAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            string result = await DoAsync(async db => await db.StringGetAsync(key));
            return ConvertObj<T>(result);
        }
        public async Task<long> StringIncrementLongAsync(string key, long val = 1)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.StringIncrementAsync(key, val));
        }
        public async Task<long> StringDecrementLongAsync(string key, long val = 1)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.StringDecrementAsync(key, val));
        }
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> StringIncrementAsync(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.StringIncrementAsync(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> StringDecrementAsync(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.StringDecrementAsync(key, val));
        }

        #endregion 异步方法

        #endregion String

        #region Hash

        #region 同步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return Do(db => db.HashExists(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                string json = ConvertJson(t);
                return db.HashSet(key, dataKey, json);
            });
        }

        public bool HashSet(string key, string dataKey, string val)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                return db.HashSet(key, dataKey, val);
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return Do(db => db.HashDelete(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete(string key, string[] dataKeys)
        {
            key = AddPrefixKey(key);
            RedisValue[] tlist = new RedisValue[dataKeys.Length];
            for (int i = 0; i < dataKeys.Length; i++)
            {
                tlist[i] = dataKeys[i];
            }
            return Do(db => db.HashDelete(key, tlist));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                string value = db.HashGet(key, dataKey);
                return ConvertObj<T>(value);
            });
        }
        public string HashGet(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                return db.HashGet(key, dataKey);
            });
        }
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return Do(db => db.HashIncrement(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return Do(db => db.HashDecrement(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                RedisValue[] values = db.HashKeys(key);
                return ConvetList<T>(values);
            });
        }
        /// <summary>
        /// 获取hashkey所有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> HashValues(string key)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                RedisValue[] values = db.HashValues(key, CommandFlags.None);
                return ConvetStrList(values);
            });
        }
        public Dictionary<string, T> HashGetAll<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                HashEntry[] values = db.HashGetAll(key, CommandFlags.None);
                Dictionary<string, T> dict = new Dictionary<string, T>();
                foreach (HashEntry he in values)
                {
                    dict.Add(he.Name, ConvertObj<T>(he.Value));
                }
                return dict;
            });
        }
        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T t)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db =>
            {
                string json = ConvertJson(t);
                return await db.HashSetAsync(key, dataKey, json);
            });
        }
        public async Task HashSetAsync<T>(string key, IDictionary<string, T> vals)
        {
            key = AddPrefixKey(key);
            await DoAsync(async db =>
            {
                var hs = new HashEntry[vals.Count];
                int i = 0;
                foreach (var kvp in vals)
                {
                    hs[i] = new HashEntry(kvp.Key, ConvertJson(kvp.Value));
                    i++;
                }

                await db.HashSetAsync(key, hs);
                return 0;
            });
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public async Task<long> HashDeleteAsync(string key, string[] dataKeys)
        {
            key = AddPrefixKey(key);
            RedisValue[] tlist = new RedisValue[dataKeys.Length];
            for (int i = 0; i < dataKeys.Length; i++)
            {
                tlist[i] = dataKeys[i];
            }
            return await DoAsync(async db => await db.HashDeleteAsync(key, tlist));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T> HashGetAsync<T>(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            string value = await DoAsync(async db => await db.HashGetAsync(key, dataKey));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 从hash批量获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HashGetListAsync<T>(string key, List<string> list)
        {
            key = AddPrefixKey(key);
            RedisValue[] inputs = new RedisValue[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                inputs[i] = list[i];
            }

            return await DoAsync(async db =>
            {
                RedisValue[] values = await db.HashGetAsync(key, inputs);
                Dictionary<string, T> dict = new Dictionary<string, T>();
                for (int j = 0; j < values.Length; j++)
                {
                    if (values[j].IsNull)
                    {
                        continue;
                    }
                    dict.Add(list[j], ConvertObj<T>(values[j]));
                }
                return dict;
            });
        }

        public async Task<HashEntry[]> HashGetAllHashEntryAsync(string key)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db =>
            {
                HashEntry[] values = await db.HashGetAllAsync(key, CommandFlags.None);
                return values;
            });
        }
        /// <summary>
        /// 从hash表中获取所有数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db =>
            {
                HashEntry[] values = await db.HashGetAllAsync(key, CommandFlags.None);
                Dictionary<string, T> dict = new Dictionary<string, T>();
                foreach (HashEntry he in values)
                {
                    dict.Add(he.Name, ConvertObj<T>(he.Value));
                }
                return dict;
            });
        }
        public async Task<long> HashIncrementAsync(string key, string dataKey, long val = 1)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.HashIncrementAsync(key, dataKey, val));
        }
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.HashDecrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            RedisValue[] values = await DoAsync(async db => await db.HashKeysAsync(key));
            return ConvetList<T>(values);
        }

        #endregion 异步方法

        #endregion Hash

        #region List

        #region 同步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRemove<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            Do(db => db.ListRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(redis =>
            {
                var values = redis.ListRange(key);
                return ConvetList<T>(values);
            });
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRightPush<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            Do(db => db.ListRightPush(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                var value = db.ListRightPop(key);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListLeftPush<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            Do(db => db.ListLeftPush(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                var value = db.ListLeftPop(key);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.ListLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRemoveAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.ListRemoveAsync(key, ConvertJson(value)));
        }
        /// <summary>
        /// 移除指定范围数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task ListTrimAsync(string key, long start, long end)
        {
            key = AddPrefixKey(key);
            await DoAsync(async db =>
            {
                await db.ListTrimAsync(key, start, end);
                return 0;
            });
        }
        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var values = await DoAsync(async redis => await redis.ListRangeAsync(key));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRightPushAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.ListRightPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var value = await DoAsync(async db => await db.ListRightPopAsync(key));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.ListLeftPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var value = await DoAsync(async db => await db.ListLeftPopAsync(key));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string key)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async redis => await redis.ListLengthAsync(key));
        }

        #endregion 异步方法

        #endregion List

        #region SortedSet 有序集合

        #region 同步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetAdd(key, ConvertJson<T>(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(redis =>
            {
                var values = redis.SortedSetRangeByRank(key);
                return ConvetList<T>(values);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async redis => await redis.SortedSetAddAsync(key, ConvertJson<T>(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async redis => await redis.SortedSetRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var values = await DoAsync(async redis => await redis.SortedSetRangeByRankAsync(key));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string key)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async redis => await redis.SortedSetLengthAsync(key));
        }

        #endregion 异步方法

        #endregion SortedSet 有序集合

        #region key

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            key = AddPrefixKey(key);
            return Do(db => db.KeyDelete(key));
        }
        public async Task<bool> KeyDeleteAsync(string key)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.KeyDeleteAsync(key));
        }
        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(List<string> keys)
        {
            List<string> newKeys = keys.Select(AddPrefixKey).ToList();
            return Do(db => db.KeyDelete(ConvertRedisKeys(newKeys)));
        }
        public async Task<long> KeyDeleteAsync(List<string> keys)
        {
            List<string> newKeys = keys.Select(AddPrefixKey).ToList();
            return await DoAsync(async db => await db.KeyDeleteAsync(ConvertRedisKeys(newKeys)));
        }
        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            key = AddPrefixKey(key);
            return Do(db => db.KeyExists(key));
        }
        public async Task<bool> KeyExistsAsync(string key)
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.KeyExistsAsync(key));
        }
        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            key = AddPrefixKey(key);
            return Do(db => db.KeyRename(key, newKey));
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefixKey(key);
            return Do(db => db.KeyExpire(key, expiry));
        }
        /// <summary>
        /// 设置Key的时间（异步）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefixKey(key);
            return await DoAsync(async db => await db.KeyExpireAsync(key, expiry));
        }
        #endregion key

        #region 发布订阅
        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = GetConnectRedis().GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }
        public async Task SubscribeAsync(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = await GetConnectRedis().GetSubscriberAsync();
            await sub.SubscribeAsync(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }
        public async Task SubscribeAsync<T>(string subChannel, Action<RedisChannel, T> handler = null)
        {
            ISubscriber sub = await GetConnectRedis().GetSubscriberAsync();
            await sub.SubscribeAsync(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, ConvertObj<T>(message));
                }
            });
        }
        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T msg)
        {
            ISubscriber sub = GetConnectRedis().GetSubscriber();
            return sub.Publish(channel, ConvertJson(msg));
        }
        public async Task<long> PublishAsync<T>(string channel, T msg)
        {
            ISubscriber sub = await GetConnectRedis().GetSubscriberAsync();
            return await sub.PublishAsync(channel, ConvertJson(msg));
        }
        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = GetConnectRedis().GetSubscriber();
            sub.Unsubscribe(channel);
        }
        public async Task UnsubscribeAsync(string channel)
        {
            ISubscriber sub = await GetConnectRedis().GetSubscriberAsync();
            await sub.UnsubscribeAsync(channel);
        }
        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = GetConnectRedis().GetSubscriber();
            sub.UnsubscribeAll();
        }
        public async Task UnsubscribeAllAsync()
        {
            ISubscriber sub = await GetConnectRedis().GetSubscriberAsync();
            await sub.UnsubscribeAllAsync();
        }

        #endregion 发布订阅

        #region 其他

        public ITransaction CreateTransaction()
        {
            return GetDatabase().CreateTransaction();
        }

        public IDatabase GetDatabase()
        {
            return GetConnectRedis().GetDatabase(DbNum);
        }
        public EndPoint[] GetEndPoints()
        {
            return GetConnectRedis().GetEndPoints();
        }
        public IServer GetServer(string hostAndPort)
        {
            return GetConnectRedis().GetServer(hostAndPort);
        }

        public List<string> Keys(string pattern)
        {
            var conn = GetConnectRedis();
            IServer server = conn.GetServer(conn.GetEndPoints()[0]);
            var keys = server.Keys(DbNum, pattern);
            List<string> rt = new List<string>();
            foreach (var k in keys)
            {
                rt.Add(k);
            }
            return rt;
        }
        public async Task<List<string>> KeysAsync(string pattern)
        {
            var conn = GetConnectRedis();
            IServer server = await conn.GetServerAsync(conn.GetEndPoints()[0]);
            var keys = server.KeysAsync(DbNum, pattern);
            List<string> rt = new List<string>();
            await foreach (var k in keys)
            {
                rt.Add(k);
            }
            return rt;
        }
        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        public void SetPrefixKey(string customKey)
        {
            _prefixKey = customKey;
        }

        #endregion 其他

        #region 辅助方法

        private string AddPrefixKey(string oldKey)
        {
            return _prefixKey + oldKey;
        }

        private T Do<T>(Func<IDatabase, T> func)
        {
            IDatabase database = GetConnectRedis().GetDatabase(DbNum);
            try
            {
                return func(database);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        private async Task<T> DoAsync<T>(Func<IDatabase, Task<T>> func)
        {
            IDatabase database = await GetConnectRedis().GetDatabaseAsync(DbNum);
            try
            {
                return await func(database);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : System.Text.Json.JsonSerializer.Serialize(value, JsonMessageSerializerConfig.SerializeOptions);
            return result;
        }

        private T ConvertObj<T>(RedisValue value)
        {
            if (value.IsNullOrEmpty)
            {
                return default(T);
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)value.ToString();
            }
            return System.Text.Json.JsonSerializer.Deserialize<T>(value, JsonMessageSerializerConfig.ObjectOptions);
        }
        private List<string> ConvetStrList(RedisValue[] values)
        {
            List<string> result = new List<string>();
            foreach (var item in values)
            {
                result.Add((string)item);
            }
            return result;
        }
        private List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        private RedisKey[] ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }


        #endregion 辅助方法


    }
}
