using ChannelUtility.Message;
using Common;
using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace IoTAIService
{
    public class AICache
    {
        private ITAServiceProvider _provider;
        public AICache(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public int GetVideoInt(string videoId, string key, int def = 0)
        {
            var cache = _provider.GetService<CacheHelper>();
            var videoNum = cache.GetCache<string>($"AIVideo:{videoId}:{key}");
            if (string.IsNullOrEmpty(videoNum))
            {
                return def;
            }
            else
            {
                return Convert.ToInt32(videoNum);
            }
        }
        public void SetVideoInt(string videoId, string key, int val)
        {
            var cache = _provider.GetService<CacheHelper>();
            cache.SetCache<string>($"AIVideo:{videoId}:{key}", val.ToString(), DateTime.Now.AddSeconds(120));
        }
        public List<AIConfigData> GetVideoAIConfig(string videoId)
        {
            var cache = _provider.GetService<CacheHelper>();
            return cache.GetCache<List<AIConfigData>>($"AIConfig:{videoId}");
        }
        public void SetVideoAIConfig(string videoId, List<AIConfigData> configs)
        {
            var cache = _provider.GetService<CacheHelper>();
            cache.SetCache<List<AIConfigData>>($"AIConfig:{videoId}", configs, DateTime.Now.AddMinutes(10));
        }
    }
}
