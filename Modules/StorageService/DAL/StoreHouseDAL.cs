using AuthService;
using Common;
using Common.Share;
using StorageService.Model;
using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StorageService.DAL
{
    public class StoreHouseDAL : BaseRepository<MZ_StoreHouse>
    {
        public virtual async Task<PageObject<MZ_StoreHouse>> SelectByPage(In_HouseList query, DataScope scope, Data_ServerTokenInfo user)
        {

            return await new SqlBuilder(help).Query<MZ_StoreHouse>().Append("select sh.*,ad.RealName as LeaderName from mz_store_house sh left join mz_admin ad on sh.LeaderId=ad.Id where sh.del_flag='0' and sh.OrgId=").AppendParam(user.OrgId)
                .Then(!string.IsNullOrEmpty(query.Status), sq => sq.Append(" and sh.Status=").AppendParam(query.Status))
                .Then(!string.IsNullOrEmpty(query.Name), sq => sq.Append(" and sh.StoreName like ").AppendParam("%" + query.Name.SqlLikeFilter() + "%"))
                .Then(query.IsSystem == true, sq => sq.Append(" and IsSystem=1"))
                .Then(scope != null, sql => sql.Append(scope.GenerateFilter("DeptId", "LeaderId")))
            .GeneratePageObjectAsync(query);
        }
        public virtual async Task<Dictionary<string, MZ_StoreHouse>> SelectDict(List<string> idList)
        {
            Dictionary<string, MZ_StoreHouse> dict = new Dictionary<string, MZ_StoreHouse>();
            var tlist = await new SqlBuilder(help).Query<MZ_StoreHouse>().Where(x => idList.Contains(x.Id)).ToListAsync();
            foreach (var t in tlist)
            {
                if (!dict.ContainsKey(t.Id))
                {
                    dict.Add(t.Id, t);
                }
            }
            return dict;
        }
    }
}
