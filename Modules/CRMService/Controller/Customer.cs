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
    /// 客户API接口
    /// </summary>
    public class Customer : AbstractLoginedController
    {
        private CustomerBLL _customerBLL;
        public Customer(CustomerBLL customerBLL)
        {
            _customerBLL = customerBLL;
        }
        /// <summary>
        /// 客户列表（公海）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Customer>>> PubList(In_CustomerList query)
        {
            query.Belong = 1;
            return this.Success(await _customerBLL.SelectList(query));
        }
        /// <summary>
        /// 客户列表（私海）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Customer>>> List(In_CustomerList query)
        {
            query.Belong = 2;
            return this.Success(await _customerBLL.SelectList(query));
        }
        /// <summary>
        /// 客户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Customer>> Info(string id)
        {
            return (await _customerBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加客户（私海）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Customer data)
        {
            return (await _customerBLL.Add(data, false)).ToAjaxResult();
        }
        /// <summary>
        /// 添加客户（公海）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> PubAdd(MZ_Customer data)
        {
            return (await _customerBLL.Add(data, true)).ToAjaxResult();
        }
        /// <summary>
        /// 修改客户（私海）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_Customer data)
        {
            return (await _customerBLL.Edit(data, false)).ToAjaxResult();
        }
        /// <summary>
        /// 修改客户（公海）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> PubEdit(MZ_Customer data)
        {
            return (await _customerBLL.Edit(data, true)).ToAjaxResult();
        }
        /// <summary>
        /// 领取客户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<DefaultAjaxResult<string>> Draw(string id)
        {
            return (await _customerBLL.Draw(id)).ToAjaxResult();
        }
        /// <summary>
        /// 取消邀请客户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancelAgent">是否取消代理权限（默认false)</param>
        /// <returns></returns>
        [HttpGet]
        [About("/CRMService/Agent/AddInvite")]
        public async Task<DefaultAjaxResult<string>> UnBind(string id, bool cancelAgent = false)
        {
            return (await _customerBLL.UnBind(id, cancelAgent)).ToAjaxResult();
        }
        /// <summary>
        /// 退回客户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Return(string id, string reason)
        {
            return (await _customerBLL.Return(id, reason)).ToAjaxResult();
        }
        /// <summary>
        /// 删除客户（私海）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _customerBLL.Delete(id, false)).ToAjaxResult();
        }
        /// <summary>
        /// 删除客户（公海）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> PubRemove(string id)
        {
            return (await _customerBLL.Delete(id, true)).ToAjaxResult();
        }
        /// <summary>
        /// 生成客户编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/CRMService/Customer/List")]
        public async Task<DefaultAjaxResult<string>> GenerateNumber()
        {
            return this.Success(await _customerBLL.GenerateNumber());
        }
    }
}
