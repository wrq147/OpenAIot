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
    /// 名片夹API
    /// </summary>
    public class Holder : AbstractLoginedController
    {
        private CardHolderBLL _cardHolder;
        public Holder(CardHolderBLL cardHolder)
        {
            _cardHolder = cardHolder;
        }
        /// <summary>
        /// 获取通讯录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> List(In_Card_Holder query)
        {

            return this.Success(await _cardHolder.SelectList(query));
        }
        /// <summary>
        /// 从通讯录中删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _cardHolder.Remove(id)).ToAjaxResult();
        }

        /// <summary>
        /// 存入通讯录
        /// </summary>
        /// <param name="id">要存入的名片</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Add(long id)
        {
            return (await _cardHolder.Add(id)).ToAjaxResult();
        }
    }
}
