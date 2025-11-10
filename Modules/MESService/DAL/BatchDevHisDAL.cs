using Common;
using Common.Share;
using MESService.Model;
using MonitorService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MESService.DAL
{
    public class BatchDevHisDAL : BaseRepository<MZ_BatchDevHis>
    {
        public virtual async Task<PageObject<MZ_BatchDevHis>> SelectByPage(In_BatchDevHisList query, IUserInfo user)
        {

            Expression<Func<MZ_BatchDevHis, MZ_ProductOper, bool>> expression = (a, b) => true;

            if (!string.IsNullOrEmpty(query.BatchNo))
            {
                expression = expression.And((a, b) => a.BatchNo == query.BatchNo);
            }
            else
            {
                expression = expression.And((a, b) => a.OrgId == user.OrgId);
            }

            if (query.beginTime != null)
            {
                expression = expression.And((a, b) => a.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a, b) => a.CreatedOn <= query.endTime);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_BatchDevHis>().LeftJoin<MZ_ProductOper>((a, b) => a.OperId == b.Id).Where(expression);

            return await tmpSql.GeneratePageObjectAsync(query, "a.CreatedOn desc");
        }
    }
}
