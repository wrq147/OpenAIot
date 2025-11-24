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
    public class ProductBatch : AbstractLoginedController
    {
        private ProductBatchBLL _batchBLL;
        public ProductBatch(ProductBatchBLL batchBLL)
        {
            _batchBLL = batchBLL;
        }
        /// <summary>
        /// 获取产品批次列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<V_ProductBatch>>> List(In_ProductBatchList query)
        {
            return this.Success(await _batchBLL.SelectByPage(query, GetUser()));
        }
        /// <summary>
        /// 添加产品批次
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Add(In_AddProductBatch data)
        {
            return (await _batchBLL.Insert(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 从设备列表中批量导入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> AddList(In_AddBatchList data)
        {
            return (await _batchBLL.InsertList(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 从设备列表中同步设备
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> SyncFromDevice()
        {
            await _batchBLL.SyncFromDevice(GetUser());
            return this.Success<int>();
        }
        /// <summary>
        /// 获取导入用设备列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<T_IotDevice>>> DevicePage(In_NoExistDevParam query)
        {
            return this.Success(await _batchBLL.NoExistDevPage(query, GetUser()));
        }

        /// <summary>
        /// 编辑产品批次
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(In_EditProductBatch data)
        {
            return (await _batchBLL.Edit(data, GetUser())).ToAjaxResult();
        }


        /// <summary>
        /// 删除产品批次
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _batchBLL.Remove(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 批量删除产品批次
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RemoveList(string[] ids)
        {
            return (await _batchBLL.RemoveList(ids, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 生成批次编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> GenerateNumber()
        {
            return this.Success(await _batchBLL.GenerateNumber(GetUser()));
        }
    }
}
