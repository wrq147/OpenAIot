using AuthService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace WeiXinService
{
    public class WeiXinConfig
    {
        private ITAServiceProvider _serviceProvider;
        public WeiXinConfig(ITAServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<WeiXinJson> GetJsonConfig()
        {
            var json = await _serviceProvider.GetService<ConfigBLL>().SelectConfigByKey("system.wx");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<WeiXinJson>(json);
        }

    }

    public class WeiXinJson
    {
        /// <summary>
        /// 默认使用的AppId
        /// </summary>
        public string default_appid { get; set; }
        /// <summary>
        /// 微信推送使用的AppId
        /// </summary>
        public string push_appid { get; set; }
        /// <summary>
        /// 推送模板
        /// </summary>
        public Dictionary<string, object> push_template { get; set; }
    }
}
