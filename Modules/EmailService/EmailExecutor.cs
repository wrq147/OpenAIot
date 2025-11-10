using AuthService;
using Common.EventBus;
using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace EmailService
{
    public class EmailExecutor
    {
        private ITAServiceProvider _provider;
        private EmailSenderHelper _emailSender;
        public EmailExecutor(ITAServiceProvider provider, EmailSenderHelper emailSender)
        {
            _provider = provider;
            _emailSender = emailSender;
        }
        public async Task SyncNoticeMessage(NoticeEvent evt)
        {
            if (evt.NoticeWay.Contains("EMAIL"))
            {
                if (evt.TargetType == "设备告警")
                {
                    string msgLevel = string.Empty;
                    switch (evt.Level)
                    {
                        case 0:
                            msgLevel = "普通";
                            break;
                        case 1:
                            msgLevel = "告警";
                            break;
                        case 2:
                            msgLevel = "紧急";
                            break;
                    }
                    foreach (var targetUser in evt.RecvUserId)
                    {
                        if (string.IsNullOrEmpty(targetUser.email)) continue;
                        await _emailSender.SendHtmlEmail(targetUser.email, $"【{msgLevel}】设备{evt.Label}告警", $"设备${evt.Label}发生事件：{evt.Content}，请尽快确认并修复");
                    }
                }
                else
                {
                    var generalOption = _provider.GetService<IOptions<GeneralOption>>();
                    foreach(var targetUser in evt.RecvUserId)
                    {
                        if (string.IsNullOrEmpty(targetUser.email)) continue;
                        if (string.IsNullOrEmpty(evt.TargetUrl))
                        {
                            await _emailSender.SendHtmlEmail(targetUser.email, evt.Label, evt.Content);
                        }
                        else
                        {
                            string cont = "<a href='" + generalOption.Value.url + evt.TargetUrl + "'>" + evt.Content + "</a>";
                            await _emailSender.SendHtmlEmail(targetUser.email, evt.Label, cont);
                        }
                    }
    

                }
            }

        }
    }
}
