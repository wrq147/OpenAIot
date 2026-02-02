using AuthService.Controller;
using Common.Share;
using Common;
using ProducerService.Business;
using ProducerService.Model;
using System;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Threading.Tasks;
namespace ProducerService.Controller
{
    /// <summary>
    /// 产品API
    /// </summary>
    public class Product : AbstractLoginedController
    {
        private ProductBLL _productBLL;
        public Product(ProductBLL productBLL)
        {
            _productBLL = productBLL;
        }
        /// <summary>
        /// 产品列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<MZ_Product>>> List(In_ProductList query)
        {
            return this.Success(await _productBLL.SelectList(query, GetUser(), false));
        }
        /// <summary>
        /// 代理商可见产品列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<MZ_Product>>> AgentList(In_ProductList query)
        {
            return this.Success(await _productBLL.SelectList(query, GetUser(), true));
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/ProducerService/Product/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Product data)
        {
            return (await _productBLL.Add(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/ProducerService/Product/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_Product data)
        {
            return (await _productBLL.Edit(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/ProducerService/Product/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _productBLL.Delete(id, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Product>> Info(string id)
        {
            return (await _productBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 生成产品sku编号
        /// </summary>
        /// <returns></returns>
        [About("/ProducerService/Product/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> GenerateNumber()
        {
            return this.Success(await _productBLL.GenerateNumber());
        }

    }
}
