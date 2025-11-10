using Common;
using Common.Share;
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
    public class OperDAL : BaseRepository<MZ_ProductOper>
    {
        public virtual async Task<PageObject<MZ_ProductOper>> SelectByPage(In_OperList query, long orgId)
        {
            Expression<Func<MZ_ProductOper, bool>> expression = (a) => a.OrgId == orgId;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And((a) => a.OperName.Contains(query.Key));
            }
            if (query.beginTime != null)
            {
                expression = expression.And((a) => a.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a) => a.create_time <= query.endTime);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_ProductOper>().Where(expression);
            if (query.Items != null && query.Items.Length > 0)
            {
                //过滤扩展字段
                foreach (var item in query.Items)
                {
                    item.AppendFilter(tmpSql, string.Empty);
                }
            }

            return await tmpSql.GeneratePageObjectAsync(query, "create_time desc");
        }
    }
}
