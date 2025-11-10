using Common;
using Common.Share;
using MESService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MESService.DAL
{
    public class WorkTaskDAL : BaseRepository<MZ_WorkTask>
    {
        public virtual async Task<PageObject<MZ_WorkTask>> SelectByPage(In_WorkTaskList query, long orgId)
        {
            Expression<Func<MZ_WorkTask, bool>> expression = (a) => a.OrgId == orgId;
            var tmpSql = new SqlBuilder(help).Query<MZ_WorkTask>().Where(expression);
            if (query.Items != null && query.Items.Length > 0)
            {
                //过滤扩展字段
                foreach (var item in query.Items)
                {
                    item.AppendFilter(tmpSql, string.Empty);
                }
            }
            return await tmpSql.GeneratePageObjectAsync(query, "StartOn desc");
        }
        public virtual async Task<List<string>> SelectTaskByOrgId(long orgId)
        {
            var tmpSql = new SqlBuilder(help).Append("select Id from mz_work_task where OrgId=").AppendParam(orgId);
            return (await tmpSql.DoAsync<DoQuerySql<string>>()).ToList();
        }
    }
}
