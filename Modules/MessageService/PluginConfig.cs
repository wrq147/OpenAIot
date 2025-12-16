using Common;
using Common.EventBus;
using MessageService.Business;
using MessageService.DAL;
using Microsoft.Extensions.Configuration;
using MonitorService.Business;
using MonitorService.Model;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;
namespace MessageService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<MessageBLL>();
            services.AddBLL<PushBLL>();
            services.AddDAL<MessageDAL>();
            services.AddDAL<MessageLogDAL>();
            services.AddDAL<PushClientDAL>();
            services.AddSingleton<NoticeExecutor>();
            services.AddSingleton<MessageConfig>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            plg.RegisterNotice(async (evt) =>
            {
                await app.ServiceProvider.GetService<NoticeExecutor>().SyncNoticeMessage(evt);
            });

            if (Constants.General.quick_init != true)
            {
                TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
                {
                    var tmpconfig = await app.ServiceProvider.GetService<MessageConfig>().GetJsonConfig();
                    if (!string.IsNullOrEmpty(tmpconfig.push_appid) && !string.IsNullOrEmpty(tmpconfig.push_appkey))
                    {
                        //添加定时获取个推token
                        string devjobname = "RefreshPushToken";
                        string devgroup = "SYSTEM";
                        if (!await app.ServiceProvider.GetService<JobBLL>().ExistJob(devjobname, devgroup))
                        {
                            MZ_Job devjob = new MZ_Job();
                            devjob.concurrent = "0";
                            devjob.createId = 0;
                            devjob.create_time = DateTime.Now;
                            devjob.updateId = 0;
                            devjob.update_time = DateTime.Now;
                            devjob.cron_expression = "0 0 0/1 * * ? ";
                            devjob.invoke_target = typeof(PushBLL).FullName + ".RefreshToken()";
                            devjob.job_group = devgroup;
                            devjob.job_name = devjobname;
                            devjob.misfire_policy = "2";
                            devjob.status = "0";

                            await app.ServiceProvider.GetService<JobBLL>().InsertJob(devjob);
                        }
                    }

                });
            }

            plg.RegisterQuartzTask();
        }
    }
}
