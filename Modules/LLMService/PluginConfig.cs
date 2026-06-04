using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
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

            services.AddScope<IChatClient>((object[] constructorArguments, ITAServiceProvider provider) => provider.GetService<IAiClientRegistry>().GetDefaultChat());
            services.AddScope<IEmbeddingGenerator<string, Embedding<float>>>((object[] constructorArguments, ITAServiceProvider provider) => provider.GetService<IAiClientRegistry>().GetDefaultEmbed());
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {

        }

    }
}
