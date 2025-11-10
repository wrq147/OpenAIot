using AuthService.Controller;
using Common.Share;
using Common;
using CRMService.Business;
using CRMService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using TemplateAction.NetCore;

namespace CRMService.Controller
{
    /// <summary>
    /// 跟进记录API
    /// </summary>
    public class Follow : AbstractLoginedController
    {
        private FollowBLL _followBLL;
        public Follow(FollowBLL follow)
        {
            _followBLL = follow;
        }
        /// <summary>
        /// 跟进列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Follow>>> List(In_FollowList query)
        {
            return this.Success(await _followBLL.SelectList(query));
        }
        /// <summary>
        /// 获取跟进记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Follow>> Info(string id)
        {
            return (await _followBLL.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 添加跟进
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Follow data)
        {
            return (await _followBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改跟进
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_Follow data)
        {
            return (await _followBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除跟进
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _followBLL.Delete(id)).ToAjaxResult();
        }
    }
}
