using AuthService.Controller;
using Common;
using Common.Share;
using MessageService.Business;
using MessageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace MessageService.Controller
{
    public class Message : AbstractLoginedController
    {
        private MessageBLL _messageBLL;
        public Message(MessageBLL messageBLL)
        {
            _messageBLL = messageBLL;
        }
        /// <summary>
        /// 获取当前用户的未读消息数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<int>))]
        public async Task<AjaxResult> UnreadCount()
        {
            return (await _messageBLL.UnreadCount()).ToAjaxResult();
        }
        /// <summary>
        /// 全部已读
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<int>))]
        public async Task<AjaxResult> ReadAll()
        {
            return (await _messageBLL.ReadAll()).ToAjaxResult();
        }
        /// <summary>
        /// 设置指定消息已读
        /// </summary>
        /// <param name="id">站内消息Id</param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<int>))]
        public async Task<AjaxResult> Read(long id)
        {
            return (await _messageBLL.Read(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取前几条未读消息
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<MZ_Message>>))]
        public async Task<AjaxResult> UnReadList(int top)
        {
            return this.Success(await _messageBLL.SelectUnread(top));
        }
        /// <summary>
        /// 获取分页消息列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<MZ_Message>>))]
        public async Task<AjaxResult> ListPage(In_MessageList query)
        {
            return this.Success(await _messageBLL.SelectPage(query));
        }
    }
}
