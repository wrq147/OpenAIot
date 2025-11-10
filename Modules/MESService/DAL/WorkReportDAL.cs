using Common;
using Common.Share;
using MESService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MESService.DAL
{
    public class WorkReportDAL : BaseRepository<MZ_WorkReport>
    {
        public virtual async Task<PageObject<MZ_WorkReport>> SelectByPage(In_ReportList query, IUserInfo user)
        {
            Expression<Func<MZ_WorkReport, bool>> expression = (a) => a.OrgId == user.OrgId;

            if (query.OnlyMy == true)
            {
                expression = expression.And((a) => a.createId == user.UserId);
            }

            if (query.beginTime != null)
            {
                expression = expression.And((a) => a.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a) => a.create_time <= query.endTime);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_WorkReport>().Include(a => a.RepBat, x => x.BatchNo).Where(expression);
            if (query.Items != null && query.Items.Length > 0)
            {
                //过滤扩展字段
                foreach (var item in query.Items)
                {
                    item.AppendFilter(tmpSql, "RepBat.");
                }
            }

            return await tmpSql.GeneratePageObjectAsync(query, "a.create_time desc");
        }
        public virtual async Task<Out_WorkTaskInfo> SelectTotal(string taskId)
        {
            var tmpSql = new SqlBuilder(help).Append("select sum(GoodNum) as TotalGoodNum,sum(DefectNum),sum(WorkTime) as TotalDefectNum from mz_work_report where WorkTaskId=").AppendParam(taskId).Append(" and Status=2");
            return (await tmpSql.DoAsync<DoQuerySql<Out_WorkTaskInfo>>()).ToFirst();
        }
    }
}
