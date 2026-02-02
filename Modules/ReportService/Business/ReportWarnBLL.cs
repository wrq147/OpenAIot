using AuthService;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using Microsoft.Extensions.Options;
using MonitorService.Business;
using MonitorService.Model;
using MonitorService.Util;
using MyAccess.DB.Builder.WhereToSql;
using ReportService.DAL;
using ReportService.Models;
using ReportService.TimerUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;

namespace ReportService.Business
{
    public class ReportWarnBLL
    {
        private ReportWarnDAL _reportWarnDAL;
        private SnowflakeHelper _snowflake;
        private ITAServiceProvider _provider;
        public ReportWarnBLL(ReportWarnDAL reportWarnDAL, SnowflakeHelper snowflake, ITAServiceProvider serviceProvider)
        {
            _reportWarnDAL = reportWarnDAL;
            _snowflake = snowflake;
            _provider = serviceProvider;
        }

        public virtual async Task Execute(string id, QuartzContext context)
        {
            var warnInfo = await _reportWarnDAL.Select(id);
            if (warnInfo == null)
            {
                await TimerSchedule.DeleteWarnJob(id);
                return;
            }

            WarnJsonConfig warnJson = null;
            try
            {
                warnJson = System.Text.Json.JsonSerializer.Deserialize<WarnJsonConfig>(warnInfo.ConditionJson, MyDefaultTextJsonConfig.DefaultOptions);
            }
            catch { }
            if (warnJson == null)
            {
                return;
            }

            MZ_Report report = await _provider.GetService<ReportDAL>().Select(warnInfo.ReportId);
            if (report == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(report.ThemeOption))
            {
                return;
            }
            ReportThemeOption themeOption = null;
            try
            {
                themeOption = System.Text.Json.JsonSerializer.Deserialize<ReportThemeOption>(report.ThemeOption, MyDefaultTextJsonConfig.DefaultOptions);
            }
            catch { }

            if (themeOption == null)
            {
                return;
            }
            var tDataSet = themeOption.globalData.Where(x => x.name == warnJson.DataSet).FirstOrDefault();
            if (tDataSet == null)
            {
                return;
            }

            if (tDataSet.dataSourceType != "database")
            {
                return;
            }

            var rs = await _provider.GetService<DataSourceBLL>().AnalysisToDataTable(tDataSet.database.type, tDataSet.database.ipAdress, tDataSet.database.port, tDataSet.database.username, tDataSet.database.password, tDataSet.database.baseName, tDataSet.database.executeSql, 0);
            if (rs.IsSuccess())
            {
                if (rs.Data.Rows.Count > 0)
                {
                    var dictList = MyAccess.Core.TypeConvert.DataTableToDict(rs.Data);
                    bool isCondiSuc = false;
                    if (warnJson.Conditions.Count > 0)
                    {
                        isCondiSuc = dictList.Where(dict =>
                        {
                            int condidxi = 0;
                            bool canExe = true;
                            foreach (var condition in warnJson.Conditions)
                            {
                                if (dict.TryGetValue(condition.field, out object nwval))
                                {
                                    bool curcondrs = false;
                                    switch (condition.valtype)
                                    {
                                        case "number":
                                            {
                                                double sourec1 = Convert.ToDouble(nwval);
                                                double sourec2 = Convert.ToDouble(condition.val);
                                                switch (condition.compare)
                                                {
                                                    case ">":
                                                        curcondrs = sourec1 > sourec2;
                                                        break;
                                                    case "<":
                                                        curcondrs = sourec1 < sourec2;
                                                        break;
                                                    case "==":
                                                        curcondrs = sourec1 == sourec2;
                                                        break;
                                                    case "><":
                                                        curcondrs = sourec1 != sourec2;
                                                        break;
                                                    case ">=":
                                                        curcondrs = sourec1 >= sourec2;
                                                        break;
                                                    case "<=":
                                                        curcondrs = sourec1 <= sourec2;
                                                        break;
                                                }
                                            }
                                            break;
                                        default:
                                            return false;
                                    }
                                    if (condidxi == 0)
                                    {
                                        canExe = curcondrs;
                                    }
                                    else
                                    {
                                        var ccccidd = condidxi - 1;
                                        if (warnJson.Groups.Count > ccccidd)
                                        {
                                            if (warnJson.Groups[ccccidd] == "&")
                                            {
                                                canExe = canExe && curcondrs;
                                            }
                                            else
                                            {
                                                canExe = canExe || curcondrs;
                                            }
                                        }
                                    }
                                    ++condidxi;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            return canExe;
                        }).Count() > 0;
                    }

                    if (isCondiSuc)
                    {
                        //计算沉默周期
                        var redis = _provider.GetService<GeneralRedisHelper>();
                        string warnQuickKey = "ReportWarnQuick::" + warnInfo.Id;
                        string redisval = await redis.StringGetAsync(warnQuickKey);
                        if (redisval != null)
                        {
                            if (warnInfo.SilenceTime.ToString() != redisval)
                            {
                                await redis.KeyDeleteAsync(warnQuickKey);
                            }
                            else
                            {
                                return;
                            }
                        }
                        await redis.StringSetAsync(warnQuickKey, warnInfo.SilenceTime.ToString(), TimeSpan.FromSeconds(warnInfo.SilenceTime.Value));

                        //符合通知条件
                        var userDAL = _provider.GetService<UserDAL>();
                        List<MZ_AdminInfo> recvList;
                        if (warnInfo.NoticeUserType == 0)
                        {
                            string[] tusers = warnInfo.NoticeUsers.Split(',');
                            List<long> longArray = new List<long>();
                            foreach (var tstr in tusers)
                            {
                                longArray.Add(long.Parse(tstr));
                            }
                            recvList = await userDAL.GetUserListByIds(longArray);
                        }
                        else if (warnInfo.NoticeUserType == 1)
                        {
                            string[] troles = warnInfo.NoticeUsers.Split(',');
                            List<long> longArray = new List<long>();
                            foreach (var tstr in troles)
                            {
                                longArray.Add(long.Parse(tstr));
                            }
                            var userIds = await userDAL.SelectUserByRoles(longArray);
                            recvList = await userDAL.GetUserListByIds(userIds);
                        }
                        else
                        {
                            return;
                        }
                        string targetUrl = string.Empty;
                        if (!string.IsNullOrEmpty(warnInfo.ShareId))
                        {
                            string pushTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            var shareData = await _provider.GetService<ShareDAL>().Select(warnInfo.ShareId);
                            if (shareData != null)
                            {
                                string turl = _provider.GetService<IOptions<GeneralOption>>().Value.url;
                                if (report.ReportType == "table")
                                {
                                    targetUrl = $"{turl}/#/report/spreadSheet/viewDataReport?tokenid={shareData.Id}&t={Uri.EscapeDataString(pushTime)}";
                                }
                                else
                                {
                                    targetUrl = $"{turl}/#/report/datav/datavRelease?tokenid={shareData.Id}&t={Uri.EscapeDataString(pushTime)}";
                                }

                            }
                        }

                        List<TargetUser> targets = new List<TargetUser>();
                        foreach (var recvId in recvList)
                        {
                            targets.Add(new TargetUser()
                            {
                                uid = recvId.Id.Value,
                                email = recvId.Email,
                                phone = recvId.Mobile
                            });
                        }
                        var nt = new NoticeEvent(2, targets.ToArray(), warnInfo.NoticeWay.Split(','));
                        nt.OrgId = warnInfo.OrgId.Value;
                        nt.TargetType = "ReportWarn";
                        nt.TargetUrl = targetUrl;
                        nt.Content = $"【{report.Name}】在" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + $"发生数据预警【{warnInfo.Name}】,请尽快处理！";
                        nt.Label = warnInfo.Name;
                        await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                    }

                }
            }
        }
        public async Task<PageObject<MZ_ReportWarn>> ListPage(In_ReportWarnPage query)
        {
            var user = _provider.GetUser();
            Expression<Func<MZ_ReportWarn, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.ReportId))
            {
                expression = expression.And(x => x.ReportId == query.ReportId);
            }
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.create_time <= query.endTime);
            }
            var tpage = await _reportWarnDAL.SelectPage(expression, query, string.Empty);
            return tpage;
        }


        public virtual async Task<BusResponse<MZ_ReportWarn>> Info(string id)
        {
            var old = await _reportWarnDAL.Select(id);
            if (old == null)
            {
                return BusResponse<MZ_ReportWarn>.Error(111, "告警不存在");
            }
            if (!string.IsNullOrEmpty(old.TimerCron))
            {
                old.CronName = ScheduleUtils.ToChineseDescription(old.TimerCron);
            }
            if (!string.IsNullOrEmpty(old.NoticeUsers))
            {
                var tuserIds = old.NoticeUsers.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(s => long.Parse(s));
                old.NoticeUserList = await _provider.GetService<UserDAL>().GetUserListByIds(tuserIds);
            }
            return BusResponse<MZ_ReportWarn>.Success(old);
        }


        public virtual async Task<BusResponse<string>> Add(MZ_ReportWarn data, IUserInfo user)
        {
            try
            {
                data.SetCreateBy(user);
                data.Id = _snowflake.NextId().ToString();
                data.OrgId = user.OrgId;
                data.NoticeUserType ??= 0;
                data.ShareId ??= string.Empty;
                if (data.Status == "0")
                {
                    await TimerSchedule.CreateWarnJob(data.Id,new List<string>() { data.TimerCron });
                }

                await _reportWarnDAL.Insert(data);
                return BusResponse<string>.Success(data.Id);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> Update(MZ_ReportWarn data, IUserInfo user)
        {
            try
            {
                var old = await _reportWarnDAL.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<string>.Error(103, "告警不存在");
                }
                if (data.Status == "0" && old.Status == "1")
                {
                    string cronStr = string.IsNullOrEmpty(data.TimerCron) ? old.TimerCron : data.TimerCron;
                    await TimerSchedule.CreateWarnJob(data.Id, new List<string>() { cronStr });
                }
                else if (data.Status == "1" && old.Status == "0")
                {
                    await TimerSchedule.DeleteWarnJob(data.Id);
                }
                else if (old.Status == "0" && !string.IsNullOrEmpty(data.TimerCron) && old.TimerCron != data.TimerCron)
                {
                    await TimerSchedule.DeleteWarnJob(data.Id);
                    await TimerSchedule.CreateWarnJob(data.Id, new List<string>() { data.TimerCron });
                }
                data.SetUpdateBy(user);
                await _reportWarnDAL.Update(data, x => x.Id == data.Id && x.OrgId == user.OrgId);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Remove(string id, IUserInfo user)
        {
            var context = _provider.GetService<ITAContext>();
            var old = await _reportWarnDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "告警不存在");
            }
            if (old.Status != "1")
            {
                return BusResponse<int>.Error(112, "请先停用告警");
            }
            return BusResponse<int>.Success(await _reportWarnDAL.Delete(x => x.Id == id && x.OrgId == user.OrgId));
        }

    }
}
