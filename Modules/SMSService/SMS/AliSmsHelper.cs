using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TemplateAction.Core;
using AuthService;

namespace SMSService
{
    public class AliSmsHelper : ISmsHelper
    {
        private ITAServiceProvider _provider;
        public AliSmsHelper(ITAServiceProvider provider)
        {
            _provider = provider;
        }


        public async Task<bool> SendSMSCode(string phone, string templateCode, JObject parameters)
        {
            var smsconfig = await _provider.GetService<ConfigBLL>().SelectConfigByKey("sms.ali");
            if (string.IsNullOrEmpty(smsconfig))
            {
                return false;
            }
            SmsAliConfig aliconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<SmsAliConfig>(smsconfig);
            string accessKeyId = aliconfig.accessKeyId;
            string accessKeySecret = aliconfig.accessKeySecret;
            string signName = aliconfig.signName;

            var sender = new AliSmsSender(accessKeyId, accessKeySecret, signName);
            object tccobj;
            if (!aliconfig.sms_template.TryGetValue(templateCode, out tccobj))
            {
                return false;
            }
            var tcc = Newtonsoft.Json.JsonConvert.SerializeObject(tccobj);

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
