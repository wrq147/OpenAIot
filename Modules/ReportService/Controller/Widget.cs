using AuthService.Controller;
using Common;
using Common.Share;
using Microsoft.Extensions.Logging;
using ReportService.Business;
using ReportService.Models;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace ReportService.Controller
{
    public class Widget : AbstractLoginedController
    {
        private WidgetBLL _widgetBLL;
        public Widget(WidgetBLL widgetBLL)
        {
            _widgetBLL = widgetBLL;
        }


        /// <summary>
        /// 查询组件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(int id)
        {
            return this.Success(await _widgetBLL.GetWidget(id));
        }

        /// <summary>
        /// 新增组件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About]

        public async Task<AjaxResult> Add(MZ_ReportCom data)
        {
            return (await _widgetBLL.AddWidget(data)).ToAjaxResult();
        }



        /// <summary>
        /// 修改组件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About]
        public async Task<AjaxResult> Edit(MZ_ReportCom data)
        {
            return (await _widgetBLL.UpdateWidget(data)).ToAjaxResult();
        }




        /// <summary>
        /// 删除组件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _widgetBLL.Remove(id)).ToAjaxResult();
        }

    }
}
