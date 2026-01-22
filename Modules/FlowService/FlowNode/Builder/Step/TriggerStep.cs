using AuthService;
using Common.EventBus;
using Common.Json;
using FlowService.Business;
using FlowService.DAL;
using FlowService.Model;
using Jint;
using Jint.Native;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 触发器
    /// </summary>
    public class TriggerStep : WorkflowStep
    {
        private static HttpClient client = new HttpClient();
        public TriggerProps props { get; set; }
        public TriggerStep()
        {
            this.PersistenceNode = false;
        }

        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            if (props.type == "WEBHOOK")
            {
                //调用接口
                HttpRequestMessage req = new HttpRequestMessage();
                req.Method = new HttpMethod(props.http.method);
                req.RequestUri = new Uri(props.http.url);
                foreach (var hd in props.http.headers)
                {
                    if (hd.isField)
                    {
                        req.Headers.Add(hd.name, context.GetFormValue(hd.value));
                    }
                    else
                    {
                        req.Headers.Add(hd.name, hd.value);
                    }
                }
                SortedDictionary<string, string> reqparams = new SortedDictionary<string, string>();
                foreach (var pd in props.http.xparams)
                {
                    if (pd.isField)
                    {
                        reqparams.Add(pd.name, context.GetFormValue(pd.value));
                    }
                    else
                    {
                        reqparams.Add(pd.name, pd.value);
                    }
                }
                if (props.http.contentType == "JSON")
                {
                    req.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(reqparams, MyDefaultTextJsonConfig.DefaultOptions), Encoding.UTF8, "application/json");
                }
                else if (props.http.contentType == "FORM")
                {
                    req.Content = new FormUrlEncodedContent(reqparams);
                }

                var response = await client.SendAsync(req);
                if (props.http.handlerByScript)
                {
                    try
                    {
                        var engine = new Engine();
                        engine.SetValue("setFormByTitle", new Action<string, string>((x, y) =>
                        {
                            context.SetFormByTitle(x, y);
                        })).Execute(props.http.script);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var responseBytes = await response.Content.ReadAsByteArrayAsync();
                            var res = Encoding.UTF8.GetString(responseBytes);
                            var okres = await Task.Run(() =>
                            {
                                return engine.Invoke("handlerOk", res);
                            });

                            if (!okres.AsBoolean())
                            {
                                return await ExecutionResult.End();
                            }
                        }
                        else
                        {
                            object errparam = new
                            {
                                code = (int)response.StatusCode,
                                message = response.ReasonPhrase
                            };
                            var errres = await Task.Run(() =>
                            {
                                return engine.Invoke("handlerFail", JsValue.FromObject(engine, errparam));
                            });
                            if (!errres.AsBoolean())
                            {
                                return await ExecutionResult.End();
                            }

                        }
                    }
                    catch
                    {
                        context.ExecutionPointer.Status = Model.NodeStatus.Failed;
                        return await ExecutionResult.End();
                    }

                }

            }
            else if (props.type == "EMAIL")
            {
                //发送邮件
                List<TargetUser> users = new List<TargetUser>();
                foreach (string targeturl in props.email.to)
                {
                    users.Add(new TargetUser()
                    {
                        email = targeturl
                    });
                }

                var nt = new NoticeEvent(2, users.ToArray(), new string[] { "EMAIL" });
                nt.TargetType = "EmailList";
                nt.Content = props.email.content;
                nt.Label = props.email.subject;
                context.NoticeList.Add(nt);
            }
            else if (props.type == "NEWFLOW")
            {
                var taskBLL = context.ServiceProvider.GetService<TaskBLL>();
                var userDAL = context.ServiceProvider.GetService<UserDAL>();
                var trilogDAL = context.ServiceProvider.GetService<FlowTrilogDAL>();
                if (this.props.flow.templateId == context.Workflow.TemplateId.Value)
                {
                    throw new Exception("无法重复创建相同模板的子流程01");
                }
                var tmptrilog = await trilogDAL.Select(context.Workflow.FlowNumber);
                HashSet<long> tmptriset = null;
                if (tmptrilog != null && !string.IsNullOrEmpty(tmptrilog.TemplateIdSet))
                {
                    tmptriset = System.Text.Json.JsonSerializer.Deserialize<HashSet<long>>(tmptrilog.TemplateIdSet, MyDefaultTextJsonConfig.DefaultOptions);
                    if (tmptriset.Contains(this.props.flow.templateId))
                    {
                        throw new Exception("无法重复创建相同模板的子流程02");
                    }
                }
                Dictionary<string, List<Out_UserItem>> tmpassign = new Dictionary<string, List<Out_UserItem>>();
                if (props.flow.assign != null)
                {
                    List<long> assignuids = new List<long>();
                    foreach (var tmpaa in props.flow.assign)
                    {
                        var tmplist = context.GetUserFormById(tmpaa.fieldid);
                        var tmpusers = await userDAL.GetUserListByIds(tmplist);
                        List<Out_UserItem> tmpou = new List<Out_UserItem>();
                        foreach (var user in tmpusers)
                        {
                            tmpou.Add(new Out_UserItem() { Id = user.Id.Value.ToString(), Avatar = user.Avatar, RealName = user.RealName });
                        }
                        tmpassign.Add(tmpaa.NodeId, tmpou);
                    }
                }

                Dictionary<string, object> newModel = new Dictionary<string, object>();
                if (props.flow.items == null)
                {
                    props.flow.items = Array.Empty<NewFlowItem>();
                }
                foreach (var item in props.flow.items)
                {
                    if (!string.IsNullOrEmpty(item.fieldid))
                    {
                        if (context.FormItems.TryGetValue(item.fieldid, out object val))
                        {
                            newModel.Add(item.id, val);
                        }
                    }
                }


                long creator = context.Workflow.createId.Value;
                if (!string.IsNullOrEmpty(props.flow.creator))
                {
                    List<long> uids = context.GetUserFormById(props.flow.creator);
                    if (uids.Count > 0)
                    {
                        creator = uids[0];
                    }
                }
                //创建子流程创建记录
                MZ_FlowTrilog tmpt = new MZ_FlowTrilog();
                tmpt.FlowNumber = "CF" + context.Workflow.Id.Value;
                if (tmptriset == null)
                {
                    tmptriset = new HashSet<long>();
                }
                tmptriset.Add(context.Workflow.TemplateId.Value);
                tmpt.TemplateIdSet = System.Text.Json.JsonSerializer.Serialize(tmptriset, MyDefaultTextJsonConfig.DefaultOptions);
                await trilogDAL.Insert(tmpt);
                if (context.FormItems.TryGetValue("@from", out object fromnumber))
                {
                    newModel.Add("@from", fromnumber);
                }
                if (context.FormItems.TryGetValue("@fromtype", out object fromtype))
                {
                    newModel.Add("@fromtype", fromtype);
                }
                newModel.Add("@FlowNumber", tmpt.FlowNumber);

                var res = await taskBLL.CreateFlow(props.flow.templateId, newModel, tmpassign, 2, context.Workflow.IsEmbed ?? false, creator);
                if (res.Code == 0)
                {
                    var fdd = (MZ_FormData)res.Data;
                    if (!string.IsNullOrEmpty(props.flow.flowas))
                    {
                        if (context.FormItems.ContainsKey(props.flow.flowas))
                        {
                            context.FormItems[props.flow.flowas] = fdd.FlowNumber;
                        }
                    }
                }
                else
                {
                    throw new Exception("子流程创建失败：" + res.Message);
                }
            }
            return await ExecutionResult.Next();
        }
    }
}
