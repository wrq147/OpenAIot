using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace Common
{
    public static class FileHelperExtensions
    {
        /// <summary>
        /// 添加文件处理服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IServiceCollection AddFileHelper(this IServiceCollection services, IOptions<GeneralOption> option)
        {
            services.AddSingleton<MinioHelper>();
            services.AddSingleton<FileHelper>();

            return services;
        }
    }
}
