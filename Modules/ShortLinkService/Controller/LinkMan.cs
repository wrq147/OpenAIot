using AuthService.Controller;
using Common;
using Common.Share;
using ShortLinkService.Business;
using ShortLinkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace ShortLinkService.Controller
{
    public class LinkMan : AbstractLoginedController
    {
        private ShortLinkBLL _linkBLL;
        public LinkMan(ShortLinkBLL linkBLL)
        {
            _linkBLL = linkBLL;
        }
        /// <summary>
        /// 短链接管理列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<DefaultAjaxResult<PageObject<MZ_ShortLink>>> List(In_ShortLinkList query)
        {
            return this.Success(await _linkBLL.SelectList(query, GetUser()));
        }

        /// <summary>
        /// 添加短链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [About("/ShortLinkService/LinkMan/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(string url)
        {
            var user = GetUser();
            return (await _linkBLL.Add(url, user.OrgId)).ToAjaxResult();
        }

        /// <summary>
        /// 删除短链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/ShortLinkService/LinkMan/List")]
        [HttpGet]
        public async Task<AjaxResult> Remove(string[] id)
        {
            return (await _linkBLL.Delete(id)).ToAjaxResult();
        }

    }
}
