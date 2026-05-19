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
    /// <summary>
    /// 支付渠道DAL
    /// </summary>
    public class PayChannelDAL : BaseRepository<MZ_PayChannel>
    {
        /// <summary>
        /// 分页查询支付渠道
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageObject<MZ_PayChannel>> ListAsync(In_PayChannelList query)
        {
            Expression<Func<MZ_PayChannel, bool>> expression = (a) => a.OrgId == query.OrgId.Value;
            if (!string.IsNullOrEmpty(query.ChannelLabel))
            {
                expression = expression.And((a) => a.ChannelLabel == query.ChannelLabel);
            }
            SqlBuilder sql = new SqlBuilder(help);
            var tmpSql = sql.Query<MZ_PayChannel>().Where(expression);
            return await tmpSql.GeneratePageObjectAsync(query, "Id desc");
        }
    }
}
