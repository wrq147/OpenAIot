using AuthService.Controller;
using AuthService.Model;
using Common;
using Common.Share;
using JiebaNet.Segmenter;
using MyAccess.DB;
using NPOI.POIFS.Crypt.Agile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService
{
    public class OrgStyleDAL : BaseRepository<MZ_OrgStyle>
    {
        public virtual async Task<MZ_AppStyle> OrgStyleById(long orgId)
        {
            return (await new SqlBuilder(help).Append("select * from mz_org_style os inner join mz_app_style s on os.StyleId=s.Id where OrgId=").AppendParam(orgId).Append(" and IsUsing=").AppendParam(true)
                .DoAsync<DoQuerySql<MZ_AppStyle>>()).ToFirst();
        }
        public virtual async Task<List<MZ_AppStyle>> OrgStyleList(long orgId)
        {
            return (await new SqlBuilder(help).Append("select * from mz_org_style os inner join mz_app_style s on os.StyleId=s.Id where OrgId=").AppendParam(orgId)
    .DoAsync<DoQuerySql<MZ_AppStyle>>()).ToList();
        }
        public virtual async Task<PageObject<MZ_Org>> StyleOrgList(In_StyleOrgList query)
        {
            if (query.NoExist == true && !string.IsNullOrEmpty(query.StyleId))
            {
                var tsql = new SqlBuilder(help).Query<MZ_Org>().Append("select * from mz_org where del_flag='0' and NOT EXISTS(select 1 from mz_org_style where OrgId=mz_org.Id and StyleId=")
                    .AppendParam(query.StyleId).Append(")")
                    .Then(!string.IsNullOrEmpty(query.Key), x =>
                    {
                        var keys = new JiebaSegmenter().CutForSearch(query.Key);
                        x.Append(" and (").FullSearch("KeyWords", keys);
                        foreach (var k in keys)
                        {
                            x.Append(" or OrgName like ").AppendParam(k + "%");
                        }
                        x.Append(")");
                    });
                return await tsql.GeneratePageObjectAsync(query, "");
            }
            else
            {
                var tsql = new SqlBuilder(help).Query<MZ_Org>().Append("select o.* from mz_org_style s INNER JOIN mz_org o on s.OrgId=o.Id where o.del_flag='0'")
                    .Then(!string.IsNullOrEmpty(query.StyleId), x =>
                    {
                        x.Append(" and s.StyleId=").AppendParam(query.StyleId);
                    }).Then(!string.IsNullOrEmpty(query.Key), x =>
                    {
                        var keys = new JiebaSegmenter().CutForSearch(query.Key);
                        x.Append(" and (").FullSearch("o.KeyWords", keys);
                        foreach (var k in keys)
                        {
                            x.Append(" or o.OrgName like ").AppendParam(k + "%");
                        }
                        x.Append(")");
                    });
                return await tsql.GeneratePageObjectAsync(query, "");
            }

        }
        public virtual async Task<int> ClearUsing(long orgId)
        {
            var dqs = await new SqlBuilder(help).Append("update mz_org_style set IsUsing=0 where OrgId=").AppendParam(orgId).DoAsync<DoExecSql>();
            return dqs.RowCount;
        }
    }
}
