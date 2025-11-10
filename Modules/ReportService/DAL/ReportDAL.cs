using Common;
using Common.Share;
using MyAccess.DB;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Builder.WhereToSql;
using AuthService;
using JiebaNet.Segmenter;
using TemplateAction.Label;

namespace ReportService.DAL
{
    public class ReportDAL : BaseRepository<MZ_Report>
    {
        public virtual async Task<bool> ExistReport(string groupPath)
        {
            var rs = await new SqlBuilder(help).Query<MZ_Report>().Append("select r.Id from mz_report r left join mz_report_group g on r.GroupId = g.Id where g.Path like ")
       .AppendParam(groupPath + '%').Append(" limit 1").ToFirstAsync();
            return rs != null ? true : false;
        }
        public virtual async Task<PageObject<MZ_Report>> SelectByPage(In_ReportListPage query, string groupPath)
        {
            return await new SqlBuilder(help).Query<MZ_Report>().Append("select Id,GroupId,DeviceType,Name,ReportType,DesInfo,Tag,Thumbnail,Status,create_time,update_time from mz_report where OrgId=").AppendParam(query.OrgId)
                .Then(!string.IsNullOrEmpty(groupPath), x =>
                {
                    x.Append(" and EXISTS(select Id from mz_report_group where mz_report.GroupId=Id and Path like '" + groupPath + "%')");
                })
                .Then(!string.IsNullOrEmpty(query.Key), x =>
                {
                    var keys = new JiebaSegmenter().CutForSearch(query.Key);
                    x.Append(" and ").FullSearch("Tag", keys);
                })
                .Then(!string.IsNullOrEmpty(query.Status), x => x.Append(" and Status=").AppendParam(query.Status))
            .GeneratePageObjectAsync(query, "create_time desc");
        }

        public virtual async Task<DateTime?> SelectReportUpdateTime(string id)
        {
            return (await new SqlBuilder(help).Append("select update_time from mz_report where Id=").Append(id).DoAsync<DoQuerySql<DateTime?>>()).ToFirstOrDefault(null);
        }
    }
}
