using AuthService.Controller;
using Common;
using Common.Share;
using PayService.Business;
using PayService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;

namespace PayService.Controller
{
    /// <summary>
    /// 钱包API
    /// </summary>
    public class Wallet : AbstractLoginedController
    {
        private readonly WalletBLL _walletBLL;

        public Wallet(WalletBLL walletBLL)
        {
            _walletBLL = walletBLL;
        }

        /// <summary>
        /// 获取当前用户钱包信息
        /// </summary>
        /// <returns>钱包信息</returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Wallet>> GetMyWallet()
        {
            return this.Success(await _walletBLL.GetWalletAsync(GetUser()));
        }

        /// <summary>
        /// 钱包充值
        /// </summary>
        /// <param name="amount">充值金额</param>
        /// <returns>充值结果</returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<bool>> Recharge(decimal amount)
        {
            return (await _walletBLL.RechargeAsync(GetUser(), amount)).ToAjaxResult();
        }
    }
}
