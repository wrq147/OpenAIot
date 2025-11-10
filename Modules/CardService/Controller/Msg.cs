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
    /// 访客消息API
    /// </summary>
    public class Msg : AbstractLoginedController
    {
        private CardMsgBLL _cardMsg;
        public Msg(CardMsgBLL cardMsg)
        {
            _cardMsg = cardMsg;
        }
        /// <summary>
        /// 获取我的浏览记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Record(In_Record_List query)
        {
            return this.Success(await _cardMsg.SelectList(query));
        }
        /// <summary>
        /// 获取受访记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Visited(In_Visited_List query)
        {
            return this.Success(await _cardMsg.SelectVisitedList(query));
        }
        /// <summary>
        /// 访问开始时调用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<AjaxResult> Visit(MZ_Card_Msg data)
        {
            return (await _cardMsg.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 访问结束时调用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> VisitEnd(long id)
        {
            return (await _cardMsg.VisitEnd(id)).ToAjaxResult();
        }
        /// <summary>
        /// 设置已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Readed(long id = 0)
        {
            return (await _cardMsg.Readed(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取未读数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> UnreadAmount()
        {
            return (await _cardMsg.UnreadAmount()).ToAjaxResult();
        }
    }
}
