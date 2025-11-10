using AuthService.Controller;
using CardService.Business;
using CardService.Model;
using Common;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace CardService.Controller
{
    /// <summary>
    /// 名片交换请求API
    /// </summary>
    public class Exchange : AbstractLoginedController
    {
        private CardExchangeBLL _cardExchange;
        public Exchange(CardExchangeBLL cardExchange)
        {
            _cardExchange = cardExchange;
        }
        /// <summary>
        /// 递名片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Add(long id)
        {
            return (await _cardExchange.Add(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取指定名片的交换信息
        /// </summary>
        /// <param name="tuid">目标用户Id</param>
        /// <param name="tcid">目标名片Id</param>
        /// <param name="ucid">登录用户名片Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long tuid, long tcid, long ucid)
        {
            return this.Success(await _cardExchange.SelectExchangeInfo(tuid, tcid, ucid));
        }
        /// <summary>
        /// 获取名片交换请求列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> List(In_CardExchange query)
        {

            return this.Success(await _cardExchange.SelectList(query));
        }
        /// <summary>
        /// 待处理数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> PendingCount()
        {
            return this.Success(await _cardExchange.SelectPendingCount(GetUser().UserId));
        }
        /// <summary>
        /// 同意
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Agree(long id)
        {
            return this.Success(await _cardExchange.Agree(id, GetUser()));
        }
        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Refuse(long id)
        {
            return this.Success(await _cardExchange.Refuse(id, GetUser()));
        }
    }
}
