using Common;
using Common.Share;
using K4os.Hash.xxHash;
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
    public class WorkOrderDAL : BaseRepository<MZ_WorkOrder>
    {
        public virtual async Task<PageObject<MZ_WorkOrder>> SelectByPage(In_WorkOrderList query, long orgId)
        {
            Expression<Func<MZ_WorkOrder, MZ_ProductPlan, bool>> expression = (a, c) => a.OrgId == orgId;
            if (query.beginTime != null)
            {
                expression = expression.And((a, c) => a.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a, c) => a.CreatedOn <= query.endTime);
            }
            if (query.Status != null)
            {
                expression = expression.And((a, c) => a.Status == query.Status);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_WorkOrder>().Include(x => x.ProdInfo, x => x.ProductId).LeftJoin<MZ_ProductPlan>((a, c) => a.PlanId == c.Id).Where(expression);
            return await tmpSql.GeneratePageObjectAsync(query, "CreatedOn desc");
        }
        public virtual async Task<List<Out_ParentWordInfo>> SelectListByIds(List<string> ids)
        {
            var tmpSql = new SqlBuilder(help).Append("select a.Id,a.WorkNumber,a.ProductId,a.Status,a.Priority,a.PlannedStartOn,a.PlannedEndOn,b.ProductName,c.PlanName from mz_work_order a left join mz_product b on a.ProductId=b.Id left join mz_product_plan c on a.PlanId=c.Id");
            return (await tmpSql.DoAsync<DoQuerySql<Out_ParentWordInfo>>()).ToList();
        }
    }
}
