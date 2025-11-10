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
    public class IotGroup : AbstractLoginedController
    {
        private IotGroupBLL _groupBLL;
        public IotGroup(IotGroupBLL groupBLL)
        {
            _groupBLL = groupBLL;
        }
        /// <summary>
        /// 设备分组树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<MZ_IotGroup>>))]
        public async Task<AjaxResult> ListTree()
        {
            var allcls = await _groupBLL.SelectAllOfOrg(GetUser());
            return this.Success(MZ_IotGroup.BuildTree(allcls));
        }
        /// <summary>
        /// 设备分组选择树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<TreeSelect<string>>>))]
        public async Task<AjaxResult> TreeSelect()
        {
            var allcls = await _groupBLL.SelectAllOfOrg(GetUser());
            List<TreeSelect<string>> tlist = MZ_IotGroup.GroupList2Tree(MZ_IotGroup.BuildTree(allcls));
            return this.Success(tlist);
        }
        /// <summary>
        /// 获取指定分组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<MZ_IotGroup>))]
        public async Task<AjaxResult> Info(string id)
        {
            return this.Success(await _groupBLL.Info(id));
        }
        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> Add(MZ_IotGroup data)
        {
            return (await _groupBLL.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑分组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> Edit(MZ_IotGroup data)
        {
            return (await _groupBLL.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _groupBLL.Remove(id)).ToAjaxResult();
        }
    }
}
