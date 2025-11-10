using AuthService;
using Common;
using Common.EventBus;
using MessageService.Business;
using MessageService.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace MessageService
{
    public class NoticeExecutor
    {
        private ITAServiceProvider _provider;
        private ILogger<NoticeExecutor> _log;
        public NoticeExecutor(ITAServiceProvider provider, ILoggerFactory factory)
        {
            _provider = provider;
            _log = factory.CreateLogger<NoticeExecutor>();
        }
        public async Task SyncNoticeMessage(NoticeEvent evt)
        {
            try
            {
                if (evt.NoticeWay.Contains("APP") && evt.RecvUserId[0].uid > 0)
                {
                    var tmpconfig = await _provider.GetService<MessageConfig>().GetJsonConfig();
                    var msgBLL = _provider.GetService<MessageBLL>();
                    MZ_Message msg = new MZ_Message();
                    msg.content = evt.Content;
                    msg.label = evt.Label;
                    msg.sender_id = evt.SendUserId;
                    msg.click_type = evt.TargetType ?? string.Empty;
                    msg.click_url = evt.TargetUrl ?? string.Empty;
                    msg.receiver_id = evt.RecvUserId[0].uid;
                    msg.OrgId = evt.OrgId;
                    await msgBLL.AddMessage(msg);

                    var pushBLL = _provider.GetService<PushBLL>();
                    GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
                    foreach (var recv in evt.RecvUserId)
                    {
                        MZ_Message_Log log = new MZ_Message_Log();
                        log.messsage_id = msg.id;
                        log.receiver_id = recv.uid;
                        log.status = 0;
                        await msgBLL.AddMessageLog(log);


                        //站内新消息提醒
                        if (tmpconfig.active_notice)
                        {
                            await TAEventDispatcher.Instance.Dispatch("/Mqtt.User.New", recv.uid.ToString());
                        }

                        //离线个推提醒
                        if (!string.IsNullOrEmpty(tmpconfig.push_appid) && !string.IsNullOrEmpty(tmpconfig.push_appkey))
                        {
                            ////用户在线不推送
                            //string searchKeys = "Token" + recv.uid + "_*";
                            //var keys = await redis.KeysAsync(searchKeys);
                            //if (keys.Count > 0)
                            //{
                            //    continue;
                            //}
                            if (evt.Label.Length < 1)
                            {
                                continue;
                            }
                            string tmptile = evt.Label;
                            if (tmptile.Length > 50)
                            {
                                tmptile = tmptile.Substring(0, 50);
                            }

                            var rsp = await pushBLL.PushSingle(recv.uid, tmptile, evt.Content, "{\"click_type\":\"" + msg.click_type + "\",\"click_url\":\"" + msg.click_url + "\"}");
                            if (!rsp.IsSuccess() && rsp.Code != 1)
                            {
                                _log.LogError(rsp.Message);
                            }
                        }

                    }
                }
            }
            catch(Exception e)
            {
                _log.LogError(e.Message+e.StackTrace);
            }
      
        }
    }
}
