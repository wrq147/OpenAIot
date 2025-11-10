using AuthService.Controller;
using Common.Share;
using Common;
using ProducerService.Business;
using ProducerService.Model;
using System;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProducerService.Controller
{
    /// <summary>
    /// 产品分组API
    /// </summary>
    public class ProductType : AbstractLoginedController
    {
        private ProductTypeBLL _productTypeBLL;
        public ProductType(ProductTypeBLL productTypeBLL)
        {
            _productTypeBLL = productTypeBLL;
        }
        /// <summary>
        /// 获取产品分组列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_ProductType>>> List()
        {
            return this.Success(await _productTypeBLL.SelectProductTypeList(GetUser()));
        }
        /// <summary>
        /// 获取指定产品分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_ProductType>> Info(string id)
        {
            return this.Success(await _productTypeBLL.Info(id));
        }
        /// <summary>
        /// 产品分组排序
        /// </summary>
        /// <param name="ids">排序Id列表</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Sort(List<string> ids)
        {
            return (await _productTypeBLL.UpdateSort(ids)).ToAjaxResult();
        }
        /// <summary>
        /// 添加产品分组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_ProductType data)
        {
            return (await _productTypeBLL.Insert(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 编辑产品分组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_ProductType data)
        {
            return (await _productTypeBLL.Update(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除产品分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _productTypeBLL.Remove(id, GetUser())).ToAjaxResult();
        }
    }
}
