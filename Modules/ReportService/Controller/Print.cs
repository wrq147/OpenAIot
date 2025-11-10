using AuthService.Controller;
using Common.Share;
using Common;
using ReportService.Business;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace ReportService.Controller
{
    public class Print : AbstractLoginedController
    {
        private PrintBLL _printBLL;
        public Print(PrintBLL printBLL)
        {
            _printBLL = printBLL;
        }
        /// <summary>
        /// 查询打印模板
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<MZ_PrintTemplate>>))]
        public async Task<AjaxResult> List(In_PrintList query)
        {
            return this.Success(await _printBLL.ListPage(query));
        }
        /// <summary>
        /// 获取指定打印模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<MZ_PrintTemplate>))]
        public async Task<AjaxResult> Info(string id)
        {
            return (await _printBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取打印数据源列表
        /// </summary>
        /// <returns></returns>
        [About("/ReportService/Print/List")]
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<MZ_PrintData>>))]
        public async Task<AjaxResult> DataList()
        {
            return (await _printBLL.DataList()).ToAjaxResult();
        }

        /// <summary>
        /// 获取打印数据源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<MZ_PrintData>))]
        public async Task<AjaxResult> DataInfo(string id)
        {
            return (await _printBLL.DataInfo(id)).ToAjaxResult();
        }

        /// <summary>
        /// 添加打印模板
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About]
        public async Task<AjaxResult> Add(MZ_PrintTemplate data)
        {
            return (await _printBLL.AddTemplate(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑打印模板
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About]
        public async Task<AjaxResult> Edit(MZ_PrintTemplate data)
        {
            return (await _printBLL.UpdateTemplate(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除打印模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _printBLL.Remove(id)).ToAjaxResult();
        }

    }
}
