using Common.Share;
using IoTService;
using Microsoft.Extensions.Options;
using MonitorService.Util;
using Quartz;
using TemplateAction.Core;

namespace IoTVideoService.PlanUtil
{
    public class PlanSchedule
    {
        private static IScheduler _scheduler;
        public static async Task InitScheduler(ITAServiceProvider provider)
        {
            GeneralOption generalOption = provider.GetService<IOptions<GeneralOption>>().Value;
            IotOption iotOption = provider.GetService<IOptions<IotOption>>().Value;
            string instanceName = "ViScheduler";
            string instanceId = "Vi_one_" + iotOption.node_name;
            var factory = SchedulerBuilder.Create()
             .WithId(instanceId)
             .WithName(instanceName)
             .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
             .WithMisfireThreshold(TimeSpan.FromSeconds(60))
             .UsePersistentStore(x =>
             {
                 x.UseProperties = false;
                 if (generalOption.sqltype == "Sqlite")
                 {
                     x.UseMicrosoftSQLite(generalOption.connstr);
                 }
                 else
                 {
                     x.UseClustering();
                     x.UseMySql(generalOption.connstr);
                 }
                 x.UseSystemTextJsonSerializer();
             })
             .Build();


            _scheduler = await factory.GetScheduler();
            _scheduler.JobFactory = new IOCJobFactory(provider);
            await _scheduler.Start();
        }
        public static async Task DeleteJob(string planId)
        {
            var jobKey = new JobKey("VIDJOB" + planId);
            await _scheduler.DeleteJob(jobKey);
        }
        /// <summary>
        /// 创建定时计划任务
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="planId"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static async Task CreateJob(string planId, List<RecordTriggerTask> tasks)
        {
            // 构建job信息
            var jobKey = new JobKey("VIDJOB" + planId);
            IJobDetail jobDetail = JobBuilder.Create(typeof(PlanConcurrentJob)).WithIdentity(jobKey).Build();
            jobDetail.JobDataMap.Put("PlanId", planId);
            int i = 0;
            List<ITrigger> triggers = new List<ITrigger>();
            foreach (var task in tasks)
            {
                ITrigger trigger;
                // 表达式调度构建器
                CronScheduleBuilder cronScheduleBuilder = CronScheduleBuilder.CronSchedule(task.CronExpression).WithMisfireHandlingInstructionIgnoreMisfires();
                var triggetBuilder = TriggerBuilder.Create().WithIdentity("VIDTRI" + planId + "_" + i).ForJob(jobDetail).WithSchedule(cronScheduleBuilder);
                trigger = triggetBuilder.Build();
                triggers.Add(trigger);
                ++i;
            }
            await _scheduler.ScheduleJob(jobDetail, triggers, true);
        }

    }
}
