using AuthService.Controller;
using Common.Share;
using Common;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using MESService.Business;

namespace MESService.Controller
{
    /// <summary>
    /// 工序API
    /// </summary>
    public class Oper : AbstractLoginedController
    {
        private OperBLL _operBLL;
        public Oper(OperBLL operBLL)
        {
            _operBLL = operBLL;
        }
        /// <summary>
        /// 工序列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<MZ_ProductOper>>> List(In_OperList query)
        {
            return this.Success(await _operBLL.SelectList(query, GetUser()));
        }
        /// <summary>
        /// 获取工序信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_ProductOper>> Info(string id)
        {
            return (await _operBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取工艺路线明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_ProductRouteOper>> RouteInfo(string id)
        {
            return (await _operBLL.RouteInfo(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加工序
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Oper/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_ProductOper data)
        {
            return (await _operBLL.Add(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 修改工序
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Oper/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_ProductOper data)
        {
            return (await _operBLL.Edit(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除工序
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/MESService/Oper/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _operBLL.Delete(id, GetUser())).ToAjaxResult();
        }

    }
}
