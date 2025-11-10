using Common.Share;
using MonitorService.DAL;
using MonitorService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MonitorService.Business
{
    public class JobLogBLL
    {
        private JobLogDAL _joblog;
        private ITAServiceProvider _provider;
        public JobLogBLL(JobLogDAL joblog, ITAServiceProvider provider)
        {
            _joblog = joblog;
            _provider = provider;
        }

        /// <summary>
        /// 获取quartz调度器日志的计划任务
        /// </summary>
        /// <param name="jobLog"></param>
        /// <returns></returns>
        public async Task<PageObject<MZ_JobLog>> SelectJobLogList(In_JobLogList query)
        {
            return await _joblog.SelectJobLogList(query);
        }

        /// <summary>
        /// 通过调度任务日志ID查询调度信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MZ_JobLog> SelectJobById(long id)
        {
            return await _joblog.SelectJobLogById(id);
        }

        /// <summary>
        /// 新增任务日志
        /// </summary>
        /// <param name="jobLog"></param>
        public async Task AddJobLog(MZ_JobLog jobLog)
        {
            await _joblog.InsertJobLog(jobLog);
        }

        /// <summary>
        /// 批量删除调度日志信息
        /// </summary>
        /// <param name="logIds"></param>
        /// <returns></returns>
        public async Task<int> DeleteJobLogByIds(long[] logIds)
        {
            return await _joblog.DeleteJobLogByIds(logIds);
        }


        /// <summary>
        /// 删除任务日志
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<int> DeleteJobLogById(long jobId)
        {
            var jobinfo = await _provider.GetService<JobDAL>().SelectJobById(jobId);
            if (jobinfo != null)
            {
                return await _joblog.DeleteJobLogByName(jobinfo.job_name, jobinfo.job_group);
            }
            else
            {
                return 0;
            }
        }



        /// <summary>
        /// 清空任务日志
        /// </summary>
        public async Task<int> CleanJobLog(string name, string group)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(group))
            {
                return await _joblog.CleanJobLog();
            }
            else
            {
                return await _joblog.DeleteJobLogByName(name, group);
            }
        }

    }
}
