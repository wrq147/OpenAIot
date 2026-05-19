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
using TemplateAction.Core;
using TemplateAction.Route;

namespace PayService.Controller
{
    /// <summary>
    /// 支付记录API
    /// </summary>
    public class PayDetail : AbstractLoginedController
    {
        private readonly PayDetailBLL _payDetailBLL;

        public PayDetail(PayDetailBLL payDetailBLL)
        {
            _payDetailBLL = payDetailBLL;
        }

        /// <summary>
        /// 分页查询支付记录
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>分页结果</returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_PayDetail>>> List(In_PayDetailList query)
        {
            return this.Success(await _payDetailBLL.ListAsync(query, GetUser()));
        }

        /// <summary>
        /// 获取单条支付记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns>支付记录</returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_PayDetail>> Info(string id)
        {
            return this.Success(await _payDetailBLL.InfoAsync(id));
        }

        /// <summary>
        /// 新增支付记录
        /// </summary>
        /// <param name="data">支付记录信息</param>
        /// <returns>新增结果</returns>
        [About("/PayService/PayDetail/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_PayDetail data)
        {
            return (await _payDetailBLL.AddAsync(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 更新支付记录状态
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="status">状态</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>更新结果</returns>
        [About("/PayService/PayDetail/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<bool>> UpdateStatus(string id, int status, string errorMsg = "")
        {
            return (await _payDetailBLL.UpdateStatusAsync(id, status, errorMsg)).ToAjaxResult();
        }
    }

}
