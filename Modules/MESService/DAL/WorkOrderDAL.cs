using Common;
using Common.Share;
using K4os.Hash.xxHash;
using MESService.Model;
using MyAccess.Aop;
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
            Expression<Func<MZ_WorkOrder, bool>> expression = (a) => a.OrgId == orgId;
            if (query.beginTime != null)
            {
                expression = expression.And((a) => a.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a) => a.CreatedOn <= query.endTime);
            }
            if (query.Status != null)
            {
                expression = expression.And((a) => a.Status == query.Status);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_WorkOrder>().Include(x => x.ProdInfo, x => x.ProductId).Include(x => x.PlanInfo, x => x.PlanId).Where(expression);
            return await tmpSql.GeneratePageObjectAsync(query, "CreatedOn desc");
        }
        public virtual async Task<List<Out_ParentWordInfo>> SelectListByIds(List<string> ids)
        {
            var tmpSql = new SqlBuilder(help).Append("select a.Id,a.WorkNumber,a.ProductId,a.Status,a.Priority,a.PlannedStartOn,a.PlannedEndOn,b.ProductName,c.PlanName from mz_work_order a left join mz_product b on a.ProductId=b.Id left join mz_product_plan c on a.PlanId=c.Id");
            return (await tmpSql.DoAsync<DoQuerySql<Out_ParentWordInfo>>()).ToList();
        }
        [Trans]
        public virtual async Task<decimal> IncreaseProgress(string orderId, decimal addval)
        {
            var rawBatchCount = await new SqlBuilder(help).Append("select BatchCount from mz_work_order where Id=").AppendParam(orderId).DoAsync<DoQuerySql<decimal>>();
            await new SqlBuilder(help).Append("update mz_work_order set BatchCount=BatchCount+").AppendParam(addval).Append(" where Id=").AppendParam(orderId).DoAsync<DoExecSql>();
            return rawBatchCount.ToFirst();
        }
    }
}
