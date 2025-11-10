using AuthService.Controller;
using ChannelUtility.Config;
using Common;
using Common.Share;
using IoTService.Business;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
namespace IoTService.Controller
{
    public class IotUpdate : AbstractLoginedController
    {
        private IotUpdateBLL _updateBLL;
        public IotUpdate(IotUpdateBLL updateBLL)
        {
            _updateBLL = updateBLL;
        }
        /// <summary>
        /// 产品升级列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotUpdate/ListPage")]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<Out_UpdateItem>>))]
        public async Task<AjaxResult> ListPage(In_UpdatePage query)
        {
            return this.Success(await _updateBLL.ListPage(query));
        }
        /// <summary>
        /// 设备手动升级
        /// </summary>
        /// <param name="id">设备Id</param>
        /// <returns></returns>
        [About("/IoTService/IotUpdate/ListPage")]
        [HttpPost]
        public async Task<AjaxResult> DeviceUp(string id)
        {
            return (await _updateBLL.ManualUp(id)).ToAjaxResult();
        }
        /// <summary>
        /// 清除升级失败的
        /// </summary>
        /// <returns></returns>
        [Des("清除失败的设备升级")]
        [About("/IoTService/IotUpdate/ListPage")]
        [HttpPost]
        public async Task<AjaxResult> ClearError()
        {
            return (await _updateBLL.ClearErr()).ToAjaxResult();
        }
    }
}
