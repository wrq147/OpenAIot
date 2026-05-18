using Common;
using MyAccess.DB;
using PayService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.DAL
{
    public class WalletDAL : BaseRepository<MZ_Wallet>
    {
   
        /// <summary>
        /// 更新钱包余额（增量）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="amount">变动金额（正数增加，负数减少）</param>
        /// <returns>受影响行数</returns>
        public virtual async Task<int> UpdateBalanceAsync(long userId, decimal amount)
        {
            SqlBuilder sql = new SqlBuilder(help);
            return await sql.UpdateColumns<MZ_Wallet>()
                .SetColum(a => a.TotalBalance, a => a.TotalBalance + amount)
                .SetColum(a => a.UpdatedOn, a => DateTime.Now)
                .Where(a => a.Id == userId)
                .DoAsync();
        }
    }
}
