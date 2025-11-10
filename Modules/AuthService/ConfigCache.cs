using Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace AuthService
{
    public class ConfigCache
    {
        private IMemoryCache _cache;
        private ILogger<ConfigCache> _log;
        public ConfigCache(ILoggerFactory factory)
        {
            _cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            _log = factory.CreateLogger<ConfigCache>();
        }
        public bool SetCache<T>(string key, T value, DateTime? expireTime = null)
        {
            try
            {
                if (expireTime == null)
                {
                    return _cache.Set<T>(key, value) != null;
                }
                else
                {
                    return _cache.Set(key, value, (expireTime.Value - DateTime.Now)) != null;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
            return false;
        }

        public bool Remove(string key)
        {
            _cache.Remove(key);
            return true;
        }
        public T? GetCache<T>(string key)
        {
            var value = _cache.Get<T>(key);
            return value;
        }
        public void Clear()
        {
            _cache.Dispose();
            _cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        }
    }
}
