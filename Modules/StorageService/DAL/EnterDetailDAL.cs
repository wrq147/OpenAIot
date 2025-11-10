using Common;
using Common.Share;
using StorageService.Model;
using MyAccess.DB;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace StorageService.DAL
{
    public class EnterDetailDAL : BaseRepository<MZ_EnterDetail>
    {
        public virtual async Task<List<MZ_EnterDetail>> SelectEnterList(List<string> stockIds)
        {
            return await new SqlBuilder(help).Query<MZ_EnterDetail>().Append("select ed.*,p.BatchName as TargetName,p.Number as TargetNumber,p.PhotoUrl from mz_enter_detail ed inner join mz_product_batch p on ed.TargetId = p.Id where ed.StockId in (").AppendParam(stockIds)
                .Append(")").ToListAsync();
        }
    }
}
