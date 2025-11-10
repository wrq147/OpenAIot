using AuthService;
using Common;
using Common.Share;
using CRMService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.DAL
{
    public class FollowDAL : BaseRepository<MZ_Follow>
    {
        public virtual async Task<PageObject<MZ_Follow>> SelectByPage(In_FollowList query, DataScope scope, Data_ServerTokenInfo user)
        {

            return await new SqlBuilder(help).Query<MZ_Follow>().Append(@"select f.*,CASE
	WHEN f.TargetType=0 THEN
		c.CustomerName
	WHEN f.TargetType=1 THEN
		cu.CompanyName
	ELSE
		''
END as TargetName
 from mz_follow f left join mz_customer c on f.TargetType=0 and f.TargetId=c.Id left join mz_clue cu on f.TargetType=1 and f.TargetId=cu.Id where f.del_flag='0' and f.OrgId=").AppendParam(user.OrgId)
                .Then(query.TargetType != null, sq => sq.Append(" and f.TargetType=").AppendParam(query.TargetType))
                .Then(!string.IsNullOrEmpty(query.TargetId), sq => sq.Append(" and f.TargetId=").AppendParam(query.TargetId))
                .Then(query.beginTime != null, sq => sq.Append(" and f.create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and f.create_time <= ").AppendParam(query.endTime))
                .Then(string.IsNullOrEmpty(query.Key), sq =>
                {
                    string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.Key) + "%";
                    sq.Append(" and (c.CustomerName like ").AppendParam(tmpkey).Append(" or cu.CompanyName like ").AppendParam(tmpkey).Append(")");
                })
                .Then(scope != null, sql => sql.Append(scope.GenerateFilter("f.DeptId", "f.FollowUser")))
            .GeneratePageObjectAsync(query, "f.create_time desc");
        }
    }
}
