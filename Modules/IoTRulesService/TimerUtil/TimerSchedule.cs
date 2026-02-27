using Common.Share;
using IoTService;
using Microsoft.Extensions.Options;
using MonitorService.Util;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.TimerUtil
{
    public class TimerSchedule
    {
        private static IScheduler _scheduler;
        public static async Task InitScheduler(ITAServiceProvider provider)
        {
            GeneralOption generalOption = provider.GetService<IOptions<GeneralOption>>().Value;
            IotOption iotOption = provider.GetService<IOptions<IotOption>>().Value;
            string instanceName = "RuleScheduler";
            string instanceId = "Rule_one_" + iotOption.node_name;
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
        public static async Task DeleteJob(long ruleId)
        {
            var jobKey = new JobKey("Rule" + ruleId);
            await _scheduler.DeleteJob(jobKey);
        }

        /// <summary>
        /// 创建定时计划任务
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="cronExps"></param>
        /// <returns></returns>
        public static async Task CreateJob(long ruleId, List<string> cronExps)
        {
            // 构建job信息
            var jobKey = new JobKey("Rule" + ruleId);
            IJobDetail jobDetail = JobBuilder.Create(typeof(TimerConcurrentJob)).WithIdentity(jobKey).Build();
            jobDetail.JobDataMap.Put("PlanId", ruleId);
            int i = 0;
            List<ITrigger> triggers = new List<ITrigger>();
            foreach (var cron in cronExps)
            {
                ITrigger trigger;
                // 表达式调度构建器
                CronScheduleBuilder cronScheduleBuilder = CronScheduleBuilder.CronSchedule(cron).WithMisfireHandlingInstructionFireAndProceed();
                var triggetBuilder = TriggerBuilder.Create().WithIdentity("Rule" + ruleId + "_" + i).ForJob(jobDetail).WithSchedule(cronScheduleBuilder);
                trigger = triggetBuilder.Build();
                triggers.Add(trigger);
                ++i;
            }
            await _scheduler.ScheduleJob(jobDetail, triggers, true);
        }

    }
}
