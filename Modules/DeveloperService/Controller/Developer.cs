using AuthService.Controller;
using Common;
using Common.Share;
using DeveloperService.Business;
using DeveloperService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace DeveloperService.Controller
{
    public class Developer : AbstractLoginedController
    {
        private DeveloperBLL _developerBLL;
        public Developer(DeveloperBLL developerBLL)
        {
            _developerBLL = developerBLL;
        }
        /// <summary>
        /// 开发者列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About()]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<MZ_Developer>>))]
        public async Task<AjaxResult> ListPage(In_DeveloperPage query)
        {
            return this.Success(await _developerBLL.ListPage(query));
        }
  
        /// <summary>
        /// 开发者详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/DeveloperService/Developer/ListPage")]
        public async Task<DefaultAjaxResult<MZ_Developer>> Info(string id)
        {
            return (await _developerBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加开发者
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About()]
        public async Task<AjaxResult> Add(MZ_Developer data)
        {
            return (await _developerBLL.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑开发者
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About()]
        public async Task<AjaxResult> Edit(MZ_Developer data)
        {
            return (await _developerBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除开发者
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About()]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _developerBLL.Remove(id)).ToAjaxResult();
        }
    }
}
