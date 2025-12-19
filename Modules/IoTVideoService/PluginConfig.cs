using ChannelUtility.Message;
using Common;
using Common.EventBus;
using IoTRulesService.DataParser;
using IoTService;
using IoTVideoService.Business;
using IoTVideoService.DAL;
using Microsoft.Extensions.Configuration;
using MonitorService.Business;
using MonitorService.Model;
using System;
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
            services.AddDAL<VideoSourceDAL>();
            services.Configure<VideoOption>(config.GetSection("IoTVideoService"));
        }
        private ITAServiceProvider _provider;
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            _provider = app.ServiceProvider;
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
                        devjob.concurrent = "0";
                        devjob.createId = 0;
                        devjob.create_time = DateTime.Now;
                        devjob.updateId = 0;
                        devjob.update_time = DateTime.Now;
                        devjob.cron_expression = "0 0/2 * * * ?";
                        devjob.invoke_target = typeof(FixVideoBLL).FullName + ".CollectVideo()";
                        devjob.job_group = videogroup;
                        devjob.job_name = videojobname;
                        devjob.misfire_policy = "3";
                        devjob.status = "0";

                        await jobBLL.InsertJob(devjob);
                    }
                }
            });

            plg.RegisterQuartzTask();

            app.ServiceProvider.GetService<MessageRunner>().OtherMessageListener += MessageHandler;
        }

        private async Task MessageHandler(BaseDeviceMessage msg)
        {
            switch (msg.MsgType)
            {

            }
        }
    }
}
