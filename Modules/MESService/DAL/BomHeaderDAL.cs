using Common;
using Common.Share;
using MESService.Model;
using MyAccess.DB;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using MyAccess.DB.Builder.WhereToSql;
using ProducerService.Model;
namespace MESService.DAL
{
    public class BomHeaderDAL : BaseRepository<MZ_BomHeader>
    {
        public virtual async Task<PageObject<MZ_BomHeader>> SelectByPage(In_BomList query, long orgId)
        {
            Expression<Func<MZ_BomHeader, MZ_Product, bool>> expression = (a, b) => a.OrgId == orgId;
            if (query.beginTime != null)
            {
                expression = expression.And((a, b) => a.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a, b) => a.create_time <= query.endTime);
            }
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And((a, b) => b.ProductName.Contains(query.Key));
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_BomHeader>().InnerJoin<MZ_Product>((a, b) => a.ProductId == b.Id).Where(expression);
            return await tmpSql.GeneratePageObjectAsync(query, "a.create_time desc");
        }
    }
}
