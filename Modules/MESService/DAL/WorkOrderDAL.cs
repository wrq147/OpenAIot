using Common;
using Common.Share;
using K4os.Hash.xxHash;
using MESService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MESService.DAL
{
    public class WorkOrderDAL : BaseRepository<MZ_WorkOrder>
    {
        public virtual async Task<PageObject<MZ_WorkOrder>> SelectByPage(In_WorkOrderList query, long orgId)
        {
            Expression<Func<MZ_WorkOrder, bool>> expression = (a) => a.OrgId == orgId;
            if (query.beginTime != null)
            {
                expression = expression.And((a) => a.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a) => a.CreatedOn <= query.endTime);
            }
            if (query.Status != null)
            {
                expression = expression.And((a) => a.Status == query.Status);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_WorkOrder>().Include(x => x.ProdInfo, x => x.ProductId).Where(expression);
            return await tmpSql.GeneratePageObjectAsync(query, "CreatedOn desc");
        }
    }
}
