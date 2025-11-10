using Microsoft.Extensions.Hosting;
using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public static class TANetConsoleHostBuilderExtensions
    {
        /// <summary>
        /// 扩展IHostBuilder,使用TemplateAction框架
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="rootPath"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        public static IHostBuilder UseTANet(this IHostBuilder builder, string rootPath, Action<TAApplication> init = null)
        {
            return builder.UseServiceProviderFactory(new TANetConsoleServiceProviderFactory(rootPath, init));
        }
        /// <summary>
        /// 默认使用应用程序所在目录
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        public static IHostBuilder UseTANet(this IHostBuilder builder, Action<TAApplication> init = null)
        {
            return UseTANet(builder, AppContext.BaseDirectory, init);
        }
    }
}
