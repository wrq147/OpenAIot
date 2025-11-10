using AuthService.Controller;
using Common.Share;
using AfterService.Business;
using AfterService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace AfterService.Controller
{
    public class RoomDevice : AbstractLoginedController
    {
        private RoomDeviceBLL _roomDeviceBLL;
        public RoomDevice(RoomDeviceBLL roomDeviceBLL)
        {
            _roomDeviceBLL = roomDeviceBLL;
        }


        /// <summary>
        /// 关联设备与房间
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AfterService/Room/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(List<In_RoomDevice> data)
        {
            return (await _roomDeviceBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除设备与房间的关联
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AfterService/Room/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Remove(List<In_RoomDevice> data)
        {
            return (await _roomDeviceBLL.Delete(data)).ToAjaxResult();
        }
    }
}
