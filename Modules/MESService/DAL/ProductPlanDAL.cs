using Common;
using Common.Share;
using MESService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MESService.DAL
{
    public class ProductPlanDAL : BaseRepository<MZ_ProductPlan>
    {
        public virtual async Task<PageObject<MZ_ProductPlan>> SelectByPage(In_PlanList query, long orgId)
        {
            Expression<Func<MZ_ProductPlan, bool>> expression = (a) => a.OrgId == orgId;
            if (query.beginTime != null)
            {
                expression = expression.And((a) => a.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a) => a.create_time <= query.endTime);
            }

            var tmpSql = new SqlBuilder(help).Query<MZ_ProductPlan>().Where(expression);
            return await tmpSql.GeneratePageObjectAsync(query, "create_time desc");
        }
        public virtual async Task<int> StartPlane(string planeId)
        {
            var tmpsql = await new SqlBuilder(help).Append("update mz_product_plan set Status=3 where Id=").AppendParam(planeId).Append(" and Status=2").DoAsync<DoQueryScalar>();
            return tmpsql.GetValueInt();
        }
        public virtual async Task<int> FinishPlane(string planeId)
        {
            var tmpsql = await new SqlBuilder(help).Append("update mz_product_plan set Status=4 where Id=").AppendParam(planeId)
                .Append(" and not exists(select 1 from mz_product_plan_item i left join mz_work_order w on i.Id=w.PlanId where i.PlanId=").AppendParam(planeId).Append(" and w.Status<>2)").DoAsync<DoQueryScalar>();
            return tmpsql.GetValueInt();
        }
        public virtual async Task<int> CancelPlane(string planeId)
        {
            var tmpsql = await new SqlBuilder(help).Append("update mz_product_plan set Status=5 where Id=").AppendParam(planeId).Append(" and Status<>4 and Status<>5").DoAsync<DoQueryScalar>();
            return tmpsql.GetValueInt();
        }
    }
}
