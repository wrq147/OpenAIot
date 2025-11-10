
using Common.Share;
using MonitorService.DAL;
using MonitorService.Model;
using MonitorService.Util;
using Quartz;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace MonitorService.Business
{
    public class JobBLL
    {
        private JobDAL _job;
        private ISchedulerFactory _schedulerFactory;
        public JobBLL(JobDAL job, ISchedulerFactory schedulerFactory)
        {
            _job = job;
            _schedulerFactory = schedulerFactory;
        }
        /// <summary>
        /// 判断是否存在指定名称任务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<bool> ExistJob(string name, string group)
        {
            return await _job.ExistJob(name, group);
        }

        /// <summary>
        /// 获取quartz调度器的计划任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<MZ_Job>> SelectJobList(In_JobList query)
        {
            PageObject<MZ_Job> jobPage = await _job.SelectJobList(query);
            foreach (MZ_Job job in jobPage.List)
            {
                JobKey jobKey = ScheduleUtils.GetJobKey(job.job_id.Value, job.job_group);
                var scheduler = await _schedulerFactory.GetScheduler();
                if (await scheduler.CheckExists(jobKey))
                    job.nextValidTime = ScheduleUtils.GetNextExecution(job.cron_expression);
            }
            return jobPage;
        }
        /// <summary>
        /// 通过调度任务名称查询调度信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<MZ_Job> SelectJobByName(string name, string group)
        {
            MZ_Job job = await _job.SelectJobByName(name, group);
            job.nextValidTime = ScheduleUtils.GetNextExecution(job.cron_expression);
            return job;
        }
        /// <summary>
        /// 通过调度任务ID查询调度信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MZ_Job> SelectJobById(long id)
        {
            MZ_Job job = await _job.SelectJobById(id);
            job.nextValidTime = ScheduleUtils.GetNextExecution(job.cron_expression);
            return job;
        }


        /// <summary>
        /// 新增任务
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task<BusResponse<long>> InsertJob(MZ_Job job)
        {
            if (!CronExpression.IsValidExpression(job.cron_expression))
            {
                return BusResponse<long>.Error(12, "新增任务'" + job.job_name + "'失败，Cron表达式不正确");
            }
            if (ScheduleUtils.GetNextExecution(job.cron_expression) == null)
            {
                return BusResponse<long>.Error(12, "任务的执行时间错误,任务无法被触发");
            }
            long rows = await _job.InsertJob(job);
            if (rows > 0)
            {
                job.job_id = rows;
                try
                {
                    var scheduler = await _schedulerFactory.GetScheduler();
                    await ScheduleUtils.CreateScheduleJob(scheduler, job);
                }
                catch (Exception ex)
                {
                    await _job.DeleteJobById(job.job_id.Value);
                    throw;
                }
            }
            return BusResponse<long>.Success(rows);
        }


        /// <summary>
        /// 更新任务的时间表达式
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task<BusResponse<int>> UpdateJob(MZ_Job job)
        {
            MZ_Job properties = await _job.SelectJobById(job.job_id.Value);
            int rows = await _job.UpdateJob(job);
            if (rows > 0)
            {
                await UpdateSchedulerJob(job, properties.job_group);
            }
            return BusResponse<int>.Success(rows);
        }


        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="job"></param>
        /// <param name="jobGroup"></param>
        public async Task UpdateSchedulerJob(MZ_Job job, string jobGroup)
        {
            // 判断是否存在
            JobKey jobKey = ScheduleUtils.GetJobKey(job.job_id.Value, jobGroup);
            var scheduler = await _schedulerFactory.GetScheduler();
            if (await scheduler.CheckExists(jobKey))
            {
                // 防止创建时存在数据问题 先移除，然后在执行创建操作
                await scheduler.DeleteJob(jobKey);
            }
            await ScheduleUtils.CreateScheduleJob(scheduler, job);
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task<int> PauseJob(MZ_Job job)
        {
            job.status = "1";
            int rows = await _job.UpdateJob(job);
            if (rows > 0)
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                await scheduler.PauseJob(ScheduleUtils.GetJobKey(job.job_id.Value, job.job_group));
            }
            return rows;
        }


        /// <summary>
        /// 恢复任务
        /// </summary>
        public async Task<int> ResumeJob(MZ_Job job)
        {
            job.status = "0";
            int rows = await _job.UpdateJob(job);
            if (rows > 0)
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                await scheduler.ResumeJob(ScheduleUtils.GetJobKey(job.job_id.Value, job.job_group));
            }
            return rows;
        }


        /// <summary>
        /// 任务调度状态修改
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<BusResponse<int>> ChangeStatus(long jobId, string status)
        {
            int rows = 0;
            MZ_Job old = await _job.SelectJobById(jobId);
            if (status == "0")
            {
                rows = await ResumeJob(old);
            }
            else if (status == "1")
            {
                rows = await PauseJob(old);
            }
            return BusResponse<int>.Success(rows);
        }



        /// <summary>
        /// 立即运行任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task Run(long jobId)
        {
            MZ_Job properties = await SelectJobById(jobId);
            await RunJob(properties);
        }
        public async Task RunJob(MZ_Job properties)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            // 参数
            JobDataMap dataMap = new JobDataMap();
            dataMap.Put(ScheduleUtils.TASK_PROPERTIES, Newtonsoft.Json.JsonConvert.SerializeObject(properties));

            await scheduler.TriggerJob(ScheduleUtils.GetJobKey(properties.job_id.Value, properties.job_group), dataMap);
        }

        /// <summary>
        /// 批量删除调度信息
        /// </summary>
        /// <param name="jobIds"></param>
        public async Task DeleteJobByIds(long[] jobIds)
        {
            foreach (long jobId in jobIds)
            {
                await DeleteJob(jobId);
            }
        }

        /// <summary>
        /// 删除任务后，所对应的trigger也将被删除
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task<int> DeleteJob(long id)
        {
            MZ_Job job = await _job.SelectJobById(id);
            int rows = await _job.DeleteJobById(id);
            if (rows > 0)
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                await scheduler.DeleteJob(ScheduleUtils.GetJobKey(job.job_id.Value, job.job_group));
            }
            return rows;
        }

        public virtual async Task ResumeExecute()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var terrjobs = await _job.SelectErrorJobs();
            foreach (var terr in terrjobs)
            {
                string tjobid = terr.Replace("TASK_CLASS_NAME", string.Empty);
                var oldJob = await _job.SelectJobById(Convert.ToInt64(tjobid));
                if (oldJob != null)
                {
                    JobKey jobKey = ScheduleUtils.GetJobKey(oldJob.job_id.Value, oldJob.job_group);
                    await scheduler.ResumeJob(jobKey);
                }
            }
        }
    }
}
