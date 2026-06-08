using Common;
using Common.EventBus;
using CSnakes.Runtime;
using IoTAIService.AICode;
using IoTAIService.AIProject;
using IoTAIService.Business;
using IoTAIService.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
namespace IoTAIService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "IoTRulesService", "DeveloperService" };


        protected override void ConfigureServices(IConfiguration config, TemplateAction.Core.IServiceCollection services)
        {
            var cs = config.GetSection("IoTAIService");
            services.Configure<IoTAIOption>(cs);
            var aiOption = cs.Get<IoTAIOption>();
            services.AddDAL<AiMemDAL>();
            services.AddDAL<AiHouseDAL>();
            services.AddSingleton<MilvusBLL>();
            services.AddBLL<AiMemBLL>();

            services.AddSingleton<FaceRecogRunner>();
            services.AddSingleton<YoloFaceDetectRunner>();
            services.AddSingleton<FaceSTNRunner>();
            services.AddSingleton<FaceKeyPointsRunner>();
            services.AddSingleton<YoloPoseDetectRunner>();
            services.AddSingleton<PoseC3DRunner>();
            services.AddSingleton<MobileCLIP2VisionRunner>();
            services.AddSingleton<OcrDetectRunner>();
            services.AddSingleton<OcrDocOriRunner>();
            services.AddSingleton<OcrRecRunner>();
            services.AddSingleton<AICache>();
            services.AddSingleton<AIProjectManager>();
            services.AddSingleton<PythonExe>();
            services.AddSingleton<AIBusProxy>();
            services.AddSingleton<AIRedisHelper>();
            services.AddSingleton<ReliableAISubscriber>();
            services.AddSingleton<AITaskRuner>();

            services.AddServices(s =>
            {
                var home = Path.Join(Environment.CurrentDirectory, "AIScript");
                string pythonRoot;
                if (string.IsNullOrEmpty(aiOption.PythonRoot))
                {
                    pythonRoot = Path.Join(Environment.CurrentDirectory, "Python");
                }
                else
                {
                    pythonRoot = aiOption.PythonRoot;
                }
                s.WithPython().WithHome(home).FromFolder(pythonRoot, "3.12").WithVirtualEnvironment(Path.Combine(home, ".venv")).WithPipInstaller();
            });
        }
        private ITAServiceProvider _provider;

        protected override async void Configure(ITAApplication app, PluginObject plg)
        {
            _provider = app.ServiceProvider;

            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                await app.ServiceProvider.GetService<MilvusBLL>().CreateMemberCollection();

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

                await app.ServiceProvider.GetService<AIProjectManager>().Init();
                await app.ServiceProvider.GetService<AIBusProxy>().UpdateUpList();

                Console.WriteLine("检测与安装Python环境...");
                app.ServiceProvider.GetService<IPythonEnvironment>();
                Console.WriteLine("完成Python环境检测与安装");
            });
        }


    }
}
