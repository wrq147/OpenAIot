using AuthService.Controller;
using AfterService.Business;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using AfterService.Model;
using System.Collections.Generic;

namespace AfterService.Controller
{
    public class RoomCategory : AbstractLoginedController
    {
        private RoomCategoryBLL _roomCategoryBLL;
        public RoomCategory(RoomCategoryBLL roomCategoryBLL)
        {
            _roomCategoryBLL = roomCategoryBLL;
        }


        /// <summary>
        /// 房间分类树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ListTree(long orgid)
        {
            var allcls = await _roomCategoryBLL.SelectAll(orgid);
            return this.Success(MZ_RoomCategory.BuildTree(allcls));
        }
        /// <summary>
        /// 获取指定房间分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(string id)
        {
            return this.Success(await _roomCategoryBLL.Info(id));
        }
        /// <summary>
        /// 房间分类排序
        /// </summary>
        /// <param name="ids">排序Id列表</param>
        /// <returns></returns>
        [HttpPost]
        [About("/AfterService/RoomCategory/Man")]
        public async Task<AjaxResult> Sort(List<string> ids)
        {
            return (await _roomCategoryBLL.UpdateSort(ids)).ToAjaxResult();
        }
        /// <summary>
        /// 添加房间分类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/AfterService/RoomCategory/Man")]
        public async Task<AjaxResult> Add(MZ_RoomCategory data)
        {
            return (await _roomCategoryBLL.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑房间分类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/AfterService/RoomCategory/Man")]
        public async Task<AjaxResult> Edit(MZ_RoomCategory data)
        {
            return (await _roomCategoryBLL.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除房间分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/AfterService/RoomCategory/Man")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _roomCategoryBLL.Remove(id)).ToAjaxResult();
        }
    }
}
