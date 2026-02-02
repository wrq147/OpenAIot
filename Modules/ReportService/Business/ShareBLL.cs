using AuthService;
using Common;
using Common.EventBus;
using Common.Share;
using DeveloperService.Business;
using Microsoft.Extensions.Options;
using MonitorService.Business;
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

namespace ReportService.Business
{
    public class ShareBLL
    {
        private ReportDAL _reportDAL;
        private ShareDAL _shareDAL;
        private ITAServiceProvider _provider;
        public ShareBLL(ReportDAL reportDAL, ShareDAL shareDAL, ITAServiceProvider serviceProvider)
        {
            _reportDAL = reportDAL;
            _shareDAL = shareDAL;
            _provider = serviceProvider;
        }
        public virtual async Task<PageObject<MZ_ReportShare>> ListPage(In_ShareListPage query)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            Expression<Func<MZ_ReportShare, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.SearchKey))
            {
                string isexistGroup = "EXISTS(select Id from mz_report where Id=mz_report_share.ReportId and Name like '%" + StringHelper.SqlLikeFilter(query.SearchKey) + "%')";
                expression = expression.And(x => SonSqlFun.SqlCondition(isexistGroup));
            }
            if (!string.IsNullOrEmpty(query.ReportId))
            {
                expression = expression.And(x => x.ReportId == query.ReportId);
            }
            var sharePage = await _shareDAL.SelectPage(expression, query, "create_time desc");

            var reportDict = await _reportDAL.NavigateDict<MZ_ReportShare, string>(sharePage.List, x => !string.IsNullOrEmpty(x.ReportId), x => x.ReportId);
            foreach (var shareItem in sharePage.List)
            {
                if (reportDict.TryGetValue(shareItem.ReportId, out MZ_Report newrport))
                {
                    shareItem.ReportName = newrport.Name;
                    shareItem.ReportType = newrport.ReportType;
                }
                shareItem.CronName = ScheduleUtils.ToChineseDescription(shareItem.TimerCron);
            }

            return sharePage;
        }
        public virtual async Task<BusResponse<MZ_ReportShare>> Info(string id)
        {
            var shareInfo = await _shareDAL.Select(id);
            if (shareInfo == null)
            {
                return BusResponse<MZ_ReportShare>.Error(111, "该分享不存在");
            }
            if (!string.IsNullOrEmpty(shareInfo.TimerCron))
            {
                shareInfo.CronName = ScheduleUtils.ToChineseDescription(shareInfo.TimerCron);
            }
            if (!string.IsNullOrEmpty(shareInfo.NoticeUsers))
            {
                var tuserIds = shareInfo.NoticeUsers.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(s => long.Parse(s));
                if (shareInfo.NoticeUserType == 0)
                {
                    shareInfo.NoticeUserList = await _provider.GetService<UserDAL>().GetUserListByIds(tuserIds);
                }
                else if (shareInfo.NoticeUserType == 1)
                {
                    shareInfo.NoticeRoleList = await _provider.GetService<UserDAL>().GetRoleListByIds(tuserIds);
                }
            }

            var report = await _reportDAL.Select(shareInfo.ReportId);
            if (report != null)
            {
                shareInfo.ReportName = report.Name;
                shareInfo.ReportType = report.ReportType;
            }
            return BusResponse<MZ_ReportShare>.Success(shareInfo);
        }
        public virtual async Task<BusResponse<string>> Delete(string[] ids)
        {
            var user = _provider.GetUser();
            try
            {
                foreach (var id in ids)
                {
                    var shareInfo = await _shareDAL.Select(id);
                    if (shareInfo == null)
                    {
                        continue;
                    }
                    if (shareInfo.TimerJobId > 0)
                    {
                        await _provider.GetService<JobBLL>().DeleteJob(shareInfo.TimerJobId.Value);
                    }
                }
                await _shareDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.Id));
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<int>> Edit(MZ_ReportShare data)
        {
            try
            {
                var old = await _shareDAL.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<int>.Error(103, "分享不存在");
                }
                var user = _provider.GetUser();
                if (old.OrgId != user.OrgId)
                {
                    return BusResponse<int>.Error(104, "无权修改");
                }
                if (data.TimerStatus == "0" && old.TimerStatus == "1")
                {
                    await TimerSchedule.CreateShareJob(data.Id, new List<string>() { data.TimerCron });
                }
                else if (data.TimerStatus == "1" && old.TimerStatus == "0")
                {
                    await TimerSchedule.DeleteShareJob(data.Id);
                }
                else if (data.TimerStatus == "0" && old.TimerStatus == "0")
                {
                    if (!string.IsNullOrEmpty(data.TimerCron) && data.TimerCron != old.TimerCron)
                    {
                        await TimerSchedule.DeleteShareJob(data.Id);
                        await TimerSchedule.CreateShareJob(data.Id, new List<string>() { data.TimerCron });
                    }
                }
                data.SetUpdateBy(user);
                await _shareDAL.Update(data, x => x.Id == data.Id && x.OrgId == user.OrgId);
                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<MZ_ReportShare>> Share(MZ_ReportShare data)
        {
            var report = await _reportDAL.Select(data.ReportId);
            if (report == null)
            {
                return BusResponse<MZ_ReportShare>.Error(211, "分享的报表不存在");
            }
            data.OrgId = report.OrgId;
            if (data.EffectiveTime == 0)
            {
                data.ExpirationTime = DateTime.Now.AddYears(1000);
            }
            else
            {
                data.ExpirationTime = DateTime.Now.AddDays(data.EffectiveTime.Value);
            }

            var user = _provider.GetUser();
            if (user.OrgId != report.OrgId)
            {
                return BusResponse<MZ_ReportShare>.Error(211, "无法分享非当前企业的报表");
            }

            var developerBLL = _provider.GetService<DeveloperBLL>();
            var userDevRes = await developerBLL.Profile();
            if (userDevRes.Data != null)
            {
                data.TokenStr = user.ToShareToken(_provider, userDevRes.Data.SecKey);
            }
            else
            {
                data.TokenStr = user.ToShareToken(_provider, string.Empty);
            }

            data.Id = MyAccess.Core.StringTool.GetGUID();
            data.SetCreateBy(user);
            data.TimerCron ??= string.Empty;
            data.NoticeUserType ??= 0;
            data.NoticeUsers ??= string.Empty;
            data.UsingPassword ??= string.Empty;
            data.TimerJobId = 0;
            if (data.TimerStatus == "0")
            {
                await TimerSchedule.CreateShareJob(data.Id, new List<string>() { data.TimerCron });
            }

            await _shareDAL.Insert(data);
            return BusResponse<MZ_ReportShare>.Success(data);
        }

        public virtual async Task Execute(string id, QuartzContext context)
        {
            var shareInfo = await _shareDAL.Select(id);
            if (shareInfo == null)
            {
                await TimerSchedule.DeleteShareJob(id);
                return;
            }
            MZ_Report report = await _reportDAL.Select(shareInfo.ReportId);
            if (report == null)
            {
                await TimerSchedule.DeleteShareJob(id);
                return;
            }
            var userDAL = _provider.GetService<UserDAL>();
            List<MZ_AdminInfo> recvList;
            if (shareInfo.NoticeUserType == 0)
            {
                string[] tusers = shareInfo.NoticeUsers.Split(',');
                List<long> longArray = new List<long>();
                foreach (var tstr in tusers)
                {
                    longArray.Add(long.Parse(tstr));
                }
                recvList = await userDAL.GetUserListByIds(longArray);
            }
            else if (shareInfo.NoticeUserType == 1)
            {
                string[] troles = shareInfo.NoticeUsers.Split(',');
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
            string turl = _provider.GetService<IOptions<GeneralOption>>().Value.url;
            string pushTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var nt = new NoticeEvent(2, targets.ToArray(), shareInfo.NoticeWay.Split(','));
            nt.OrgId = shareInfo.OrgId.Value;
            nt.TargetType = "ReportShare";
            if (report.ReportType == "table")
            {
                nt.TargetUrl = $"{turl}/#/report/spreadSheet/viewDataReport?tokenid={shareInfo.Id}&t={Uri.EscapeDataString(pushTime)}";
            }
            else
            {
                nt.TargetUrl = $"{turl}/#/report/datav/datavRelease?tokenid={shareInfo.Id}&t={Uri.EscapeDataString(pushTime)}";
            }
            nt.Content = $"本报表由系统在{pushTime}向您推送,请点击查看！";
            nt.Label = report.Name;
            await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
        }

    }
}
