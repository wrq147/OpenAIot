using CRMService.Business;
using CRMService.DAL;
using Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using TemplateAction.NetCore;
using Common.EventBus;
using MonitorService.Business;
using MonitorService.Model;
using Microsoft.Extensions.Logging;
using CRMService.Model;
namespace CRMService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        private ILogger<PluginConfig> _log;
        public override string[] DependOn => new string[] { "AuthService", "DeveloperService", "DictService", "DiscussService", "ProducerService", "MonitorService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<CustomerBLL>();
            services.AddBLL<CRMAgentBLL>();
            services.AddBLL<ClueBLL>();
            services.AddBLL<ContactBLL>();
            services.AddBLL<FollowBLL>();
            services.AddBLL<OpportunityBLL>();
            services.AddBLL<PlanBLL>();
            services.AddBLL<CrmConfBLL>();
            services.AddBLL<CRMActionBLL>();
            services.AddBLL<PeriodBLL>();
            services.AddBLL<DiscussEventBLL>();
            services.AddBLL<CrmReportBLL>();


            services.AddDAL<CustomerDAL>();
            services.AddDAL<ClueDAL>();
            services.AddDAL<ContactDAL>();
            services.AddDAL<FollowDAL>();
            services.AddDAL<OpportDetailDAL>();
            services.AddDAL<OpportunityDAL>();
            services.AddDAL<PeriodDAL>();
            services.AddDAL<CRMActionDAL>();
            services.AddDAL<PlanDAL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            var factory = app.ServiceProvider.GetService<ILoggerFactory>();
            _log = factory.CreateLogger<PluginConfig>();

            if (Constants.General.quick_init != true)
            {
                TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
                {
                    //定时将未跟进的客户或线索放入公海
                    string jobname = "FollowToPublicTask";
                    string group = "SYSTEM";
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    if (!await jobBLL.ExistJob(jobname, group))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "1";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "0 0 3 * * ?";
                        job.invoke_target = typeof(CRMActionBLL).FullName + ".NoFollowToPublic()";
                        job.job_group = group;
                        job.job_name = jobname;
                        job.misfire_policy = "2";
                        job.status = "0";

                        await jobBLL.InsertJob(job);
                    }

                });
            }


            plg.RegisterCall("GetCustomerByOrg", async (evt) =>
            {
                var paramdata = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(evt.Params);
                long fromOrgId = Convert.ToInt64(paramdata.from);
                long toOrgId = Convert.ToInt64(paramdata.to);
                var res = await app.ServiceProvider.GetService<CustomerDAL>().SelectCustomerByOrgId(fromOrgId, toOrgId);
                return new CallResponse(res);
            });

            //监听业务事件
            plg.RegisterBus("NewDiscuss", async (bs) =>
            {
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscussEvent>(bs.Params);
                await app.ServiceProvider.GetService<DiscussEventBLL>().DoEvent(evt);
            });

            plg.RegisterBus("JoinBy", async (bs) =>
            {
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<JoinEventData>(bs.Params);
                await app.ServiceProvider.GetService<CRMAgentBLL>().JoinByOtherMod(evt);
            });

        }
        public override void Unload(ITAApplication app, PluginObject plg)
        {
            base.Unload(app, plg);

        }
    }
}
