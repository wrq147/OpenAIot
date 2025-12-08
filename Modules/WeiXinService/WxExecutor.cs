using Common.EventBus;
using Common.Json;
using Common.Share;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using WeiXinService.DAL;

namespace WeiXinService
{
    public class WxExecutor
    {
        private ITAServiceProvider _provider;
        private ILogger<WxExecutor> _log;
        private IOptions<GeneralOption> _generalOption;
        public WxExecutor(ITAServiceProvider provider, ILoggerFactory factory, IOptions<GeneralOption> generalOption)
        {
            _provider = provider;
            _log = factory.CreateLogger<WxExecutor>();
            _generalOption = generalOption;
        }
        public async Task SyncNoticeMessage(NoticeEvent evt)
        {
            try
            {
                if (evt.RecvUserId.Length > 0)
                {
                    if (evt.NoticeWay.Contains("WX"))
                    {
                        var wxConfig = await _provider.GetService<WeiXinConfig>().GetJsonConfig();
                        var apiHelper = _provider.GetService<WxApiHelper>();
                        //微信公众号推送
                        if (!string.IsNullOrEmpty(wxConfig.push_appid) && wxConfig.push_template != null)
                        {
                            IDictionary<string, string> cp = new Dictionary<string, string>();
                            cp.Add("label", evt.Label);
                            cp.Add("content", evt.Content);
                            cp.Add("url", evt.TargetUrl);
                            cp.Add("type", evt.TargetType);
                            cp.Add("now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            object tccobj;
                            if (wxConfig.push_template.TryGetValue(evt.TargetType, out tccobj))
                            {
                                string tcc = System.Text.Json.JsonSerializer.Serialize(tccobj, MyDefaultTextJsonConfig.DefaultOptions);
                                //替换参数
                                foreach (var prp in cp)
                                {
                                    tcc = tcc.Replace("$" + prp.Key, prp.Value);
                                }

                                WxPushItem wxitem = System.Text.Json.JsonSerializer.Deserialize<WxPushItem>(tcc, MyDefaultTextJsonConfig.DefaultOptions);

                                var tpushwx = await apiHelper.AccountInfo(wxConfig.push_appid);
                                if (tpushwx != null)
                                {
                                    var wxDAL = _provider.GetService<WeiXinDAL>();
                                    foreach (var recv in evt.RecvUserId)
                                    {
                                        if (recv.uid == 0)
                                        {
                                            continue;
                                        }
                                        var recvWx = await wxDAL.SelectByUid(recv.uid, tpushwx.AppId);
                                        Dictionary<string, string> tdata = new Dictionary<string, string>();
                                        if (!string.IsNullOrEmpty(wxitem.firstData))
                                        {
                                            tdata.Add("first", wxitem.firstData);
                                        }
                                        if (!string.IsNullOrEmpty(wxitem.keyword1))
                                        {
                                            tdata.Add("keyword1", wxitem.keyword1);
                                        }
                                        if (!string.IsNullOrEmpty(wxitem.keyword2))
                                        {
                                            tdata.Add("keyword2", wxitem.keyword2);
                                        }
                                        if (!string.IsNullOrEmpty(wxitem.keyword3))
                                        {
                                            tdata.Add("keyword3", wxitem.keyword3);
                                        }
                                        if (!string.IsNullOrEmpty(wxitem.keyword4))
                                        {
                                            tdata.Add("keyword4", wxitem.keyword4);
                                        }
                                        if (!string.IsNullOrEmpty(wxitem.remark))
                                        {
                                            tdata.Add("remark", wxitem.remark);
                                        }

                                        await apiHelper.PushTemplateMsg(tpushwx.AppId, recvWx.OpenId, wxitem.templateid, wxitem.url, tdata, wxitem.miniprogram_appid, wxitem.miniprogram_pagepath);
                                    }

                                }



                            }

                        }
                        //企业微信推送
                        var usrCropDAL = _provider.GetService<UserCropDAL>();
                        var uids = evt.RecvUserId.Where(x => x.uid > 0).Select(x => x.uid).ToList();
                        var admwxlist = await usrCropDAL.SelectList(x => uids.Contains(x.Id.Value));
                        var appIds = admwxlist.Select(x => x.AppId).Distinct().ToList();
                        foreach (var appid in appIds)
                        {
                            var tmpacc = await apiHelper.AccountInfo(appid);
                            if (tmpacc.OrgId != evt.OrgId)
                            {
                                continue;
                            }
                            var tousers = admwxlist.Where(x => x.AppId == appid).Select(x => x.CropUserId).Distinct().ToList();
                            string sourceurl = "/flowable/todo?id=";
                            string tmptargeturl = evt.TargetUrl;
                            if (tmptargeturl.StartsWith(sourceurl))
                            {
                                tmptargeturl = tmptargeturl.Substring(sourceurl.Length);
                                tmptargeturl = _generalOption.Value.url + "/corp/#/pages_flow/process_detail?id=" + tmptargeturl + "&toDo=true";
                            }
                            else if (evt.TargetType == "PlaneTaskNotice")
                            {
                                tmptargeturl = tmptargeturl.Substring(evt.TargetUrl.IndexOf("?id=") + 4);
                                tmptargeturl = _generalOption.Value.url + "/corp/#/pages_device/device_info/task_detail?id=" + tmptargeturl;
                            }
                            else if (evt.TargetType == "PlaneGroupNotice")
                            {
                                tmptargeturl = _generalOption.Value.url + "/corp/#/pages_device/device_info/plane_task?isManage=true";
                            }
                            await apiHelper.PushCorpTemplateMsg(tmpacc.AppId, tousers, evt.Label, evt.Content, tmptargeturl);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                _log.LogError(e.Message + e.StackTrace);
            }

        }
    }
    public class WxPushItem
    {
        public string templateid { get; set; }
        public string url { get; set; }
        public string miniprogram_appid { get; set; }
        public string miniprogram_pagepath { get; set; }
        public string firstData { get; set; }
        public string keyword1 { get; set; }
        public string keyword2 { get; set; }
        public string keyword3 { get; set; }
        public string keyword4 { get; set; }
        public string remark { get; set; }
    }
}
