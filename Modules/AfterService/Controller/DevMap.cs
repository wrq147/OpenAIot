using AfterService.Business;
using AfterService.Model;
using AuthService.Controller;
using Common;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using MyAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace AfterService.Controller
{
    /// <summary>
    /// 设备的地图分布API
    /// </summary>
    public class DevMap : AbstractLoginedController
    {
        private KFDeviceBLL _deviceBLL;
        public DevMap(KFDeviceBLL deviceBLL)
        {
            _deviceBLL = deviceBLL;
        }

        /// <summary>
        /// 获取设备分布的统计信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="dstates">设备运行状态统计：维修、保养等（多个逗号分隔）</param>
        /// <returns></returns>
        [HttpGet]
        [About("/AfterService/DevMap/List")]
        public async Task<DefaultAjaxResult<Dictionary<string, int>>> Info(string code, string dstates = "")
        {
            var user = GetUser();
            var scope = await user.GetScope(this.ServiceProvider, "/AfterService/Room/List");
            return this.Success(await _deviceBLL.SelectKFDevAreaInfo(code, dstates, user, scope));
        }
        /// <summary>
        /// 获取设备分布数量列表
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [About()]
        public async Task<DefaultAjaxResult<List<Out_DevAreaData>>> List(string code = "100000")
        {
            var user = GetUser();
            var scope = await user.GetScope(this.ServiceProvider, "/AfterService/Room/List");
            return this.Success(await _deviceBLL.SelectAreaDataList(code, user, scope));
        }
        /// <summary>
        /// 通过经纬度获取指定范围内的区域设备数量
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/AfterService/DevMap/List")]
        public async Task<DefaultAjaxResult<List<Out_DevAreaData>>> RangeAreaList(In_DevRangeAreaList query)
        {
            var user = GetUser();
            var scope = await user.GetScope(this.ServiceProvider, "/AfterService/Room/List");
            return this.Success(await _deviceBLL.SelectKFDevAreaDataListByRange(query, user, scope));
        }
        /// <summary>
        /// 通过经纬度获取指定范围内的设备
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/AfterService/DevMap/List")]
        public async Task<DefaultAjaxResult<List<MZ_IotDevice>>> RangeList(In_DevRangeList query)
        {
            var user = GetUser();
            var scope = await user.GetScope(this.ServiceProvider, "/AfterService/Room/List");
            return this.Success(await _deviceBLL.SelectKFDeviceListByRange(query, user, scope));
        }
    }
}
