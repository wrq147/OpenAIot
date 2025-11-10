using AuthService.Controller;
using Common;
using Common.Share;
using MonitorService.Business;
using MonitorService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;

namespace MonitorService.Controller
{
    [About("/MonitorService/OperLog")]
    public class OperLog : AbstractLoginedController
    {
        private OperLogBLL _operLogBLL;
        public OperLog(OperLogBLL operLogBLL)
        {
            _operLogBLL = operLogBLL;
        }
        /// <summary>
        /// 查询所有操作日志
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> List(In_OperLogList query)
        {
            return this.Success(await _operLogBLL.SelectOperLogList(query));
        }
        /// <summary>
        /// 查询本人的操作日志
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> PersonList(In_OperLogList query)
        {
            return this.Success(await _operLogBLL.SelectOperLogListByPerson(query));
        }
        [About]
        [HttpGet]
        public async Task<IResult> Export(In_OperLogList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_OperLog> page = await _operLogBLL.SelectOperLogList(query);
                List<MZ_OperLog> list = page.List;
                Dictionary<string, ParamRenderToExcel<MZ_OperLog>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_OperLog>>();
                FiedNames.Add("oper_id", new ParamRenderToExcel<MZ_OperLog>("日志主键"));
                FiedNames.Add("title", new ParamRenderToExcel<MZ_OperLog>("模块标题"));
                FiedNames.Add("operator_type", new ParamRenderToExcel<MZ_OperLog>("操作类别"));
                FiedNames.Add("method", new ParamRenderToExcel<MZ_OperLog>("方法名称"));
                FiedNames.Add("status", new ParamRenderToExcel<MZ_OperLog>("操作状态", x => x.status == 0 ? "正常" : "异常"));
                FiedNames.Add("oper_time", new ParamRenderToExcel<MZ_OperLog>("操作时间", x => x.oper_time.ToString("yyyy-MM-dd HH:mm:ss")));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_OperLog>("操作日志表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        [HttpGet]
        [About()]
        public async Task<AjaxResult> Remove(long[] id)
        {
            return this.Success(await _operLogBLL.DeleteOperLogByIds(id));
        }
        [HttpGet]
        [About("Remove")]
        public async Task<AjaxResult> Clean()
        {
            return this.Success(await _operLogBLL.CleanOperLog());
        }
    }
}
