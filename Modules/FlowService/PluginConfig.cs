using Common;
using Common.EventBus;
using Common.Share;
using FlowService.Business;
using FlowService.DAL;
using FlowService.FlowNode.Builder;
using FlowService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace FlowService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService", "MonitorService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddTransient<GroupBLL>();
            services.AddTransient<OrgBLL>();
            services.AddBLL<TaskBLL>();
            services.AddBLL<FlowBLL>();
            services.AddBLL<FlowReportBLL>();


            services.AddSingleton<GroupDAL>();
            services.AddSingleton<OrgDAL>();
            services.AddDAL<FlowTemplateDAL>();
            services.AddDAL<FlowTemplateLinkDAL>();
            services.AddDAL<FormDAL>();
            services.AddDAL<FormDataDAL>();
            services.AddDAL<FlowDAL>();
            services.AddDAL<FlowNodeDAL>();
            services.AddDAL<FlowQueryDAL>();
            services.AddDAL<FlowDeviceDAL>();
            services.AddDAL<FlowTrilogDAL>();
            services.AddSingleton<DeviceRedisHelper>();
            services.AddSingleton<DeviceBusProxy>();
            services.AddWorkflow();

        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                var generalOption = app.ServiceProvider.GetService<IOptions<GeneralOption>>();
                if (!string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
                {
                    Task _ = Task.Run(async () =>
                    {
                        var bus = app.ServiceProvider.GetService<NatsScope>().Bus;
                        await foreach (var msg in bus.SubscribeAsync("/RuleNode.Change", "RuleNode" + MyAccess.Core.StringTool.GetGUID(), DefalutNatsJsonSerializer<string>.Default))
                        {
                            app.ServiceProvider.GetService<DeviceBusProxy>().UpdateUpList();
                        }
                    });
                }
            });
            //监听创建新的流程
            plg.RegisterCall("NewFlowTask", async (bs) =>
            {
                var data = bs.To<In_TaskAdd>();
                if (data == null)
                {
                    return CallResponse.Next();
                }
                var taskBLL = app.ServiceProvider.GetService<TaskBLL>();
                if (data.assign == null)
                {
                    data.assign = new Dictionary<string, List<Out_UserItem>>();
                }
                try
                {
                    var res = await taskBLL.CreateFlow(data.templateId, data.model, data.assign, 2, true, data.UserId, data.flowId);
                    if (res.IsSuccess())
                    {
                        var tmpformdata = res.Data as MZ_FormData;
                        return CallResponse.Success(tmpformdata.flowId);
                    }
                    else
                    {
                        return CallResponse.Error(res.Code, res.Message);
                    }
                }
                catch (Exception ex)
                {
                    return CallResponse.Error(33, ex.Message);
                }
            });


            plg.RegisterQuartzTask();

        }

    }
}
