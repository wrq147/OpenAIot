using AuthService;
using Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace SMSService
{
    /// <summary>
    /// 闪云短信平台
    /// </summary>
    public class ShanYunSmsHelper : ISmsHelper
    {
        private ITAServiceProvider _provider;
        private ILogger<ShanYunSmsHelper> _log;
        public ShanYunSmsHelper(ITAServiceProvider provider, ILoggerFactory factory)
        {
            _provider = provider;
            _log = factory.CreateLogger<ShanYunSmsHelper>();
        }

        public async Task<bool> SendSMSCode(string phone, string templateCode, JObject parameters)
        {
            var smsconfig = await _provider.GetService<ConfigBLL>().SelectConfigByKey("sms.shanyun");
            if (string.IsNullOrEmpty(smsconfig))
            {
                return false;
            }
            SmsShanYunConfig configObj = Newtonsoft.Json.JsonConvert.DeserializeObject<SmsShanYunConfig>(smsconfig);
            if (configObj == null)
            {
                return false;
            }
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            string tkey = "SMS" + DateTime.Now.ToString("yyMMddHHmm");
            long val = await tmpredis.StringIncrementLongAsync(tkey);
            await tmpredis.KeyExpireAsync(tkey, TimeSpan.FromMinutes(2));
            var tmpOrderId = tkey + (val + 1).ToString().PadLeft(2, '0');

            string tccContent;
            if (!configObj.sms_template.TryGetValue(templateCode, out tccContent))
            {
                return false;
            }
            //替换参数
            var props = parameters.Properties();
            foreach (var prp in props)
            {
                string tmpxx = prp.Value.ToString();
                tccContent = tccContent.Replace("$" + prp.Name, tmpxx);
            }
            List<SmsShanYunSendItem> items = new List<SmsShanYunSendItem>();
            items.Add(new SmsShanYunSendItem()
            {
                phoneNumber = phone,
                content = tccContent,
                outOrderId = tmpOrderId
            });

            var bodyTimestamp = MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now).ToString();
            Dictionary<string, object> userBody = new Dictionary<string, object>();
            userBody.Add("sendList", items);
            if (!string.IsNullOrEmpty(configObj.signatureStr))
            {
                userBody.Add("signatureStr", configObj.signatureStr);
            }

            var userBodyStr = Newtonsoft.Json.JsonConvert.SerializeObject(userBody);
            var signStr = MyAccess.Core.Crypter.MD5(userBodyStr + configObj.secret + bodyTimestamp);
            var tcc = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                userAccount = configObj.account,
                businessBody = userBody,
                timestamp = bodyTimestamp,
                sign = signStr
            });

            var rsp = await HttpHelper.Instance.PostJsonAsync("http://apiext.szshanyun.com:8089/receive", tcc, Encoding.UTF8).ConfigureAwait(false);
            if (rsp != null && rsp.Contains("操作成功"))
            {
                return true;
            }
            _log.LogError(rsp);
            return false;
        }
    }

    public class SmsShanYunConfig
    {
        public string account { get; set; }
        public string secret { get; set; }
        /// <summary>
        /// 签名字符串
        /// </summary>
        public string signatureStr { get; set; }
        public Dictionary<string, string> sms_template { get; set; }
    }
    public class SmsShanYunSendItem
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 客户订单号，必须是唯一值
        /// </summary>
        public string outOrderId { get; set; }
    }
}
