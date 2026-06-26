using Common;
using Common.EventBus;
using Common.Share;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MonitorService.Business;
using MonitorService.DAL;
using MonitorService.Model;
using MonitorService.Util;
using Quartz;
using System;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace MonitorService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddTransient<ServerBLL>();
            services.AddTransient<JobBLL>();
            services.AddTransient<JobLogBLL>();
            services.AddTransient<OperLogBLL>();
            services.AddBLL<CalendarBLL>();

            services.AddSingleton<JobDAL>();
            services.AddSingleton<JobLogDAL>();
            services.AddSingleton<OperLogDAL>();
            services.AddDAL<HolidayOrgDAL>();
            services.AddDAL<HolidayTypeDAL>();

            services.AddSingleton<OperLogThread>();
            services.Configure<MonitorOption>(config.GetSection("MonitorService"));

            //添加quartz
            services.AddSingleton<QuartzDisallowConcurrentJob>();
            services.AddSingleton<QuartzJob>();
            services.AddSingleton<ISchedulerFactory>((arguments, provider) =>
            {
                GeneralOption generalOption = provider.GetService<IOptions<GeneralOption>>().Value;
                MonitorOption monitorOption = provider.GetService<IOptions<MonitorOption>>().Value;

                string instanceName = "MonitorScheduler";
                string instanceId = "instance_one";
                if (!string.IsNullOrEmpty(monitorOption.instance_id))
                {
                    instanceId = monitorOption.instance_id;
                }
                var factory = SchedulerBuilder.Create()
                 .WithId(instanceId)
                 .WithName(instanceName)
                 .UseDefaultThreadPool(x => x.MaxConcurrency = 10)
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


                TAAsyncHelper.RunSync(async () =>
                {
                    var sched = await factory.GetScheduler();
                    sched.JobFactory = new IOCJobFactory(provider);
                    await sched.Start();
                    return sched;
                });


                return factory;
            });

            services.AddSingleton<OperLogMiddleware>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            var monitorOption = app.ServiceProvider.GetService<IOptions<MonitorOption>>();
            //添加操作日志记录中间件
            if (monitorOption.Value.log_enable)
            {
                ((TASiteApplication)app).UseMiddleware<OperLogMiddleware>();
            }




            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                if (Constants.General.quick_init != true)
                {
                    //添加定时删除
                    string jobname = "CleanOperLog";
                    string group = "SYSTEM";
                    if (!await jobBLL.ExistJob(jobname, group))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "0";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "0 0 0 1/1 * ?";
                        job.invoke_target = typeof(OperLogBLL).FullName + ".CleanOver()";
                        job.job_group = group;
                        job.job_name = jobname;
                        job.misfire_policy = "2";
                        job.status = "0";

                        await jobBLL.InsertJob(job);
                    }

                    //定时清除临时文件
                    string clearFilejobname = "ClearUpTmpFiles";
                    if (!await jobBLL.ExistJob(clearFilejobname, group))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "0";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "0 0 0 1/1 * ?";
                        job.invoke_target = typeof(FileHelper).FullName + ".ClearUpTmpFiles()";
                        job.job_group = group;
                        job.job_name = clearFilejobname;
                        job.misfire_policy = "2";
                        job.status = "0";

                        await jobBLL.InsertJob(job);
                    }


                    //定时生成假期、删除过期假期
                    string holidayjobname = "HolidayTask";
                    if (!await jobBLL.ExistJob(holidayjobname, group))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "0";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "0 0 3 * * ?";
                        job.invoke_target = typeof(CalendarBLL).FullName + ".Execute()";
                        job.job_group = group;
                        job.job_name = holidayjobname;
                        job.misfire_policy = "2";
                        job.status = "0";

                        await jobBLL.InsertJob(job);
                    }

                    //恢复错误任务
                    string errjobname = "ErrorResumeTask";
                    if (!await jobBLL.ExistJob(errjobname, group))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "0";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "0 0/30 * * * ?";
                        job.invoke_target = typeof(JobBLL).FullName + ".ResumeExecute()";
                        job.job_group = group;
                        job.job_name = errjobname;
                        job.misfire_policy = "0";
                        job.status = "0";

                        await jobBLL.InsertJob(job);
                    }
                }

            });


            plg.RegisterQuartzTask();

        }

    }
}
