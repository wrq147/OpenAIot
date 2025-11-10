using AuthService;
using Common;
using Common.Share;
using StorageService.Model;
using MyAccess.DB;
using IoTService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ProducerService.Model;

namespace StorageService.DAL
{
    public class StockPileDAL : BaseRepository<MZ_StockPile>
    {
        public virtual async Task<List<Out_StockPie>> SelectPilesFromParentOrg(List<string> targets, long orgId)
        {
            return (await new SqlBuilder(help).Query<Out_StockPie>().Append("select * from mz_stockpile_v where TargetId in (").AppendParam(targets).Append(") and EXISTS(select 1 from mz_agent where OrgId=" + orgId + " and ParentOrgId=mz_stockpile_v.OrgId)")
                .DoAsync<DoQuerySql<Out_StockPie>>()).ToList();
        }
        public virtual async Task<PageObject<V_ProductBatch>> SelectNoPile(In_EnterDevList query, Data_ServerTokenInfo user)
        {
            return await new SqlBuilder(help).Query<V_ProductBatch>().Append("select d.* from mz_product_batch_v d where d.OrgId=").AppendParam(user.OrgId)
                .Append(" and (d.ProductLabel='U' or not exists(select * from mz_stock_pile where TargetId=d.Id and (Quantity>0 or LockQuantity>0)))")
            .Then(!string.IsNullOrEmpty(query.Key), sql =>
            {
                var tmpkey = query.Key.SqlLikeFilter();
                sql.Append(" and (d.BatchName like ").AppendParam("%" + tmpkey + "%")
                .Append(" or d.Number like ").AppendParam("%" + tmpkey + "%")
                .Append(" or d.LNumber like ").AppendParam("%" + tmpkey + "%")
                .Append(" or d.ProductName like ").AppendParam("%" + tmpkey + "%")
                .Append(" or d.SkuNumber like ").AppendParam("%" + tmpkey + "%")
                .Append(")")
                ;
            })
            .GeneratePageObjectAsync(query, "d.Id desc");
        }
        public virtual async Task<PageObject<Out_StockPie>> SelectByPage(In_PileList query, Data_ServerTokenInfo user, DataScope scope)
        {
            string tmpkey = query.Key.SqlLikeFilter();
            string filterWarn = "";
            if (query.IsWarn == null)
            {
                filterWarn = " and (Quantity>0 or LockQuantity>0)";
            }
            else
            {
                if (query.IsWarn == true)
                {
                    filterWarn = " and MinNum<>-1 and MaxNum<>-1";
                }
                else
                {
                    filterWarn = " and MinNum=-1 and MaxNum=-1";
                }
            }
            return await new SqlBuilder(help).Query<Out_StockPie>().Append("select sp.*,h.StoreName from mz_stockpile_v sp left join mz_store_house h on sp.HouseId=h.Id where sp.OrgId=").AppendParam(user.OrgId).Append(filterWarn)
                .Then(!string.IsNullOrEmpty(query.InventId), sq => sq.Append(" and NOT EXISTS(select Id from mz_inventory_item where InventoryId=").AppendParam(query.InventId).Append(" and TargetId=sp.TargetId)"))
                .Then(query.TargetType != null, sq => sq.Append(" and sp.TargetType=").AppendParam(query.TargetType))
                .Then(!string.IsNullOrEmpty(query.HouseId), sq => sq.Append(" and sp.HouseId=").AppendParam(query.HouseId))
                .Then(!string.IsNullOrEmpty(tmpkey), sql => sql.Append(" and (sp.Name like ").AppendParam("%" + tmpkey + "%").Append(" or sp.DeviceNumber like ").AppendParam(tmpkey + "%").Append(")"))
                .Then(string.IsNullOrEmpty(query.HouseId) && scope != null, sql => sql.Append(" and sp.HouseId in (select Id from mz_store_house where OrgId=" + user.OrgId + scope.GenerateFilter("DeptId", "LeaderId") + ")"))
            .GeneratePageObjectAsync(query, string.Empty);
        }
        public virtual async Task<int> LockPile(string houseId, int t, string id, decimal num)
        {
            return (await new SqlBuilder(help).Append("update mz_stock_pile set Quantity=Quantity-").AppendParam(num).Append(",LockQuantity=LockQuantity+").AppendParam(num)
                .Append(" where HouseId=").AppendParam(houseId).Append(" and TargetType=").AppendParam(t).Append(" and TargetId=").AppendParam(id)
                .Append(" and Quantity>=").AppendParam(num)
                .DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> LockBack(string houseId, int t, string id, decimal num)
        {
            return (await new SqlBuilder(help).Append("update mz_stock_pile set Quantity=Quantity+").AppendParam(num).Append(",LockQuantity=LockQuantity-").AppendParam(num)
                .Append(" where HouseId=").AppendParam(houseId).Append(" and TargetType=").AppendParam(t).Append(" and TargetId=").AppendParam(id)
                .DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> ReducePile(string houseId, int t, string id, decimal num, decimal? price)
        {
            return (await new SqlBuilder(help).Append("update mz_stock_pile set Quantity=Quantity-").AppendParam(num)
.Append(" where HouseId=").AppendParam(houseId).Append(" and TargetType=").AppendParam(t).Append(" and TargetId=").AppendParam(id)
.Append(" and Quantity>=").AppendParam(num)
.DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> IncreaseLock(MZ_StockPile pile)
        {
            pile.MinNum = -1;
            pile.MaxNum = -1;
            pile.IsTrigger = false;
            return (await new SqlBuilder(help).Insert(pile).Append(" ON DUPLICATE KEY UPDATE Price=(Price*(LockQuantity+Quantity)+").AppendParam(pile.Price * pile.LockQuantity).Append(")/(LockQuantity+Quantity+").AppendParam(pile.LockQuantity).Append("),LockQuantity=LockQuantity+").AppendParam(pile.LockQuantity)
                .DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> IncreasePile(MZ_StockPile pile)
        {
            pile.MinNum = -1;
            pile.MaxNum = -1;
            pile.IsTrigger = false;
            return (await new SqlBuilder(help).Insert(pile).Append(" ON DUPLICATE KEY UPDATE Price=(Price*(LockQuantity+Quantity)+").AppendParam(pile.Price * pile.Quantity).Append(")/(LockQuantity+Quantity+").AppendParam(pile.Quantity).Append("),Quantity=Quantity+").AppendParam(pile.Quantity)
                .DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> ReduceLock(string houseId, int t, string id, decimal num, decimal? price)
        {
            return (await new SqlBuilder(help).Append("update mz_stock_pile set LockQuantity=LockQuantity-").AppendParam(num)
    .Append(" where HouseId=").AppendParam(houseId).Append(" and TargetType=").AppendParam(t).Append(" and TargetId=").AppendParam(id)
.Append(" and LockQuantity>=").AppendParam(num)
    .DoAsync<DoExecSql>()).RowCount;
        }


        public virtual async Task<Out_StockStatistics> SelectStockStatistics(Data_ServerTokenInfo user, DataScope scope)
        {
            var sqlbuilder = new SqlBuilder(help).Append("select SUM(CASE WHEN TargetType = 0 THEN Quantity ELSE 0 END) AS PartsCount,SUM(CASE WHEN TargetType = 1 THEN Quantity ELSE 0 END) AS DevCount from mz_stock_pile where OrgId=").AppendParam(user.OrgId)
                .Then(scope != null, sql => sql.Append(" and HouseId in (select Id from mz_store_house where OrgId=" + user.OrgId + scope.GenerateFilter("DeptId", "LeaderId") + ")"));

            return (await sqlbuilder.DoAsync<DoQuerySql<Out_StockStatistics>>()).ToFirst();
        }
        public virtual async Task<Out_StockStatistics> SelectTodayStockStatistics(Data_ServerTokenInfo user, DataScope scope)
        {
            var sqlbuilder = new SqlBuilder(help).Append("select SUM(CASE WHEN TargetType=0 and (FormType = 0 or FormType=2) THEN Quantity ELSE 0 END) AS TodayPartsOutCount,SUM(CASE WHEN TargetType=1 and (FormType = 0 or FormType=2) THEN Quantity ELSE 0 END) AS TodayDevOutCount,SUM(CASE WHEN TargetType=0 and (FormType = 1 or FormType=3) THEN Quantity ELSE 0 END) AS TodayPartsInCount,SUM(CASE WHEN TargetType=1 and (FormType = 1 or FormType=3) THEN Quantity ELSE 0 END) AS TodayDevInCount from mz_stock_record where OrgId=").AppendParam(user.OrgId)
                .Append(" and CreatedOn>=").AppendParam(DateTime.Today)
    .Then(scope != null, sql => sql.Append(" and HouseId in (select Id from mz_store_house where OrgId=" + user.OrgId + scope.GenerateFilter("DeptId", "LeaderId") + ")"));

            return (await sqlbuilder.DoAsync<DoQuerySql<Out_StockStatistics>>()).ToFirst();
        }

        public virtual async Task<Out_Item> SelectItemByKey(string key, long orgId)
        {
            var sqlbuilder = new SqlBuilder(help).Append("select sp.*,p.BatchName as Name,p.PhotoUrl,p.Number as DeviceNumber,p.Unit from mz_stock_pile sp inner join mz_product_batch_v p on sp.TargetId=p.Id where (p.Number=").AppendParam(key).Append(" or p.LNumber=").AppendParam(key).Append(") and sp.OrgId=").AppendParam(orgId);
            return (await sqlbuilder.DoAsync<DoQuerySql<Out_Item>>()).ToFirst();
        }
    }
}
