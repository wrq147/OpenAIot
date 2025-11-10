using AuthService;
using AuthService.Model;
using Common;
using Common.Share;
using ProducerService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProducerService.DAL
{
    public class AgentDAL : BaseRepository<MZ_Agent>
    {
        public virtual async Task<PageObject<Out_Agent>> SelectByPage(In_AgentList query, IUserInfo factoryUser)
        {
            return await new SqlBuilder(help).Query<Out_Agent>().Append("select a.*,o.OrgName as FactoryName from mz_agent_v a left join mz_org o on a.FactoryId=o.Id where FactoryId=" + factoryUser.OrgId)
                .Then(query.ParentOrgId != null, sq => sq.Append(" and a.ParentOrgId=").AppendParam(query.ParentOrgId))
                .Then(!string.IsNullOrEmpty(query.OrgName), sq => sq.Append(" and a.OrgName like ").AppendParam("%" + query.OrgName.SqlLikeFilter() + "%"))
                .Then(query.beginTime != null, sq => sq.Append(" and a.create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and a.create_time <= ").AppendParam(query.endTime))
            .GeneratePageObjectAsync(query, "a.create_time desc");
        }
        public virtual async Task<OutCertInfo> SelectCertInfo(string id, long orgId)
        {
            return await new SqlBuilder(help).Query<OutCertInfo>().Append("select a.*,o.OrgName as FactoryName,o.Logo,o.AddressName,o.AddressDetail,o.Intro from mz_agent_v a left join mz_org o on a.FactoryId=o.Id where a.Id=").AppendParam(id).Append(" and OrgId=")
                .AppendParam(orgId).ToFirstAsync();
        }
        public virtual async Task<List<Out_AgentFactory>> SelectFactory(long id)
        {
            return await new SqlBuilder(help).Query<Out_AgentFactory>().Append("select o.OrgName as FactoryName,o.Logo,po.OrgName as ParentOrgName,a.* from mz_agent a left join mz_org o on a.FactoryId=o.Id left join mz_org po on a.ParentOrgId=po.Id where OrgId=")
                .AppendParam(id).ToListAsync();
        }


    }
}
