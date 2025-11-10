using Common;
using MESService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.DAL
{
    public class WorkBomDAL : BaseRepository<MZ_WorkBom>
    {
        public virtual async Task<int> UpdateUsedQuantity(string workOrderId,string operId,decimal num)
        {
            var tmpSql = new SqlBuilder(help).Append("update mz_work_bom set UsedQuantity=Quantity*").AppendParam(num)
                .Append(" where WorkOrderId=").AppendParam(workOrderId).Append(" and OperId=").AppendParam(operId);
            return (await tmpSql.DoAsync<DoExecSql>()).RowCount;
        }
    }
}
