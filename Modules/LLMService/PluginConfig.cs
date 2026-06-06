using Common;
using Common.EventBus;
using LLMService.Business;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using MonitorService.Business;
using MonitorService.Model;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace LLMService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            var cs = config.GetSection("LLMService");
            services.Configure<LLMOption>(cs);
            var aiOption = cs.Get<LLMOption>();
            services.AddSingleton<IAiClientRegistry, AiClientRegistry>();
            services.AddSingleton<ChatBLL>();
            services.AddSingleton<ShortMemoryBLL>();
            services.AddSingleton<MemoryRagBLL>();
            services.AddSingleton<InfoRagBLL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                var memoryBLL = app.ServiceProvider.GetService<MemoryRagBLL>();
                await memoryBLL.CreateRagCollection();
                if (Constants.General.quick_init != true)
                {
                    //添加定时记忆总结
                    string summaryjobname = "MemorySummaryTask";
                    string summarygroup = "SYSTEM";
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    if (!await jobBLL.ExistJob(summaryjobname, summarygroup))
                    {
                        MZ_Job devjob = new MZ_Job();
                        devjob.concurrent = "0";
                        devjob.createId = 0;
                        devjob.create_time = DateTime.Now;
                        devjob.updateId = 0;
                        devjob.update_time = DateTime.Now;
                        devjob.cron_expression = "0 * * * * ?";
                        devjob.invoke_target = typeof(ShortMemoryBLL).FullName + ".ProcessExpiredSessions()";
                        devjob.job_group = summarygroup;
                        devjob.job_name = summaryjobname;
                        devjob.misfire_policy = "3";
                        devjob.status = "0";

                        await jobBLL.InsertJob(devjob);
                    }
                }
            });

            plg.RegisterQuartzTask();


        }

    }
}
