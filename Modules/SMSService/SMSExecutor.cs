using AuthService;
using Common;
using Common.EventBus;
using Common.Share;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace SMSService
{
    public class SMSExecutor
    {
        private ITAServiceProvider _provider;
        private ILogger<SMSExecutor> _log;
        public SMSExecutor(ITAServiceProvider provider, ILoggerFactory factory)
        {
            _provider = provider;
            _log = factory.CreateLogger<SMSExecutor>();
        }
        private async Task<ISmsHelper> GetSmsHelper(string type)
        {
            var sms_type = await _provider.GetService<ConfigBLL>().SelectConfigByKey($"system.sms.{type}");
            if (string.IsNullOrEmpty(sms_type))
            {
                sms_type = await _provider.GetService<ConfigBLL>().SelectConfigByKey("system.sms");
            }
            //添加短信服务
            switch (sms_type)
            {
                case "ali":
                    return _provider.GetService<AliSmsHelper>();
                case "json":
                    return _provider.GetService<JsonSmsHelper>();
                case "shanyun":
                    return _provider.GetService<ShanYunSmsHelper>();
                default:
                    return _provider.GetService<AliSmsHelper>();
            }
        }
        public async Task SyncNoticeMessage(NoticeEvent evt)
        {
            if (evt.NoticeWay.Contains("SMS"))
            {
                ISmsHelper smsHelper = await GetSmsHelper(evt.TargetType);
                foreach (var targetUser in evt.RecvUserId)
                {
                    if (string.IsNullOrEmpty(targetUser.phone)) continue;
                    if (evt.TargetType == "验证码")
                    {
                        IDictionary<string, string> cp = new Dictionary<string, string>();
                        cp.Add("code", evt.Content);
                        if (!await smsHelper.SendSMSCode(targetUser.phone, evt.TargetType, cp))
                        {
                            _log.LogError("短信验证码发送失败");
                        }
                    }
                    else if (evt.TargetType == "跳转短信")
                    {
                        var generOption = _provider.GetService<IOptions<GeneralOption>>();
                        var context = _provider.GetService<ITAContext>();
                        IDictionary<string, string> cp = new Dictionary<string, string>();
                        string tmpurl = string.IsNullOrEmpty(generOption.Value.url) ? ("http://" + IpHelper.GetAvaOutIp()) : generOption.Value.url;
                        cp.Add("url", tmpurl + "/wx?t=" + evt.Content);
                        if (!await smsHelper.SendSMSCode(targetUser.phone, evt.TargetType, cp))
                        {
                            _log.LogError("跳转短信发送失败");
                        }
                    }
                    else if (evt.TargetType == "邀请短信")
                    {
                        IDictionary<string, string> cp = new Dictionary<string, string>();
                        cp.Add("url", evt.Content);
                        if (!await smsHelper.SendSMSCode(targetUser.phone, evt.TargetType, cp))
                        {
                            _log.LogError("邀请短信发送失败");
                        }
                    }
                    else
                    {
                        IDictionary<string, string> cp = new Dictionary<string, string>();
                        cp.Add("label", evt.Label);
                        cp.Add("content", evt.Content);
                        cp.Add("url", evt.TargetUrl);
                        cp.Add("type", evt.TargetType);
                        if (!await smsHelper.SendSMSCode(targetUser.phone, evt.TargetType, cp))
                        {
                            _log.LogError("短信发送失败");
                        }
                    }
                }

            }

        }
    }
}
