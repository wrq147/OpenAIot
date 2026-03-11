using Common;
using Common.EventBus;
using IoTAIService.AICode;
using IoTAIService.AIProject;
using IoTAIService.Business;
using IoTAIService.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
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

            services.AddSingleton<FaceDetOnnxRunner>();
            services.AddSingleton<FaceRecogRunner>();
            services.AddSingleton<FaceSTNRunner>();
            services.AddSingleton<FaceKeyPointsRunner>();
            services.AddSingleton<YoloWorldDetectRunner>();
            services.AddSingleton<YoloLargeWorldDetectRunner>();
            services.AddSingleton<AICache>();
            services.AddSingleton<AIProjectManager>();
            services.AddSingleton<PythonExe>();
            services.AddSingleton<AIBusProxy>();
            services.AddSingleton<AIRedisHelper>();
            services.AddSingleton<ReliableAISubscriber>();
            services.AddSingleton<AITaskRuner>();
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

                _provider.GetService<ReliableAISubscriber>().StartReceiving();

                var bus = app.ServiceProvider.GetService<NatsScope>().Bus;
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
                app.ServiceProvider.GetService<PythonExe>().Init();
                await app.ServiceProvider.GetService<AIProjectManager>().Init();
                await app.ServiceProvider.GetService<AIBusProxy>().UpdateUpList();
            });
        }

     
    }
}
