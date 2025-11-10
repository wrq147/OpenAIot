using Common;
using StorageService.Model;
using MyAccess.DB;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StorageService.DAL
{
    public class LeaveDetailDAL : BaseRepository<MZ_LeaveDetail>
    {
        public virtual async Task<List<MZ_LeaveDetail>> SelectLeaveList(List<string> stockIds)
        {
            return await new SqlBuilder(help).Query<MZ_LeaveDetail>().Append("select ed.*,p.BatchName as TargetName,p.PhotoUrl from mz_leave_detail ed inner join mz_product_batch p on ed.TargetId = p.Id where ed.StockId in (").AppendParam(stockIds)
                .Append(")").ToListAsync();
        }
    }
}
