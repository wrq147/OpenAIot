using AuthService.Controller;
using Common.Share;
using AfterService.Business;
using AfterService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using TemplateAction.NetCore;
using IoTService.Models;

namespace AfterService.Controller
{
    /// <summary>
    /// 设备计划
    /// </summary>
    public class DevPlane : AbstractLoginedController
    {
        private DevPlaneBLL _planeBLL;
        public DevPlane(DevPlaneBLL planeBLL)
        {
            _planeBLL = planeBLL;
        }

        /// <summary>
        /// 获取计划列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_PlaneType>>> List(In_DevPlaneType query)
        {
            return this.Success(await _planeBLL.SelectList(query));
        }
        /// <summary>
        /// 获取设备的计划流程列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_PlaneFlowItem>>> DeviceFlowList(string id)
        {
            return this.Success(await _planeBLL.DeviceFlowList(id));
        }

        /// <summary>
        /// 添加计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AfterService/DevPlane/Add")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_PlaneType data)
        {
            return (await _planeBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AfterService/DevPlane/Edit")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_PlaneType data)
        {
            return (await _planeBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/AfterService/DevPlane/Remove")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _planeBLL.Delete(id)).ToAjaxResult();
        }


        /// <summary>
        /// 获取计划信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="showTarget">是否显示目标</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_PlaneType>> Info(string id, bool showTarget = false)
        {
            return (await _planeBLL.Info(id, showTarget)).ToAjaxResult();
        }
        /// <summary>
        /// 获取计划的设备列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotDevice>>> DevList(In_PlaneDevList query)
        {
            return this.Success(await _planeBLL.DevListPage(query));
        }
    }
}
