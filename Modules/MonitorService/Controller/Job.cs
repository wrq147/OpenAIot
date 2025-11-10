using AuthService;
using AuthService.Controller;
using Common;
using Common.Share;
using MonitorService.Model;
using System;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Linq;
using MonitorService.Business;
using System.Threading.Tasks;
using TemplateAction.Label;
using System.Collections.Generic;
using MonitorService.Util;

namespace MonitorService.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [About("/MonitorService/Job")]
    public class Job : AbstractLoginedController
    {
        private JobBLL _jobBLL;
        public Job(JobBLL jobBLL)
        {
            _jobBLL = jobBLL;
        }

        /// <summary>
        /// 查询定时任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> List(In_JobList query)
        {
            return this.Success(await _jobBLL.SelectJobList(query));
        }


        ///
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<IResult> Export(In_JobList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_Job> page = await _jobBLL.SelectJobList(query);
                List<MZ_Job> list = page.List;
                Dictionary<string, ParamRenderToExcel<MZ_Job>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_Job>>();
                FiedNames.Add("job_id", new ParamRenderToExcel<MZ_Job>("任务序号"));
                FiedNames.Add("job_name", new ParamRenderToExcel<MZ_Job>("任务名称"));
                FiedNames.Add("job_group", new ParamRenderToExcel<MZ_Job>("任务组名"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_Job>("任务表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 获取定时任务详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _jobBLL.SelectJobById(id));
        }

        /// <summary>
        /// 新增定时任务
        /// </summary>
        /// <param name="SysJob"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Job job)
        {
            job.SetCreateBy(GetUser());
            return (await _jobBLL.InsertJob(job)).ToAjaxResult();
        }



        /// <summary>
        /// 修改定时任务
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Job job)
        {
            job.SetUpdateBy(GetUser());
            return (await _jobBLL.UpdateJob(job)).ToAjaxResult();
        }



        /// <summary>
        /// 定时任务状态修改
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        [HttpPost]
        [About("ChangeStatus")]
        public async Task<AjaxResult> ChangeStatus(long jobId, string status)
        {
            return (await _jobBLL.ChangeStatus(jobId, status)).ToAjaxResult();
        }


        /// <summary>
        /// 定时任务立即执行一次
        /// </summary>
        /// <param name="SysJob"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Run(long id)
        {
            await _jobBLL.Run(id);
            return this.Success<string>();
        }


        /// <summary>
        /// 删除定时任务
        /// </summary>
        /// <param name="Long"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [About()]
        public async Task<AjaxResult> Remove(long[] id)
        {
            await _jobBLL.DeleteJobByIds(id);
            return this.Success<string>();
        }
        /// <summary>
        /// Cron表达式转描述
        /// </summary>
        /// <param name="cron"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> CronToDes(string cron)
        {
            return this.Success(ScheduleUtils.ToChineseDescription(cron));
        }
    }
}
