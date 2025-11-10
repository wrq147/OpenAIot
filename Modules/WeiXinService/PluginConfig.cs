using System;
using TemplateAction.Core;
using WeiXinService.DAL;
using Common;
using Microsoft.Extensions.Configuration;
using TemplateAction.NetCore;
using System.Threading.Tasks;
using MonitorService.Business;
using MonitorService.Model;
using WeiXinService.Business;
using Common.EventBus;

namespace WeiXinService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService", "MonitorService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddSingleton<WxApiHelper>();
            services.AddBLL<CorpSyncBLL>();
            services.AddBLL<WeiXinBLL>();
            services.AddDAL<WeiXinDAL>();
            services.AddDAL<UserWxDAL>();
            services.AddDAL<UserCropDAL>();
            services.AddDAL<CorpSyncDAL>();
            services.AddDAL<CorpTaskDAL>();
            services.AddSingleton<WxExecutor>();
            services.AddSingleton<CorpWxSyncThread>();
            services.AddSingleton<WeiXinConfig>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            plg.RegisterNotice(async (evt) =>
            {
                await app.ServiceProvider.GetService<WxExecutor>().SyncNoticeMessage(evt);
            });

            if (Constants.General.quick_init != true)
            {
                TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
                {
                    //添加定时刷新微信令牌的自动任务
                    string jobname = "WxRefreshJob";
                    string group = "SYSTEM";
                    if (!await app.ServiceProvider.GetService<JobBLL>().ExistJob(jobname, group))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "1";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "0 0 0/1 * * ?";
                        job.invoke_target = typeof(WxApiHelper).FullName + ".RefreshToken()";
                        job.job_group = group;
                        job.job_name = jobname;
                        job.misfire_policy = "2";
                        job.status = "0";

                        await app.ServiceProvider.GetService<JobBLL>().InsertJob(job);
                    }
                });
            }
        }

    }
}
