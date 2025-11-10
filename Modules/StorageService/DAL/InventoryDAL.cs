using AuthService;
using Common;
using Common.Share;
using StorageService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;

namespace StorageService.DAL
{
    public class InventoryDAL : BaseRepository<MZ_Inventory>
    {
        public virtual async Task<PageObject<MZ_Inventory>> SelectByPage(In_InventoryList query, Data_ServerTokenInfo user, bool isTask)
        {
            Expression<Func<MZ_Inventory, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.Name))
            {
                expression = expression.And(x => x.Name.Contains(query.Name));
            }
            string tasksql = "";
            if (isTask)
            {
                tasksql = " and ((a.Status=2 and exists(select UserId from mz_inventory_user where InventoryId=a.Id and TimeIn=0 and UserId=" + user.UserId + "))";
                tasksql += " or (a.Status=3 and exists(select UserId from mz_inventory_user where InventoryId=a.Id and TimeIn=1 and UserId=" + user.UserId + ")))";
            }
            else
            {
                if (query.Status != null)
                {
                    expression = expression.And(x => x.Status == query.Status);
                }
            }


            if (query.beginTime != null)
            {
                expression = expression.And(x => x.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.create_time <= query.endTime);
            }
            return await new SqlBuilder(help).Query<MZ_Inventory>().Include(x => x.House, x => x.HouseId)
                .Where(expression).Append(tasksql).GeneratePageObjectAsync(query, string.Empty);
        }
    }
}
