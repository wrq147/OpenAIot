using Common;
using Common.Share;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using PayService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace PayService.DAL
{
    public class PayDetailDAL : BaseRepository<MZ_PayDetail>
    {
        /// <summary>
        /// 分页查询支付记录
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageObject<MZ_PayDetail>> ListAsync(In_PayDetailList query)
        {
            Expression<Func<MZ_PayDetail, bool>> expression = (a) => a.OrgId == query.OrgId.Value;
            if (query.Status.HasValue)
            {
                expression = expression.And((a) => a.Status == query.Status);
            }
            if (!string.IsNullOrEmpty(query.OrderId))
            {
                expression = expression.And((a) => a.OrderId == query.OrderId);
            }
            SqlBuilder sql = new SqlBuilder(help);
            var tmpSql = sql.Query<MZ_PayDetail>().Where(expression);
            return await tmpSql.GeneratePageObjectAsync(query, "Id desc");
        }
    }
}
