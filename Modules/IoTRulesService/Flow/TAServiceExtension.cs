using IoTRulesService.Flow.Builder;
using System;
using TemplateAction.Core;

namespace IoTRulesService.Flow
{
    public static class TAServiceExtension
    {
        /// <summary>
        /// 注册规则引擎
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRuleflow(this IServiceCollection services)
        {
            services.AddTransient<RuleExecutorBuilder>();
            services.AddTransient<RuleExecutor>();
            return services;
        }
    }
}
