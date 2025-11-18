using AuthService.Controller;
using Common;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace IoTService.Controller
{
    /// <summary>
    /// 物联分类接口
    /// </summary>
    public class IotClass : AbstractLoginedController
    {
        private IotClassBLL _classBLL;
        public IotClass(IotClassBLL classBLL)
        {
            _classBLL = classBLL;
        }

        /// <summary>
        /// 分类树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<MZ_IotClass>>))]
        public async Task<AjaxResult> ListTree()
        {
            var allcls = await _classBLL.SelectAllClass();
            return this.Success(MZ_IotClass.BuildTree(allcls));
        }
        /// <summary>
        /// 获取指定分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<MZ_IotClass>))]
        public async Task<AjaxResult> Info(string id)
        {
            return this.Success(await _classBLL.Info(id));
        }
        /// <summary>
        /// 分类排序
        /// </summary>
        /// <param name="ids">排序Id列表</param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotClass/ListTree")]
        public async Task<AjaxResult> Sort(List<string> ids)
        {
            return (await _classBLL.UpdateSort(ids)).ToAjaxResult();
        }
        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotClass/ListTree")]
        [Des("添加一条协议分类记录")]
        public async Task<AjaxResult> Add(MZ_IotClass data)
        {
            return (await _classBLL.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑分类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotClass/ListTree")]
        public async Task<AjaxResult> Edit(MZ_IotClass data)
        {
            return (await _classBLL.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotClass/ListTree")]
        [Des("删除一条协议分类记录")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _classBLL.Remove(id)).ToAjaxResult();
        }
    }
}
