using AuthService;
using Common.IdGenerator;
using Common.Share;
using JiebaNet.Segmenter;
using MonitorService.Business;
using MyAccess.DB;
using ReportService.DAL;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace ReportService.Business
{
    public class ReportBLL
    {
        private ReportDAL _reportDAL;
        private SnowflakeHelper _snowflake;
        private ITAServiceProvider _provider;
        public ReportBLL(ReportDAL reportDAL, SnowflakeHelper snowflake, ITAServiceProvider serviceProvider)
        {
            _reportDAL = reportDAL;
            _snowflake = snowflake;
            _provider = serviceProvider;
        }

        public virtual async Task<PageObject<MZ_Report>> ListPage(In_ReportListPage query)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            query.OrgId = user.OrgId;

            string groupPath = null;
            if (query.GroupId != null)
            {
                var groupDAL = _provider.GetService<ReportGroupDAL>();
                MZ_ReportGroup cls = await groupDAL.Select(query.GroupId);
                if (cls != null)
                {
                    groupPath = cls.Path;
                }
            }
            return await _reportDAL.SelectByPage(query, groupPath);
        }
        public virtual async Task<DateTime?> GetReportUpdateTime(string id)
        {
            var updateTime = await _reportDAL.SelectReportUpdateTime(id);
            return updateTime;
        }

        public virtual async Task<BusResponse<MZ_Report>> Info(string id, IUserInfo user)
        {
            var report = await _reportDAL.Select(id);
            if (report == null)
            {
                return BusResponse<MZ_Report>.Error(111, "报表不存在");
            }
            if (user != null)
            {
                if (report.OrgId != user.OrgId)
                {
                    return BusResponse<MZ_Report>.Error(112, "无权查看当前报表");
                }
            }

            return BusResponse<MZ_Report>.Success(report);
        }
        public virtual async Task<BusResponse<int>> CopyReport(long id)
        {
            try
            {
                MZ_Report old = await _reportDAL.Select(id);
                if (old == null)
                {
                    return BusResponse<int>.Error(103, "当前报表不存在");
                }
                var context = _provider.GetService<ITAContext>();
                var user = Data_ServerTokenInfo.From(context);
                old.Name += "-复制";
                old.Id = _snowflake.NextId().ToString();
                old.SetCreateBy(user);
                old.SetUpdateBy(user);
                old.Status = "0";

                return BusResponse<int>.Success(await _reportDAL.Insert(old));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<string>> AddReport(MZ_Report data)
        {
            try
            {
                if (data.Name != null || data.DesInfo != null)
                {
                    data.Tag = string.Join(" ", new JiebaSegmenter().CutForSearch(data.Name + " " + data.DesInfo ?? string.Empty));
                }
                var context = _provider.GetService<ITAContext>();
                var user = Data_ServerTokenInfo.From(context);
                if (user.OrgId <= 0)
                {
                    return BusResponse<string>.Error(112, "非企业用户无法添加报表");
                }

                data.SetCreateBy(user);
                data.Id = _snowflake.NextId().ToString();
                data.OrgId = user.OrgId;
                switch (data.DeviceType)
                {
                    case "pc":
                        data.Resolution = "1920*1080";
                        data.DrawOption = "[]";
                        break;
                    case "phone":
                        data.Resolution = "375*667";
                        data.DrawOption = "[]";
                        break;
                    case "double":
                        data.Resolution = "{\"pc\":\"1920*1080\",\"phone\":\"375*667\"}";
                        data.DrawOption = "{\"pc\":[],\"phone\":[]}";
                        break;
                    default:
                        data.Resolution = "1920*1080";
                        data.DrawOption = "[]";
                        break;
                }

                data.ThemeOption = string.Empty;
                data.MapOption = string.Empty;
                data.Zindex = 0;
                data.IdGlobal = 0;
                data.Thumbnail = string.Empty;
                data.Status = "0";
                data.del_flag = "0";
                await _reportDAL.Insert(data);
                return BusResponse<string>.Success(data.Id);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> UpdateReport(MZ_Report data)
        {
            try
            {
                MZ_Report old = await _reportDAL.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<int>.Error(103, "当前报表不存在");
                }
                if (data.Name != null || data.DesInfo != null)
                {
                    string newname = data.Name ?? old.Name;
                    string newIntro = data.DesInfo ?? old.DesInfo;
                    data.Tag = string.Join(" ", new JiebaSegmenter().CutForSearch(data.Name + " " + data.DesInfo ?? string.Empty));
                }
                var context = _provider.GetService<ITAContext>();
                var user = Data_ServerTokenInfo.From(context);
                data.SetUpdateBy(user);
                return BusResponse<int>.Success(await _reportDAL.Update(data, x => x.Id == data.Id && x.OrgId == user.OrgId));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            var rs = await _reportDAL.Delete(x => x.Id == id && x.OrgId == user.OrgId);

            var shareDAL = _provider.GetService<ShareDAL>();
            var shareList = await shareDAL.SelectList(x => x.ReportId == id && x.OrgId == user.OrgId);
            foreach (var delshare in shareList)
            {
                if (delshare.TimerJobId > 0)
                {
                    await _provider.GetService<JobBLL>().DeleteJob(delshare.TimerJobId.Value);
                }
            }
            await shareDAL.Delete(x => x.ReportId == id && x.OrgId == user.OrgId);
            return BusResponse<int>.Success(rs);
        }

    }
}
