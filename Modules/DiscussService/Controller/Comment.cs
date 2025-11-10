using AuthService.Controller;
using Common;
using Common.Share;
using DiscussService.Business;
using DiscussService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace DiscussService.Controller
{
    /// <summary>
    /// 评论API
    /// </summary>
    public class Comment : AbstractLoginedController
    {
        private CommentBLL _commentBLL;
        public Comment(CommentBLL commentBLL)
        {
            _commentBLL = commentBLL;
        }
        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Add(In_AddComment data)
        {
            return (await _commentBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 评论列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Comment>>> List(In_CommentList query)
        {
            return this.Success(await _commentBLL.SelectList(query));
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpDelete]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _commentBLL.Delete(id)).ToAjaxResult();
        }
    }
}
