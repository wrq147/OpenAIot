using Common;
using Common.Share;
using MESService.Model;
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
    public class RouteDAL : BaseRepository<MZ_ProductRoute>
    {
        public virtual async Task<PageObject<MZ_ProductRoute>> SelectByPage(In_RouteList query, long orgId)
        {
            Expression<Func<MZ_ProductRoute, bool>> expression = (a) => a.OrgId == orgId;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And((a) => a.RouteName.Contains(query.Key));
            }

            if (query.beginTime != null)
            {
                expression = expression.And((a) => a.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a) => a.create_time <= query.endTime);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_ProductRoute>().Where(expression);
            return await tmpSql.GeneratePageObjectAsync(query, "create_time desc");
        }
    }
}
