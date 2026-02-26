using ChannelUtility;
using ChannelUtility.Message;
using Common;
using Common.EventBus;
using IoTAIService.AICode;
using IoTAIService.AIProject;
using IoTAIService.Business;
using IoTAIService.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MonitorService.Business;
using MonitorService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace IoTAIService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "IoTRulesService", "DeveloperService" };


        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            var cs = config.GetSection("IoTAIService");
            services.Configure<IoTAIOption>(cs);

            services.AddDAL<AiMemDAL>();
            services.AddDAL<AiHouseDAL>();
            services.AddSingleton<MilvusBLL>();
            services.AddBLL<AiMemBLL>();
            services.AddBLL<AINodeBLL>();

            services.AddSingleton<FaceDetOnnxRunner>();
            services.AddSingleton<FaceRecogRunner>();
            services.AddSingleton<FaceSTNRunner>();
            services.AddSingleton<FaceKeyPointsRunner>();
            services.AddSingleton<AICache>();
            services.AddSingleton<AIProjectManager>();
            services.AddSingleton<PythonExe>();
            services.AddSingleton<AIBusProxy>();
            services.AddSingleton<AIRedisHelper>();
        }
        private ITAServiceProvider _provider;
        protected override async void Configure(ITAApplication app, PluginObject plg)
        {
            _provider = app.ServiceProvider;
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                var aiOption = app.ServiceProvider.GetService<IOptions<IoTAIOption>>();
                if (aiOption.Value.InitMilvus == true)
                {
                    var milvusBLL = app.ServiceProvider.GetService<MilvusBLL>();
                    await milvusBLL.CreateMemberCollection();
                }

                if (Constants.General.quick_init != true)
                {
                    //添加定时检测节点心跳
                    string heartjobname = "AINodeHeartCheck";
                    string heartgroup = "SYSTEM";
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    if (!await jobBLL.ExistJob(heartjobname, heartgroup))
                    {
                        MZ_Job devjob = new MZ_Job();
                        devjob.concurrent = "0";
                        devjob.createId = 0;
                        devjob.create_time = DateTime.Now;
                        devjob.updateId = 0;
                        devjob.update_time = DateTime.Now;
                        devjob.cron_expression = "0 * * * * ?";
                        devjob.invoke_target = typeof(AINodeBLL).FullName + ".ExecuteSendHeartbeat()";
                        devjob.job_group = heartgroup;
                        devjob.job_name = heartjobname;
                        devjob.misfire_policy = "2";
                        devjob.status = "0";

                        await jobBLL.InsertJob(devjob);
                    }
                }

                var bus = app.ServiceProvider.GetService<NatsScope>().Bus;
                _ = Task.Run(async () =>
               {
                   string tkey = "device.ai." + aiOption.Value.AINodeName;
                   if (string.IsNullOrEmpty(aiOption.Value.AINodeName))
                   {
                       tkey = "device.ai";
                   }
                   await foreach (var msg in bus.SubscribeAsync(tkey, "AIDeviceG", DefalutNatsJsonSerializer<string>.Default))
                   {
                       try
                       {
                           if (string.IsNullOrEmpty(msg.Data))
                           {
                               var tmpoption = _provider.GetService<IOptions<IoTAIOption>>();
                               var redis = _provider.GetService<GeneralRedisHelper>();
                               await redis.HashSetAsync("AIExeNodes", tmpoption.Value.AINodeName, DateTime.Now.AddSeconds(130).ToString("o"));
                               return;
                           }
                           var rs = System.Text.Json.JsonSerializer.Deserialize<AIDetectRequestMeesage>(msg.Data, JsonMessageSerializerConfig.DefaultOptions);
                           await _provider.GetService<AIProjectManager>().MessageHandler(rs);
                       }
                       catch (Exception ex)
                       {
                           Console.WriteLine(ex.ToString());
                       }
                   }
               });

                var nodesub = await bus.SubscribeCoreAsync("RuleNode.Change", "RuleNode" + MyAccess.Core.StringTool.GetGUID(), DefalutNatsJsonSerializer<string>.Default);
                _ = Task.Run(async () =>
               {
                   await foreach (var msg in nodesub.Msgs.ReadAllAsync())
                   {
                       try
                       {
                           await app.ServiceProvider.GetService<AIBusProxy>().UpdateUpList();
                       }
                       catch { }
                   }
               });

                //初始化AI项目
                await app.ServiceProvider.GetService<AIProjectManager>().Init();
                await app.ServiceProvider.GetService<AIBusProxy>().RegNode();
                await app.ServiceProvider.GetService<AIBusProxy>().UpdateUpList();
            });
        }

     
    }
}
