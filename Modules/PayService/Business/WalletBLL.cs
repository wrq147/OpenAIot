using Common.Share;
using MyAccess.Aop;
using PayService.DAL;
using PayService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.Business
{
    public class WalletBLL
    {
        private readonly WalletDAL _walletDAL;

        public WalletBLL(WalletDAL walletDAL)
        {
            _walletDAL = walletDAL;
        }

        /// <summary>
        /// 获取用户钱包信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>钱包信息</returns>
        public virtual async Task<MZ_Wallet> GetWalletAsync(long userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentNullException(nameof(userId), "用户ID不能为空");
            }
            return await _walletDAL.Select(userId);
        }

        /// <summary>
        /// 充值钱包
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="amount">充值金额</param>
        /// <returns>业务响应</returns>
        [Trans]
        public virtual async Task<BusResponse<bool>> RechargeAsync(IUserInfo user, decimal amount)
        {
            if (user.UserId <= 0)
            {
                return BusResponse<bool>.Error(221, "用户ID不能为空");
            }
            if (amount <= 0)
            {
                return BusResponse<bool>.Error(222, "充值金额必须大于0");
            }

            try
            {

                // 查询钱包是否存在，不存在则创建
                var wallet = await _walletDAL.Select(user.UserId);
                if (wallet == null)
                {
                    wallet = new MZ_Wallet
                    {
                        Id = user.UserId,
                        TotalBalance = amount,
                        CurrencyCode = "CNY",
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now
                    };
                    await _walletDAL.Insert(wallet);
                }
                else
                {
                    await _walletDAL.UpdateBalanceAsync(user.UserId, amount);
                }

                return BusResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return BusResponse<bool>.Error(223, $"充值失败：{ex.Message}");
            }
        }
    }
}
