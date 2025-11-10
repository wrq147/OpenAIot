using Microsoft.Extensions.Caching.Memory;
using System;

namespace Common.UserAgent
{
    public class UserAgentHelper
    {
        private static UserAgentSettings _settings = new UserAgentSettings();
        private static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        public static UserAgent Parse(string? userAgentString)
        {
            userAgentString = (userAgentString?.Length > _settings.UaStringSizeLimit) ? userAgentString?.Trim().Substring(0, _settings.UaStringSizeLimit) : userAgentString?.Trim();
            return _cache.GetOrCreate(userAgentString, entry =>
            {
                entry.SlidingExpiration = _settings.CacheSlidingExpiration;
                if (_settings.AbsoluteExpirationRelativeToNow != null) entry.AbsoluteExpirationRelativeToNow = _settings.AbsoluteExpirationRelativeToNow;
                entry.Size = 1;
                return new UserAgent(_settings, userAgentString);
            });
        }

    }
}
