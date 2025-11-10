
using Common;
using Common.EventBus;
using Common.FluentMigrator;
using Common.IdGenerator;
using Common.Share;
using Common.Swagger;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace App
{
    public class PluginConfig : IPluginConfig
    {
        public string[] DependOn => Array.Empty<string>();
        public void Loaded(ITAApplication app, TemplateAction.Core.IServiceCollection services, PluginObject plg)
        {
            IConfiguration config = app.ServiceProvider.GetService<IConfiguration>();
            services.Configure<GeneralOption>(config.GetSection("General"));
            var generalOption = app.ServiceProvider.GetService<IOptions<GeneralOption>>();
            //存全局GeneralOption配置
            Constants.General = generalOption.Value;

            //添加缓存
            services.AddSingleton<CacheHelper>();
            services.AddSingleton<GeneralRedisHelper>();
            services.AddSingleton<SnowflakeHelper>();
            //添加FileHelper
            services.AddFileHelper(generalOption);
            //添加Excel处理
            services.AddSingleton<ExcelHelper>();


            //添加Swagger
            services.AddTAWebApiExplorer();
            services.AddServices(s =>
            {
                s.AddSwaggerGen(c =>
                {
                    c.SchemaFilter<SwaggerJsonRenameFilter>();
                    c.DocInclusionPredicate((string documentName, ApiDescription apiDescription) => { return true; });
                    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API接口文档", Version = "v1" });

                    //添加注释
                    DirectoryInfo theFolder = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "XML"));
                    foreach (FileInfo nextFile in theFolder.GetFiles())
                    {
                        c.IncludeXmlComments(nextFile.FullName, true);
                    }

                    c.AddSecurityDefinition("系统Api令牌", new OpenApiSecurityScheme
                    {
                        Description = "请在输入框中输入登录令牌",
                        Name = "Authorization",//Token默认的参数名称
                        In = ParameterLocation.Header,//Token默认存放Authorization信息的位置(请求头中)
                        Type = SecuritySchemeType.ApiKey
                    });
                    c.AddSecurityDefinition("开发者Api令牌", new OpenApiSecurityScheme
                    {
                        Description = "请在输入框中输入开发者令牌",
                        Name = "token",//Token默认的参数名称
                        In = ParameterLocation.Header,//Token默认存放Authorization信息的位置(请求头中)
                        Type = SecuritySchemeType.ApiKey
                    });

                    c.OperationFilter<SecuritySchemeOperationFilter>();

                });

            });

            if (Constants.General.quick_init != true)
            {
                //添加FluentMigrator插件
                services.AddServices(s =>
                {
                    s.AddTAFluent();
                    if (generalOption.Value.sqltype == "Sqlite")
                    {
                        s.ConfigureRunner(rb => rb.AddSQLite().WithGlobalConnectionString(generalOption.Value.connstr));
                    }
                    else
                    {
                        s.ConfigureRunner(rb => rb.AddMySql5().WithGlobalConnectionString(generalOption.Value.connstr));
                    }
                });

            }


            //添加IHttpClientFactory
            services.AddServices(s =>
            {
                s.AddHttpClient();
            });


            //添加验证码插件
            services.AddSingleton<CaptchaImageHelper>();


            if (!string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
            {
                //添加Rabbit事件总线
                services.AddSingleton<RabbitScope>();
                TAEventDispatcher.Instance.AddScope(app.ServiceProvider.GetService<RabbitScope>());
            }

        }

        public void Unload(ITAApplication app, PluginObject plg)
        {

        }
    }
}
