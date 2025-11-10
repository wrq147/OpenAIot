using AuthService;
using Common;
using Common.Share;
using StorageService.Model;
using MyAccess.DB;
using System.Threading.Tasks;

namespace StorageService.DAL
{
    public class LeaveStockDAL : BaseRepository<MZ_LeaveStock>
    {

        /// <summary>
        /// 获取出库单列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_LeaveStock>> SelectLeaveByPage(In_LeaveList query, Data_ServerTokenInfo user, DataScope scope)
        {
            string tmpkey = query.Key.SqlLikeFilter();
            return await new SqlBuilder(help).Query<MZ_LeaveStock>()
                .Append(@"select ls.*,g.OrgName as ToName,c.CustomerName,u.RealName as Creater from mz_leave_stock ls left join mz_customer c on ls.CustomerId=c.Id and ls.LeaveMethod=0 left join mz_org g on ls.ToOrgId=g.Id left join mz_admin u on ls.createId=u.Id where ls.OrgId=").AppendParam(user.OrgId)
                .Then(!string.IsNullOrEmpty(tmpkey), sql => sql.Append(" and (ls.StockNumber like ").AppendParam(tmpkey + "%")
                 .Append(" or exists(select ed.StockId from mz_leave_detail ed inner join mz_product_batch p on ed.StockId=ls.Id and ed.TargetId=p.Id where p.BatchName like ").AppendParam("%" + tmpkey + "%").Append(")")
                )
                .Then(!string.IsNullOrEmpty(query.ToCompany), sql => sql.Append(" and (c.CustomerName like ").AppendParam("%" + tmpkey + "%").Append(" or g.OrgName like ").AppendParam("%" + tmpkey + "%").Append(")"))
                .Then(query.LeaveMethod != null, sql => sql.Append(" and ls.LeaveMethod=").AppendParam(query.LeaveMethod))
                .Then(query.Status != null && query.Status > -1, sq => sq.Append(" and ls.Status=").AppendParam(query.Status))
                .Then(!string.IsNullOrEmpty(query.FromHouseId), sq => sq.Append(" and ls.FromHouseId=").AppendParam(query.FromHouseId))
                .Then(query.beginTime != null, sq => sq.Append(" and ls.OutDate >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and ls.OutDate <= ").AppendParam(query.endTime))
                .Then(string.IsNullOrEmpty(query.FromHouseId) && scope != null, sql => sql.Append(" and ls.FromHouseId in (select Id from mz_store_house where OrgId=" + user.OrgId + scope.GenerateFilter("DeptId", "LeaderId") + ")"))
            .GeneratePageObjectAsync(query, "OutDate desc");
        }
        public virtual async Task DeleteNotDetail()
        {
            await new SqlBuilder(help).Append("delete from mz_leave_stock where not EXISTS(select Id from mz_leave_detail where StockId=mz_leave_stock.Id)")
                .DoAsync<DoExecSql>();
        }
    }
}
