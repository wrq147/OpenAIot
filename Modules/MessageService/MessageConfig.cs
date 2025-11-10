using AuthService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MessageService
{
    public class MessageConfig
    {
        private ITAServiceProvider _serviceProvider;
        public MessageConfig(ITAServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<MessageJson> GetJsonConfig()
        {
            var json = await _serviceProvider.GetService<ConfigBLL>().SelectConfigByKey("system.message");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<MessageJson>(json);
        }
    }
    public class MessageJson
    {
        /// <summary>
        /// 是否激活消息主动通知用户
        /// </summary>
        public bool active_notice { get; set; }
        /// <summary>
        /// 个推开放平台的AppId
        /// </summary>
        public string push_appid { get; set; }
        /// <summary>
        /// 个推开放平台的appkey
        /// </summary>
        public string push_appkey { get; set; }
        public string push_appsecret { get; set; }
        public string push_mastersecret { get; set; }
    }
}
