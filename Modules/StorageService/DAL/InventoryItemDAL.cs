using Common;
using Common.Share;
using StorageService.Model;
using MyAccess.DB;
using System.Threading.Tasks;


namespace StorageService.DAL
{
    public class InventoryItemDAL : BaseRepository<MZ_InventoryItem>
    {
        public virtual async Task<int> InsertByHouseId(In_AllItem data)
        {
            var des = new SqlBuilder(help).Append("insert ignore into mz_inventory_item(Id,OrgId,InventoryId,HouseId,TargetType,TargetId,FirstUserId,CheckUserId) select UUID(),OrgId,").AppendParam(data.InventId).Append(",").AppendParam(data.HouseId)
                .Append(",TargetType,TargetId,0,0 from mz_stockpile_v where HouseId=").AppendParam(data.HouseId)
                .Then(!string.IsNullOrEmpty(data.Key), sql => sql.Append(" and (Name like ").AppendParam("%" + data.Key + "%").Append(" or DeviceNumber like ").AppendParam(data.Key + "%").Append(")"));
            return (await des.DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<Out_InventoryItem> SelectItemByNumber(string id, string number)
        {
            var des = new SqlBuilder(help).Append("select a.*,b.DeviceNumber,b.Name from mz_inventory_item a left join mz_stockpile_v b on a.HouseId=b.HouseId and a.TargetId=b.TargetId where b.DeviceNumber=").AppendParam(number).Append(" and a.InventoryId=").AppendParam(id);
            return (await des.DoAsync<DoQuerySql<Out_InventoryItem>>()).ToFirst();
        }
        public virtual async Task<int> InitSnapQuantity(string id)
        {
            var des = new SqlBuilder(help).Append("update mz_inventory_item set SnapQuantity=(select Quantity from mz_stock_pile where HouseId=mz_inventory_item.HouseId and TargetId=mz_inventory_item.TargetId) where InventoryId=").AppendParam(id);
            return (await des.DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<PageObject<Out_InventoryItem>> SelectItemList(In_InventoryItem data)
        {
            return await new SqlBuilder(help).Query<Out_InventoryItem>().Append("select a.*,b.DeviceNumber,b.Name,b.PhotoUrl,b.Unit,b.Quantity,b.LockQuantity from mz_inventory_item a left join mz_stockpile_v b on a.HouseId=b.HouseId and a.TargetId=b.TargetId where a.InventoryId=").AppendParam(data.Id)
                .Then(data.OnlyRevise == true, x => x.Append(" and ((a.FirstUserId=0 and a.CheckUserId=0) or (a.SnapQuantity<>a.FirstCount or a.SnapQuantity<>a.CheckCount))"))
                .Then(data.UnFirst == true, x => x.Append(" and a.FirstUserId=0"))
                .Then(data.UnCheck == true, x => x.Append(" and a.CheckUserId=0"))
                .GeneratePageObjectAsync(data, string.Empty);
        }

    }
}
