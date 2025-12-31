using AuthService.Fields;
using Common;
using Common.Share;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using ProducerService.Model;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace ProducerService.DAL
{
    public class SupplierDAL : BaseRepository<MZ_Supplier>
    {
        public virtual async Task<PageObject<MZ_Supplier>> SelectByPage(In_SupplierList query, long orgId)
        {
            Expression<Func<MZ_Supplier, bool>> expression = a => a.OrgId == orgId;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(a => a.Number.Contains(query.Key) || a.SupplierName.Contains(query.Key) || a.FullName.Contains(query.Key));
            }
            if (!string.IsNullOrEmpty(query.ContactName))
            {
                expression = expression.And(a => a.ContactName.Contains(query.ContactName));
            }
            if (!string.IsNullOrEmpty(query.Tel))
            {
                expression = expression.And(a => a.Tel.Contains(query.Tel));
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                expression = expression.And(a => a.Status == query.Status);
            }
            if (query.beginTime != null)
            {
                expression = expression.And(a => a.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(a => a.create_time <= query.endTime);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_Supplier>().Where(expression);
            FieldUtility.AppendFilter(tmpSql, query.Items, "Id");
            return await tmpSql.GeneratePageObjectAsync(query, "create_time desc");
        }
    }
}
