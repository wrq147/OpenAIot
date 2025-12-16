using Common;
using IoTService.Business;
using IoTVideoService.Business;
using Microsoft.Extensions.Configuration;
using MonitorService.Business;
using MonitorService.Model;
using System;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.NetCore;
namespace IoTVideoService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "IoTService", "IoTRulesService" };


        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<FixVideoBLL>();
        }

        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                if (Constants.General.quick_init != true)
                {
                    string videojobname = "CollectFixVideo";
                    string videogroup = "SYSTEM";
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    if (!await jobBLL.ExistJob(videojobname, videogroup))
                    {
                        MZ_Job devjob = new MZ_Job();
                        devjob.concurrent = "1";
                        devjob.createId = 0;
                        devjob.create_time = DateTime.Now;
                        devjob.updateId = 0;
                        devjob.update_time = DateTime.Now;
                        devjob.cron_expression = "0 0/1 * * * ?";
                        devjob.invoke_target = typeof(FixVideoBLL).FullName + ".CollectVideo()";
                        devjob.job_group = videogroup;
                        devjob.job_name = videojobname;
                        devjob.misfire_policy = "2";
                        devjob.status = "0";

                        await jobBLL.InsertJob(devjob);
                    }


             

                }
            });
        }
    }
}
