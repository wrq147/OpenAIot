using Common;
using LLMService.Business;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace LLMService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            var cs = config.GetSection("LLMService");
            services.Configure<LLMOption>(cs);
            var aiOption = cs.Get<LLMOption>();
            services.AddSingleton<IAiClientRegistry, AiClientRegistry>();
            services.AddBLL<ChatBLL>();
            services.AddSingleton<MemoryRagBLL>();
            services.AddSingleton<InfoRagBLL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                var memoryBLL = app.ServiceProvider.GetService<MemoryRagBLL>();
                await memoryBLL.CreateRagCollection();
            });
        }

    }
}
