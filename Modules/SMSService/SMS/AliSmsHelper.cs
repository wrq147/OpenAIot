using AuthService;
using Common.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace SMSService
{
    public class AliSmsHelper : ISmsHelper
    {
        private ITAServiceProvider _provider;
        public AliSmsHelper(ITAServiceProvider provider)
        {
            _provider = provider;
        }


        public async Task<bool> SendSMSCode(string phone, string templateCode, IDictionary<string, string> parameters)
        {
            var smsconfig = await _provider.GetService<ConfigBLL>().SelectConfigByKey("sms.ali");
            if (string.IsNullOrEmpty(smsconfig))
            {
                return false;
            }
            SmsAliConfig aliconfig = System.Text.Json.JsonSerializer.Deserialize<SmsAliConfig>(smsconfig, MyDefaultTextJsonConfig.DefaultOptions);
            string accessKeyId = aliconfig.accessKeyId;
            string accessKeySecret = aliconfig.accessKeySecret;
            string signName = aliconfig.signName;

            var sender = new AliSmsSender(accessKeyId, accessKeySecret, signName);
            object tccobj;
            if (!aliconfig.sms_template.TryGetValue(templateCode, out tccobj))
            {
                return false;
            }
            var tcc = System.Text.Json.JsonSerializer.Serialize(tccobj, MyDefaultTextJsonConfig.DefaultOptions);
            var result = await sender.SendAsync(phone, tcc, parameters);
            if (result.Code == "OK")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }

    public class SmsAliConfig
    {
        public Dictionary<string, object> sms_template { get; set; }
        public string signName { get; set; }
        public string accessKeyId { get; set; }
        public string accessKeySecret { get; set; }
    }
}
