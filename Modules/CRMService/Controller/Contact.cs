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
    /// 联系人API接口
    /// </summary>
    public class Contact : AbstractLoginedController
    {
        private ContactBLL _contactBLL;
        public Contact(ContactBLL contactBLL)
        {
            _contactBLL = contactBLL;
        }
        /// <summary>
        /// 联系人列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Contact>>> List(In_ContactList query)
        {
            return this.Success(await _contactBLL.SelectList(query));
        }
        /// <summary>
        /// 联系人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Contact>> Info(string id)
        {
            return (await _contactBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加联系人
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Contact data)
        {
            return (await _contactBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改联系人
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_Contact data)
        {
            return (await _contactBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _contactBLL.Delete(id)).ToAjaxResult();
        }
    }
}
