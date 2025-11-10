using AuthService;
using Common;
using Common.Share;
using StorageService.Model;
using MyAccess.DB;
using System.Threading.Tasks;

namespace StorageService.DAL
{
    public class EnterStockDAL : BaseRepository<MZ_EnterStock>
    {

        /// <summary>
        /// 获取入库单列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_EnterStock>> SelectEnterByPage(In_EnterList query, Data_ServerTokenInfo user, DataScope scope)
        {
            string tmpkey = query.Key.SqlLikeFilter();
            return await new SqlBuilder(help).Query<MZ_EnterStock>()
                .Append(@"select ls.*,g.OrgName as FromName,u.RealName as Creater from mz_enter_stock ls left join mz_org g on ls.FromOrgId=g.Id left join mz_admin u on ls.createId=u.Id where ls.OrgId=").AppendParam(user.OrgId)
                .Then(!string.IsNullOrEmpty(tmpkey), sql => sql.Append(" and (ls.StockNumber like ").AppendParam(tmpkey + "%").Append(" or g.OrgName like ").AppendParam("%" + tmpkey + "%")
                .Append(" or exists(select ed.StockId from mz_enter_detail ed inner join mz_product_batch p on ed.StockId=ls.Id and ed.TargetId=p.Id where p.BatchName like ").AppendParam("%" + tmpkey + "%").Append(")"))
                .Then(!string.IsNullOrEmpty(query.FromCompany), sql => sql.Append(" and g.OrgName like ").AppendParam("%" + tmpkey + "%"))
                .Then(query.EnterMethod != null, sql => 
                {
                    if (query.EnterMethod == 21)
                    {
                        sql.Append(" and ls.EnterMethod<>1");
                    }
                    else
                    {
                        sql.Append(" and ls.EnterMethod=").AppendParam(query.EnterMethod);
                    }
                })
                .Then(!string.IsNullOrEmpty(query.ToHouseId), sq => sq.Append(" and ls.ToHouseId=").AppendParam(query.ToHouseId))
                .Then(query.beginTime != null, sq => sq.Append(" and ls.InDate >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and ls.InDate <= ").AppendParam(query.endTime))
                .Then(query.Status != null && query.Status > -1, sq => sq.Append(" and ls.Status=").AppendParam(query.Status))
                .Then(string.IsNullOrEmpty(query.ToHouseId) && scope != null, sql => sql.Append(" and ls.ToHouseId in (select Id from mz_store_house where OrgId=" + user.OrgId + scope.GenerateFilter("DeptId", "LeaderId") + ")"))
            .GeneratePageObjectAsync(query, "InDate desc");
        }
        public virtual async Task DeleteNotDetail()
        {
            await new SqlBuilder(help).Append("delete from mz_enter_stock where not EXISTS(select Id from mz_enter_detail where StockId=mz_enter_stock.Id)")
                .DoAsync<DoExecSql>();
        }
    }
}
