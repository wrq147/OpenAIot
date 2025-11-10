using Common;
using Common.Share;
using DeveloperService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperService.DAL
{
    public class DeveloperDAL : BaseRepository<MZ_Developer>
    {
        public virtual async Task<PageObject<MZ_Developer>> SelectListPage(In_DeveloperPage query)
        {
            return await new SqlBuilder(help).Query<MZ_Developer>().Append("select d.*, u.RealName as DeveloperName,o.OrgName as DeveloperOrgName from mz_developer d left join mz_admin u on u.Id = d.UserId left join mz_org o on d.OrgId=o.Id and d.UserType=1 where 1=1")
                .Then(!string.IsNullOrEmpty(query.Key), x =>
                {
                    string likekey = StringHelper.SqlLikeFilter(query.Key);
                    x.Append(" and (u.RealName like ").AppendParam("%" + likekey + "%").Append(" or o.OrgName like ").AppendParam("%" + likekey + "%").Append(")");
                 })
                .GeneratePageObjectAsync(query);
        }
    }
}
