using Common;
using Microsoft.Extensions.Logging;
using MonitorService.Business;
using MonitorService.DAL;
using MonitorService.Model;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MonitorService.Util
{
    /// <summary>
    /// 抽象quartz调用
    /// </summary>
    public abstract class AbstractQuartzJob : IJob
    {
        private ITAServiceProvider _provider;
        public ITAServiceProvider Provider { get { return _provider; } }
        private ILogger<AbstractQuartzJob> _log;
        private static ThreadLocal<DateTime> _threadLocal = new ThreadLocal<DateTime>();
        public AbstractQuartzJob(ITAServiceProvider provider)
        {
            _provider = provider;
            _log = _provider.GetService<ILoggerFactory>().CreateLogger<AbstractQuartzJob>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            string objstr = context.JobDetail.JobDataMap.GetString(ScheduleUtils.TASK_PROPERTIES);
            MZ_Job job = Newtonsoft.Json.JsonConvert.DeserializeObject<MZ_Job>(objstr);
            try
            {
                Before(context, job);
                if (job != null)
                {
                    await DoExecute(context, job);
                }
                await After(context, job, null);
            }
            catch (Exception e)
            {
                await After(context, job, e);
            }
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context"></param>
        /// <param name="job"></param>
        protected void Before(IJobExecutionContext context, MZ_Job job)
        {
            _threadLocal.Value = new DateTime();
        }

        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name="context"></param>
        /// <param name="job"></param>
        /// <param name="e"></param>
        protected async Task After(IJobExecutionContext context, MZ_Job job, Exception e)
        {
            if (context.NextFireTimeUtc == null)
            {
                if (ScheduleUtils.GetNextExecution(job.cron_expression) == null)
                {
                    //任务完结，则清除任务
                    await _provider.GetService<JobDAL>().DeleteJobById(job.job_id.Value);
                    await _provider.GetService<JobLogDAL>().ClearFinsihJobLog(job.job_name);
                    return;
                }
            }


            DateTime startTime = _threadLocal.Value;
            MZ_JobLog sysJobLog = new MZ_JobLog();
            sysJobLog.create_time = DateTime.Now;
            sysJobLog.job_name = job.job_name;
            sysJobLog.job_group = job.job_group;
            sysJobLog.invoke_target = job.invoke_target;
            TimeSpan ts = new DateTime() - startTime;
            sysJobLog.job_message = string.Format("{0} 总共耗时：{1}毫秒", job.job_name, ts.Milliseconds);
            if (e != null)
            {
                sysJobLog.status = "1";
                sysJobLog.exception_info = (e.Message + "栈信息：" + e.StackTrace).Limit(2000);
            }
            else
            {
                sysJobLog.status = "0";
                sysJobLog.exception_info = string.Empty;
            }

            //// 写入数据库当中
            JobLogBLL joblogBLL = _provider.GetService<JobLogBLL>();
            await joblogBLL.AddJobLog(sysJobLog);
        }

        protected abstract Task DoExecute(IJobExecutionContext context, MZ_Job job);
    }
}
