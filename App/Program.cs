using Common;
using Common.FluentMigrator;
using Common.KuaiDi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Reflection;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            //配置分词词库目录
            JiebaNet.Segmenter.ConfigManager.ConfigFileBaseDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"Jieba";

            //配置log4net日志
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "log4net.config"));

            //加载快递信息
            KuaiDiPreRead.LoadData(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"data_set");

            //参数时区转换
            DefatulParamMapping.DefaultMappingResolver = Constants.DefaultMappingResolver;
            //初始化http服务并启动
            new TANetCoreHttpHostBuilder().ConfigureMiddleware(ks =>
            {
                //最大请求200M
                ks.Limits.MaxRequestBodySize = 200000000;
            }).ConfigureServices(ac =>
            {
                ac.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                    options.Providers.Add<GzipCompressionProvider>();
                });
            }).Configure((IApplicationBuilder builder) =>
            {
                builder.UseWebSockets();
                builder.UseResponseCompression();
                builder.UseAllowCORS();
                builder.UseDefaultFiles();
                var provider = new FileExtensionContentTypeProvider();
                provider.Mappings[".xls"] = "application/vnd.ms-excel";
                provider.Mappings[".apk"] = "application/vnd.android.package-archive";
                provider.Mappings[".wgt"] = "application/octet-stream";
                builder.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = (c) =>
                    {
                        var request = c.Context.Request;
                        StringValues wh;
                        if (request.Query.TryGetValue("wh", out wh))
                        {
                            try
                            {
                                //缩略图重定向
                                var thumstr = wh.ToString();
                                if (thumstr == "500x500")
                                {
                                    string turl = c.Context.Request.Path;
                                    int lastidx = turl.LastIndexOf('/');
                                    string finalurl = turl.Substring(0, lastidx) + "/s_" + turl.Substring(lastidx + 1);
                                    c.Context.Response.Redirect(finalurl);
                                }
                            }
                            catch { }

                        }
                    },
                    ContentTypeProvider = provider
                });
                builder.UseSwagger();
                builder.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "API接口文档 V1");
                    c.UseRequestInterceptor(@"(req) => {if(!req.headers['Authorization']){var reg = new RegExp('tk=(.*)', 'i');var r = window.location.search.match(reg);req.headers['Authorization']=r[1]; }return req; }");
                });
                builder.UseTAMvcAsync(app =>
                {
                    app.UseTAFluent();
                    app.UsePluginAssets(true);
                    app.UseParamMapping(new BodyParamMapping(Constants.ParamDecodeJson));
                    app.UseRouters(new RouterBuilder().UsePlugin().UseDefault(Assembly.GetEntryAssembly().GetName().Name).Build());
                });

            }).ConfigureLogging((config, logginbuilder) =>
            {
                logginbuilder.AddConfiguration(config.GetSection("Logging")).AddLog4Net().AddConsole();
            }).Build().Run();
        }
    }
}
