using AuthService.Controller;
using Common;
using Common.Share;
using AfterService.Business;
using AfterService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using Quartz.Impl.AdoJobStore.Common;

namespace AfterService.Controller
{
    /// <summary>
    /// 房间管理
    /// </summary>
    public class Room : AbstractLoginedController
    {
        private RoomBLL _roomBLL;
        public Room(RoomBLL roomBLL)
        {
            _roomBLL = roomBLL;
        }
        /// <summary>
        /// 获取房间列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_Room>>> List(In_RoomList query)
        {
            var user = GetUser();
            var scope = await user.GetScope(this.ServiceProvider, "/AfterService/Room/List");
            var tlist = await _roomBLL.QueryList(query, user, scope);
            return this.Success(tlist);
        }
        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About()]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Room data)
        {
            return (await _roomBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改房间
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About()]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_Room data)
        {
            return (await _roomBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About()]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _roomBLL.Delete(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Room>> Info(string id)
        {
            return (await _roomBLL.Info(id)).ToAjaxResult();
        }
    }
}
