using AuthService.Model;
using CardService.Model;
using Common;
using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardService.DAL
{
    public class CardOrgDAL : BaseDbSupport
    {
        public virtual async Task<int> UpdateCardOrg(MZ_Card_Org cardOrg)
        {
            return (await new SqlBuilder(help).Update(cardOrg)
         .DoAsync<DoExecSql>()).RowCount;
        }

        public virtual async Task<int> Insert(MZ_Card_Org data)
        {
            var docmd = await new SqlBuilder(help).Insert(data).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }

        public virtual async Task<MZ_Card_Org> SelectByBindId(string bindId)
        {
            return (await new SqlBuilder(help).Append("select * from mz_card_org where BindId=").AppendParam(bindId)
         .DoAsync<DoQuerySql<MZ_Card_Org>>()).ToFirst();
        }
        public virtual async Task<MZ_Card_Org_V> SelectCardOrgById(long id)
        {
            return (await new SqlBuilder(help).Append("select * from mz_card_org_v where Id=").AppendParam(id)
.DoAsync<DoQuerySql<MZ_Card_Org_V>>()).ToFirst();
        }
        public virtual async Task<bool> ExistCardOrg(long id)
        {
            return await new SqlBuilder(help).Query<MZ_Card_Org>().SomeAsync(x => x.OrgId == id);
        }
        public virtual async Task<int> DeleteCardOrg(long id)
        {
            return await new SqlBuilder(help).Delete<MZ_Card_Org>("OrgId=").AppendParam(id).DoAsync();
        }

        /// <summary>
        /// 获取用户的组织列表数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="man"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_User_Org_V>> SelectUserOrgListById(long userId, bool man)
        {
            return (await new SqlBuilder(help).Append("select o.*,uo.UserId,uo.dept_id,uo.post_name,d.dept_name from mz_user_org uo left join mz_org o on uo.OrgId=o.Id left join mz_dept d on uo.dept_id=d.dept_id where o.del_flag='0' and uo.UserId=").AppendParam(userId)
                .Then(man, sq =>
                {
                    sq.Append(" and exists(select 1 from mz_user_role m where m.RoleID=2 and m.UserId=").AppendParam(userId).Append(" and m.OrgId=uo.OrgId)");
                })
                .DoAsync<DoQuerySql<MZ_User_Org_V>>()).ToList();
        }
    }
}
