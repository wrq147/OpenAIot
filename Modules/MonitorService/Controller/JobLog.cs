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
    [About("/MonitorService/Job")]
    public class JobLog : AbstractLoginedController
    {
        private JobLogBLL _joblogBLL;
        public JobLog(JobLogBLL joblogBLL)
        {
            _joblogBLL = joblogBLL;
        }

        /// <summary>
        /// 查询定时任务调度日志列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> List(In_JobLogList query)
        {
            return this.Success(await _joblogBLL.SelectJobLogList(query));
        }


        /// <summary>
        /// 导出定时任务调度日志列表
        /// </summary>
        /// <param name="sysJobLog"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<IResult> Export(In_JobLogList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_JobLog> page = await _joblogBLL.SelectJobLogList(query);
                List<MZ_JobLog> list = page.List;
                Dictionary<string, ParamRenderToExcel<MZ_JobLog>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_JobLog>>();
                FiedNames.Add("job_log_id", new ParamRenderToExcel<MZ_JobLog>("日志序号"));
                FiedNames.Add("job_name", new ParamRenderToExcel<MZ_JobLog>("任务名称"));
                FiedNames.Add("job_group", new ParamRenderToExcel<MZ_JobLog>("任务组名"));
                FiedNames.Add("job_message", new ParamRenderToExcel<MZ_JobLog>("日志信息"));
                FiedNames.Add("status", new ParamRenderToExcel<MZ_JobLog>("执行状态", x => x.status == "0" ? "正常" : "失败"));
                FiedNames.Add("create_time", new ParamRenderToExcel<MZ_JobLog>("创建时间", x => x.create_time.ToString("yyyy-MM-dd HH:mm:ss")));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_JobLog>("任务日志表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }


        /// <summary>
        /// 根据调度编号获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _joblogBLL.SelectJobById(id));
        }

        [HttpGet]
        [About("Remove")]
        public async Task<AjaxResult> Remove(long[] id)
        {
            return this.Success(await _joblogBLL.DeleteJobLogByIds(id));
        }



        /// <summary>
        /// 清空定时任务调度日志
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        [HttpGet]
        [About("Remove")]
        public async Task<AjaxResult> Clean(string name = "", string group = "")
        {
            return this.Success(await _joblogBLL.CleanJobLog(name, group));
        }
    }
}
