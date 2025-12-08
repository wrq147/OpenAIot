using AuthService;
using Common;
using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace SMSService
{
    /// <summary>
    /// 通用的json格式接口访问短信平台
    /// </summary>
    public class JsonSmsHelper : ISmsHelper
    {
        private ITAServiceProvider _provider;
        public JsonSmsHelper(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<bool> SendSMSCode(string phone, string templateCode, IDictionary<string, string> parameters)
        {
            var smsconfig = await _provider.GetService<ConfigBLL>().SelectConfigByKey("sms.json");
            if (string.IsNullOrEmpty(smsconfig))
            {
                return false;
            }
            SmsJsonConfig jsonconfig = System.Text.Json.JsonSerializer.Deserialize<SmsJsonConfig>(smsconfig, MyDefaultTextJsonConfig.DefaultOptions);
            string tUrl = jsonconfig.url;
            string tOk = jsonconfig.ok;

            object tccobj;
            if (!jsonconfig.sms_template.TryGetValue(templateCode, out tccobj))
            {
                return false;
            }
            var tcc = System.Text.Json.JsonSerializer.Serialize(tccobj, MyDefaultTextJsonConfig.DefaultOptions);
            //替换手机号
            tcc = tcc.Replace("$mobile", phone);
            //替换参数
            foreach (var prp in parameters)
            {
                tcc = tcc.Replace("$" + prp.Key, prp.Value);
            }

            var rsp = await HttpHelper.Instance.PostJsonAsync(tUrl, tcc, Encoding.UTF8).ConfigureAwait(false);
            if (rsp != null && rsp.Contains(tOk))
            {
                return true;
            }
            return false;
        }
    }

    public class SmsJsonConfig
    {
        public Dictionary<string, object> sms_template { get; set; }
        public string url { get; set; }
        public string ok { get; set; }
    }
}
