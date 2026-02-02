using AuthService.Controller;
using Common.Share;
using Common;
using ProducerService.Business;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace ProducerService.Controller
{
    /// <summary>
    /// 供应商API
    /// </summary>
    public class Supplier : AbstractLoginedController
    {
        private SupplierBLL _supplierBLL;
        public Supplier(SupplierBLL supplierBLL)
        {
            _supplierBLL = supplierBLL;
        }
        /// <summary>
        /// 供应商列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<MZ_Supplier>>> List(In_SupplierList query)
        {
            return this.Success(await _supplierBLL.SelectList(query, GetUser()));
        }
        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/ProducerService/Supplier/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Supplier data)
        {
            return (await _supplierBLL.Add(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/ProducerService/Supplier/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_Supplier data)
        {
            return (await _supplierBLL.Edit(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/ProducerService/Supplier/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _supplierBLL.Delete(id, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Supplier>> Info(string id)
        {
            return (await _supplierBLL.Info(id)).ToAjaxResult();
        }
    }
}
