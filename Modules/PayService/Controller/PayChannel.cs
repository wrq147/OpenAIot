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
    /// 支付渠道API
    /// </summary>
    public class PayChannel : AbstractLoginedController
    {
        private readonly PayChannelBLL _payChannelBLL;

        public PayChannel(PayChannelBLL payChannelBLL)
        {
            _payChannelBLL = payChannelBLL;
        }

        /// <summary>
        /// 分页查询支付渠道
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>分页结果</returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_PayChannel>>> List(In_PayChannelList query)
        {
            return this.Success(await _payChannelBLL.ListAsync(query, GetUser()));
        }

        /// <summary>
        /// 获取单条支付渠道
        /// </summary>
        /// <param name="id">渠道ID</param>
        /// <returns>支付渠道</returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_PayChannel>> Info(string id)
        {
            return this.Success(await _payChannelBLL.InfoAsync(id));
        }

        /// <summary>
        /// 新增支付渠道
        /// </summary>
        /// <param name="data">渠道信息</param>
        /// <returns>新增结果</returns>
        [About("/PayService/PayChannel/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_PayChannel data)
        {
            // 设置当前企业ID
            data.OrgId = GetUser().OrgId;
            return (await _payChannelBLL.AddAsync(data)).ToAjaxResult();
        }

        /// <summary>
        /// 编辑支付渠道
        /// </summary>
        /// <param name="data">渠道信息</param>
        /// <returns>编辑结果</returns>
        [About("/PayService/PayChannel/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<bool>> Edit(MZ_PayChannel data)
        {
            return (await _payChannelBLL.EditAsync(data)).ToAjaxResult();
        }
    }
}
