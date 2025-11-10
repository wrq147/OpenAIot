using AuthService.Controller;
using Common;
using Common.Share;
using MESService.Business;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace MESService.Controller
{
    /// <summary>
    /// 工艺路线API
    /// </summary>
    public class Route : AbstractLoginedController
    {
        private RouteBLL _routeBLL;
        public Route(RouteBLL routeBLL)
        {
            _routeBLL = routeBLL;
        }
        /// <summary>
        /// 工艺路线列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_ProductRoute>>> List(In_RouteList query)
        {
            return this.Success(await _routeBLL.SelectList(query, GetUser()));
        }
        /// <summary>
        /// 获取工艺路线信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_ProductRoute>> Info(string id)
        {
            return (await _routeBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加工艺路线
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Route/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_ProductRoute data)
        {
            return (await _routeBLL.Add(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 修改工艺路线
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Route/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_ProductRoute data)
        {
            return (await _routeBLL.Edit(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除工艺路线
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/MESService/Route/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _routeBLL.Delete(id, GetUser())).ToAjaxResult();
        }
    }
}
