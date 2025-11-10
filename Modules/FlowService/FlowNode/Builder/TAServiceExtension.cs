using Common;
using FlowService.FlowNode.Builder.Step;
using System;
using TemplateAction.Core;

namespace FlowService.FlowNode.Builder
{

    public static class TAServiceExtension
    {
        /// <summary>
        /// 注册工作流
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWorkflow(this IServiceCollection services)
        {
            services.AddBLL<WorkflowExecutor>();
            return services;
        }
    }
}
