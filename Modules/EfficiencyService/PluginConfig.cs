using Common;
using Common.EventBus;
using EfficiencyService.Business;
using EfficiencyService.DAL;
using Microsoft.Extensions.Configuration;
using MonitorService.Business;
using MonitorService.Model;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace EfficiencyService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService", "MonitorService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<FactorBLL>();
            services.AddBLL<PolicyBLL>();
            services.AddBLL<CommonBLL>();
            services.AddBLL<ProductionBLL>();
            services.AddBLL<OrgConfBLL>();

            services.AddDAL<FactorDAL>();
            services.AddDAL<PolicyDAL>();
            services.AddDAL<CommonDAL>();
            services.AddDAL<ProductionDAL>();
            services.AddDAL<OrgConfDAL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            if (Constants.General.quick_init != true)
            {
                TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
                {
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    //定时每日结存采集数据
                    string jhjobname = "CollectEfficDayTask";
                    string jhgroup = "SYSTEM";
                    if (!await jobBLL.ExistJob(jhjobname, jhgroup))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "0";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "2 0 0 * * ?";
                        job.invoke_target = typeof(ProductionBLL).FullName + ".ExecuteEnergy($context)";
                        job.job_group = jhgroup;
                        job.job_name = jhjobname;
                        job.misfire_policy = "1";
                        job.status = "0";

                        await jobBLL.InsertJob(job);
                    }

                });
            }

            plg.RegisterQuartzTask();
        }
    }
}
