using ChannelUtility;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void SetVideoInt(string videoId, string key,int val)
        {
            var cache = _provider.GetService<CacheHelper>();
            cache.SetCache<string>($"AIVideo:{videoId}:{key}", val.ToString(), DateTime.Now.AddSeconds(120));
        }
    }
}
