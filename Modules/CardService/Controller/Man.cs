using AuthService.Controller;
using CardService.Business;
using Common;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace CardService.Controller
{
    /// <summary>
    /// 企业管理员设置API
    /// </summary>
    public class Man : AbstractLoginedController
    {
        private CardManBLL _cardMan;
        public Man(CardManBLL cardMan)
        {
            _cardMan = cardMan;
        }
        /// <summary>
        /// 管理员列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> List()
        {
            return this.Success(await _cardMan.Select(GetUser()));
        }
        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Add(long id)
        {
            return (await _cardMan.Insert(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _cardMan.Delete(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 判断是否有指定企业的管理员权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Exist(long id)
        {
            return (await _cardMan.ExistMan(id)).ToAjaxResult();
        }
    }
}
