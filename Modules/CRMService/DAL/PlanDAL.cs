using AuthService;
using Common;
using Common.Share;
using CRMService.Model;
using MyAccess.DB;
using System.Threading.Tasks;

namespace CRMService.DAL
{
    public class PlanDAL : BaseRepository<MZ_FollowPlan>
    {
        public virtual async Task<PageObject<MZ_FollowPlan>> SelectByPage(In_FollowPlanList query, DataScope scope, Data_ServerTokenInfo user)
        {
            return await new SqlBuilder(help).Query<MZ_FollowPlan>().Append("select p.*,c.CustomerName,c.CustomerType from mz_follow_plan p left join mz_customer c on p.CustomerId=c.Id where p.OrgId=").AppendParam(user.OrgId)
                .Then(query.beginTime != null, sq => sq.Append(" and p.create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and p.create_time <= ").AppendParam(query.endTime))
                .Then(query.PlanStartTime != null, sq => sq.Append(" and p.PlanTime >= ").AppendParam(query.PlanStartTime))
                .Then(query.PlanEndTime != null, sq => sq.Append(" and p.PlanTime <= ").AppendParam(query.PlanEndTime))
                .Then(!string.IsNullOrEmpty(query.Status), sq =>
                {
                    sq.Append(" and p.Status=").AppendParam(query.Status);
                })
                .Then(!string.IsNullOrEmpty(query.Key), sq =>
                 {
                     string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.Key) + "%";
                     sq.Append(" and c.CustomerName like ").AppendParam(tmpkey);
                 })
                .Then(scope != null, sql => sql.Append(scope.GenerateFilter("p.DeptIds", "p.Executor", string.Empty, true)))
            .GeneratePageObjectAsync(query, "p.create_time desc");
        }


    }
}
