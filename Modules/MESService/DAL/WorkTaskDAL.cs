using Common;
using Common.Share;
using MESService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using ProducerService.Model;
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
            Expression<Func<MZ_WorkTask, MZ_WorkOrder, MZ_ProductOper, MZ_Product, bool>> expression = (a, b, c, d) => a.OrgId == orgId;
            if (query.Status == 0)
            {
                expression = expression.And((a, b, c, d) => a.IsFinish == false);
            }
            else if (query.Status == 1)
            {
                expression = expression.And((a, b, c, d) => a.IsFinish == true);
            }
            if (!string.IsNullOrEmpty(query.WorkOrderId))
            {
                expression = expression.And((a, b, c, d) => a.WorkOrderId == query.WorkOrderId);
            }
            if (!string.IsNullOrEmpty(query.OperId))
            {
                expression = expression.And((a, b, c, d) => a.OperId == query.OperId);
            }
            if (query.beginTime != null)
            {
                expression = expression.And((a, b, c, d) => a.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a, b, c, d) => a.CreatedOn <= query.endTime);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_WorkTask>()
                .LeftJoin<MZ_WorkOrder>((a, b) => a.WorkOrderId == b.Id).LeftJoin<MZ_ProductOper>((a, b, c) => a.OperId == c.Id).LeftJoin<MZ_Product>((a, b, c, d) => a.ProductId == d.Id)
                .Where(expression, "a.*,b.WorkNumber,c.OperName,c.AssignedUser,d.SkuNumber,d.ProductName");
            return await tmpSql.GeneratePageObjectAsync(query, "a.CreatedOn desc");
        }
        public virtual async Task<List<string>> SelectTaskByOrgId(long orgId)
        {
            var tmpSql = new SqlBuilder(help).Append("select Id from mz_work_task where OrgId=").AppendParam(orgId);
            return (await tmpSql.DoAsync<DoQuerySql<string>>()).ToList();
        }
    }
}
