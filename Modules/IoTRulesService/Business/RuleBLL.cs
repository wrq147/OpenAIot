using AuthService;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using Esprima.Ast;
using IoTRulesService.DAL;
using IoTRulesService.Flow.Builder;
using IoTRulesService.Model;
using IoTRulesService.TimerUtil;
using IoTService;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.Extensions.Logging;
using MonitorService.Business;
using MonitorService.Model;
using MonitorService.Util;
using MyAccess.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.Business
{
    public class RuleBLL
    {
        private ITAServiceProvider _provider;
        private RuleTemplateDAL _ruleTemplate;
        private RuleTriggerDAL _ruleTrigger;
        private SnowflakeHelper _snowflake;

        public RuleBLL(ITAServiceProvider provider, RuleTemplateDAL ruleTemplate, RuleTriggerDAL ruleTrigger, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _ruleTemplate = ruleTemplate;
            _ruleTrigger = ruleTrigger;
            _snowflake = snowflake;
        }
        /// <summary>
        /// 定时心跳验证iot节点是否存活
        /// </summary>
        /// <returns></returns>
        public virtual async Task ExecuteSendHeartbeat()
        {
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            var dict = redis.HashGetAll<string>("RuleExeNodes");
            var serverBus = _provider.GetService<ServerBusProxy>();
            var isneedupdate = false;
            //发送心跳
            foreach (var kvp in dict)
            {
                if (DateTime.TryParse(kvp.Value, out DateTime dt))
                {
                    if (dt < DateTime.Now)
                    {
                        isneedupdate = true;
                        await redis.HashDeleteAsync("RuleExeNodes", kvp.Key);
                    }
                }
                await serverBus.TestUpNode(kvp.Key);
            }

            //判断是否存在超时
            if (isneedupdate)
            {
                //通知更新所有节点监听者
                await serverBus.PublishNodeChange();
            }
        }
        public virtual async Task ExecuteProductTime(TimeEvent evt)
        {
            var redis = _provider.GetService<IotRedisHelper>();
            await redis.WaitReadLockAsync("NodeIdx", TimeSpan.FromSeconds(60));
            try
            {
                var serverBus = _provider.GetService<ServerBusProxy>();
                var devDAL = _provider.GetService<IotDeviceDAL>();
                var idx = serverBus.GetNodeIdx();
                var tdevlist = await devDAL.SelectDevicesByIdx(evt.ProductId, idx);
                foreach (var dev in tdevlist)
                {
                    //初始化参数
                    Dictionary<string, string> newparamType = new Dictionary<string, string>();
                    Dictionary<string, object> newparams = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(evt.HttpParams))
                    {
                        var tmprdict = await redis.GetRuleVal(evt.RuleId, dev.Id);
                        if (tmprdict == null)
                        {
                            tmprdict = new Dictionary<string, object>();
                        }
                        else
                        {
                            if (tmprdict.Count > 0)
                            {
                                foreach (var key in tmprdict.Keys)
                                {
                                    newparams.Add(key, tmprdict[key]);
                                }
                            }
                        }

                        if (evt.HasValue)
                        {
                            DateTime curtime = DateTime.Now;
                            long lastTime;
                            if (newparams.ContainsKey("@@LastTime"))
                            {
                                lastTime = Convert.ToInt64(newparams["@@LastTime"]);
                            }
                            else
                            {
                                lastTime = TypeConvert.Time2Unix(curtime);
                            }
                            long deltaSecond = TypeConvert.Time2Unix(curtime) - Convert.ToInt64(lastTime);
                            if (newparams.ContainsKey("TimeDelta"))
                            {
                                newparams["TimeDelta"] = Convert.ToInt64(newparams["TimeDelta"]) + deltaSecond;
                            }
                            else
                            {
                                newparams.Add("TimeDelta", deltaSecond);
                            }
                            if (newparams.ContainsKey("TriggerDelta"))
                            {
                                newparams["TriggerDelta"] = deltaSecond;
                            }
                            else
                            {
                                newparams.Add("TriggerDelta", deltaSecond);
                            }
                            if (newparams.ContainsKey("@@LastTime"))
                            {
                                newparams["@@LastTime"] = TypeConvert.Time2Unix(curtime);
                            }
                            else
                            {
                                newparams.Add("@@LastTime", TypeConvert.Time2Unix(curtime));
                            }
                        }

                        var paramlist = System.Text.Json.JsonSerializer.Deserialize<List<BaseInputValue>>(evt.HttpParams, TslModel.TSLOptions);
                        foreach (var item in paramlist)
                        {
                            newparamType.Add(item.code, item.type);
                            if (!newparams.ContainsKey(item.code))
                            {
                                object val;
                                if (item.defval != null)
                                {
                                    val = item.defval;
                                }
                                else
                                {
                                    continue;
                                }
                                newparams.Add(item.code, val);
                            }
                        }
                    }

                    bool isDebug = _provider.GetService<RuleCache>().IsDebug(evt.RuleId);
                    var executor = await _provider.GetService<RuleExecutorBuilder>().Build(evt.RuleJson);
                    ExecuteRuleMessage msg = new ExecuteRuleMessage();
                    msg.ProductId = evt.ProductId;
                    msg.DeviceId = dev.DeviceId;
                    msg.Inputs = newparams;
                    msg.TriggerWay = 2;
                    var extonext = await executor.Execute(msg, newparamType, newparams, isDebug, evt.RuleId);
                    await redis.SaveRuleVal(evt.RuleId, dev.Id, extonext.GetGlobalParams());
                }
            }
            catch (Exception ex)
            {
                _provider.GetService<ILoggerFactory>().CreateLogger<RuleBLL>().LogError(ex.Message + ex.StackTrace);
                return;
            }
            finally
            {
                await redis.ReleaseReadLockAsync("NodeIdx");
            }
        }
        /// <summary>
        /// http触发与定时器触发
        /// </summary>
        /// <param name="id"></param>
        /// <param name="triggerWay"></param>
        /// <param name="context"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<List<StreamData>>> Execute(long id, int triggerWay, QuartzContext context, Dictionary<string, object> inputs = null)
        {
            try
            {
                var template = await _ruleTemplate.Select(id);
                if (template == null)
                {
                    await TimerSchedule.DeleteJob(id);
                    return BusResponse<List<StreamData>>.Error(110, "规则不存在");
                }
                if (template.TriggerWay != triggerWay)
                {
                    return BusResponse<List<StreamData>>.Error(111, "触发方式错误");
                }
                if (template.Status == "1")
                {
                    if (context != null && context.PreviousFireTimeUtc != null)
                    {
                        await TimerSchedule.DeleteJob(id);
                    }
                    return BusResponse<List<StreamData>>.Error(115, "规则暂停中");
                }

                var tmpItems = await _ruleTrigger.SelectList(x => x.RuleId == template.Id);
                if (tmpItems.Count > 0)
                {
                    string[] prodids = tmpItems[0].TopicDevice.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    string prdid = prodids[0];
                    if (!string.IsNullOrEmpty(prdid))
                    {
                        TimeEvent evt = new TimeEvent();
                        evt.RuleId = id;
                        evt.HasValue = context.PreviousFireTimeUtc.HasValue;
                        evt.ProductId = prdid;
                        evt.HttpParams = template.HttpParams;
                        evt.RuleJson = template.RuleJson;
                        await TAEventDispatcher.Instance.Dispatch(TimeEvent.EventKey, evt);
                        return BusResponse<List<StreamData>>.Success(null);
                    }
                }

                //初始化参数
                Dictionary<string, string> newparamType = new Dictionary<string, string>();
                Dictionary<string, object> newparams = new Dictionary<string, object>();
                if (inputs == null)
                {
                    inputs = new Dictionary<string, object>();
                }
                var redis = _provider.GetService<IotRedisHelper>();

                if (!string.IsNullOrEmpty(template.HttpParams))
                {
                    var paramlist = System.Text.Json.JsonSerializer.Deserialize<List<BaseInputValue>>(template.HttpParams, TslModel.TSLOptions);
                    if (template.TriggerWay == 2)
                    {
                        var tparamDict = paramlist.ToDictionary((x) => x.code);
                        var tmprdict = await redis.GetRuleVal(id, tparamDict);
                        if (tmprdict.Count > 0)
                        {
                            foreach (var key in tmprdict.Keys)
                            {
                                newparams.Add(key, tmprdict[key]);
                            }
                        }
                        if (context.PreviousFireTimeUtc.HasValue)
                        {
                            DateTime curtime = DateTime.Now;
                            long lastTime;
                            if (newparams.ContainsKey("@@LastTime"))
                            {
                                lastTime = Convert.ToInt64(newparams["@@LastTime"]);
                            }
                            else
                            {
                                lastTime = TypeConvert.Time2Unix(curtime);
                            }
                            long deltaSecond = TypeConvert.Time2Unix(curtime) - Convert.ToInt64(lastTime);
                            if (newparams.ContainsKey("TimeDelta"))
                            {
                                newparams["TimeDelta"] = Convert.ToInt64(newparams["TimeDelta"]) + deltaSecond;
                            }
                            else
                            {
                                newparams.Add("TimeDelta", deltaSecond);
                            }
                            if (newparams.ContainsKey("TriggerDelta"))
                            {
                                newparams["TriggerDelta"] = deltaSecond;
                            }
                            else
                            {
                                newparams.Add("TriggerDelta", deltaSecond);
                            }
                            if (newparams.ContainsKey("@@LastTime"))
                            {
                                newparams["@@LastTime"] = TypeConvert.Time2Unix(curtime);
                            }
                            else
                            {
                                newparams.Add("@@LastTime", TypeConvert.Time2Unix(curtime));
                            }
                        }

                    }
                    foreach (var item in paramlist)
                    {
                        newparamType.Add(item.code, item.type);
                        if (!newparams.ContainsKey(item.code))
                        {
                            object val;
                            if (!inputs.TryGetValue(item.code, out val))
                            {
                                if (item.defval != null)
                                {
                                    val = item.defval;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            newparams.Add(item.code, val);
                        }
                    }
                }


                bool isDebug = _provider.GetService<RuleCache>().IsDebug(id);
                var executor = await _provider.GetService<RuleExecutorBuilder>().Build(template.RuleJson);
                ExecuteRuleMessage msg = new ExecuteRuleMessage();
                msg.ProductId = "ExecuteRule";
                msg.DeviceId = null;
                msg.Inputs = newparams;
                msg.TriggerWay = template.TriggerWay.Value;
                var extonext = await executor.Execute(msg, newparamType, newparams, isDebug, id);
                if (template.TriggerWay == 2)
                {
                    await redis.SaveRuleVal(id, extonext.GetGlobalParams());
                }


                return BusResponse<List<StreamData>>.Success(extonext.Data);
            }
            catch (Exception ex)
            {
                _provider.GetService<ILoggerFactory>().CreateLogger<RuleBLL>().LogError(ex.Message + ex.StackTrace);
                return null;
            }

        }

        public virtual async Task<PageObject<MZ_RuleTemplate>> ListPage(In_ListPage query, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return PageObject<MZ_RuleTemplate>.Empty();
            }
            string groupPath = null;
            if (query.GroupId != null)
            {
                var groupDAL = _provider.GetService<RuleGroupDAL>();
                MZ_RuleGroup cls = await groupDAL.Select(query.GroupId);
                if (cls != null)
                {
                    groupPath = cls.Path;
                }
            }
            var tlistpage = await _ruleTemplate.SelectByPage(query, groupPath, user.OrgId);
            foreach (var item in tlistpage.List)
            {
                if (!string.IsNullOrEmpty(item.HttpParams))
                {
                    var paramlist = System.Text.Json.JsonSerializer.Deserialize<List<BaseInputValue>>(item.HttpParams, TslModel.TSLOptions);
                    if (paramlist.Where(x => !x.readOnly).Any())
                    {
                        item.EnableParam = true;
                        continue;
                    }
                }
                item.EnableParam = false;
            }
            return tlistpage;
        }
        public virtual async Task<BusResponse<MZ_RuleTemplate>> GetRule(long id)
        {
            MZ_RuleTemplate template = await _ruleTemplate.Select(id);
            if (template == null)
            {
                return BusResponse<MZ_RuleTemplate>.Error(21, "模板不存在");
            }
            template.TriggerList = await _ruleTrigger.SelectList(x => x.RuleId == id);
            if (!string.IsNullOrEmpty(template.TimerCron))
            {
                template.CronName = ScheduleUtils.ToChineseDescription(template.TimerCron);
            }
            return BusResponse<MZ_RuleTemplate>.Success(template);
        }
        public virtual async Task<BusResponse<long>> AddRule(MZ_RuleTemplate data)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);

            if (string.IsNullOrEmpty(data.CreatedFrom))
            {
                data.CreatedFrom = "pc";
            }
            data.GroupId ??= string.Empty;
            data.TimerCron ??= string.Empty;
            data.Id = _snowflake.NextId();
            data.Status = "0";
            data.Remark ??= string.Empty;
            data.Sort ??= 0;
            data.OrgId = user.OrgId;
            data.SetCreateBy(user);
            data.TimerJobId = 0;
            if (data.TriggerWay == 2)
            {
                await TimerSchedule.CreateJob(data.Id.Value, new List<string>() { data.TimerCron });
            }
            else
            {
                data.TimerCron = string.Empty;
            }

            await _ruleTemplate.Insert(data);
            if (data.TriggerList != null && data.TriggerList.Count > 0)
            {
                foreach (var item in data.TriggerList)
                {

                    item.RuleId = data.Id.Value;
                }
                //清除缓存
                await TAEventDispatcher.Instance.Dispatch(RuleChangeEvent.EventKey, new RuleChangeEvent(data.TriggerList));
                await _ruleTrigger.Insert(data.TriggerList);
            }
            return BusResponse<long>.Success(data.Id.Value);
        }

        public virtual async Task<BusResponse<int>> EditRule(MZ_RuleTemplate data, IUserInfo user)
        {
            MZ_RuleTemplate old = await _ruleTemplate.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(31, "规则不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "无权编辑当前规则");
            }
            if (old.GroupId == null && data.GroupId == null)
            {
                data.GroupId = string.Empty;
            }
            data.OrgId = null;
            data.CreatedFrom = null;
            data.SetUpdateBy(user);
            data.TimerJobId = 0;
            if (old.TriggerWay == 2)
            {
                if (old.Status == "1" && data.Status == "0")
                {
                    string cronExp = data.TimerCron == null ? old.TimerCron : data.TimerCron;
                    await TimerSchedule.CreateJob(old.Id.Value, new List<string>() { cronExp });
                }
                else if (old.Status == "0" && data.Status == "1")
                {
                    await TimerSchedule.DeleteJob(old.Id.Value);
                }
                else
                {
                    if (!string.IsNullOrEmpty(data.TimerCron) && data.TimerCron != old.TimerCron)
                    {
                        //重启定时任务
                        await TimerSchedule.DeleteJob(old.Id.Value);
                        string cronExp = data.TimerCron == null ? old.TimerCron : data.TimerCron;
                        await TimerSchedule.CreateJob(old.Id.Value, new List<string>() { cronExp });
                    }

                }
            }

            if (!string.IsNullOrEmpty(data.HttpParams))
            {
                var paramlist = System.Text.Json.JsonSerializer.Deserialize<List<BaseInputValue>>(data.HttpParams, TslModel.TSLOptions);
                foreach (var paramitem in paramlist)
                {
                    if (paramitem.defval != null)
                    {
                        switch (paramitem.type)
                        {
                            case "int":
                                paramitem.defval = Convert.ToInt64(paramitem.defval);
                                break;
                            case "float":
                                paramitem.defval = Convert.ToDouble(paramitem.defval);
                                break;
                            case "boolean":
                                paramitem.defval = Convert.ToBoolean(paramitem.defval);
                                break;
                        }
                    }

                }
                data.HttpParams = System.Text.Json.JsonSerializer.Serialize(paramlist, TslModel.TSLOptions);
            }

            data.TriggerWay = null;
            int rs = await _ruleTemplate.Update(data);

            //旧的订阅通知删除
            var tmpItems = await _ruleTrigger.SelectList(x => x.RuleId == data.Id);
            if (tmpItems.Count > 0)
            {
                await TAEventDispatcher.Instance.Dispatch(RuleChangeEvent.EventKey, new RuleChangeEvent(tmpItems));
            }

            if (data.TriggerList != null)
            {
                await _ruleTrigger.Delete(x => x.RuleId == data.Id);
                foreach (var item in data.TriggerList)
                {
                    item.RuleId = data.Id.Value;
                }
                await TAEventDispatcher.Instance.Dispatch(RuleChangeEvent.EventKey, new RuleChangeEvent(data.TriggerList));

                await _ruleTrigger.Insert(data.TriggerList);
            }
            return BusResponse<int>.Success(rs);
        }
        public virtual async Task<BusResponse<int>> Reset(long id, IUserInfo user)
        {
            MZ_RuleTemplate old = await _ruleTemplate.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(31, "规则不存在");
            }
            MZ_RuleTemplate data = new MZ_RuleTemplate();
            data.Id = id;
            if (old.TriggerWay == 2)
            {
                await TimerSchedule.DeleteJob(old.Id.Value);
                string cronExp = data.TimerCron == null ? old.TimerCron : data.TimerCron;
                await TimerSchedule.CreateJob(old.Id.Value, new List<string>() { cronExp });
            }
            data.SetUpdateBy(user);
            await _ruleTemplate.Update(data);
            await _provider.GetService<IotRedisHelper>().DeleteRule(id);
            return BusResponse<int>.Success();
        }
        public virtual async Task<BusResponse<int>> Remove(long id)
        {
            var rule = await _ruleTemplate.Select(id);
            if (rule == null)
            {
                return BusResponse<int>.Error(31, "规则不存在");
            }
            if (rule.Status != "1")
            {
                return BusResponse<int>.Error(31, "暂停的规则才能删除");
            }
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            if (rule.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "无权删除当前规则");
            }
            try
            {
                await _ruleTemplate.Delete(id);

                await _provider.GetService<IotRedisHelper>().DeleteRule(rule.Id.Value);

                //旧的订阅通知删除
                var tmpItems = await _ruleTrigger.SelectList(x => x.RuleId == id);
                if (tmpItems.Count > 0)
                {
                    await TAEventDispatcher.Instance.Dispatch(RuleChangeEvent.EventKey, new RuleChangeEvent(tmpItems));
                }

                await _ruleTrigger.Delete(x => x.RuleId == id);


                if (rule.TimerJobId != null && rule.TimerJobId > 0)
                {
                    await _provider.GetService<JobBLL>().DeleteJob(rule.TimerJobId.Value);
                }

                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(51, ex.Message);
            }

        }
    }
}
