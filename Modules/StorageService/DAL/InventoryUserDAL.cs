using Common;
using StorageService.Model;
using MyAccess.DB;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace StorageService.DAL
{
    public class InventoryUserDAL : BaseRepository<MZ_InventoryUser>
    {
        public virtual async Task<List<MZ_InventoryUser>> SelectListWithUser(Expression<Func<MZ_InventoryUser, bool>> expression, string orderby = "")
        {
            return await new SqlBuilder(help).Query<MZ_InventoryUser>().Include(x => x.UserInfo, x => x.UserId).Where(expression).Then(!string.IsNullOrEmpty(orderby), x => x.Append(" order by " + orderby)).ToListAsync();
        }
    }
}
