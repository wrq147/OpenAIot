using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.Route;

namespace TemplateAction.NetCore
{

    public static class TANetCoreHttpExtension
    {
        private static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public static async Task WriteAsync(this HttpResponse response, byte[] content)
        {
            await response.Body.WriteAsync(content);
        }
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value, SerializeOptions));
        }

        public static T GetObject<T>(this ISession session, string key) where T : class
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
        /// <summary>
        /// 文件新增异步保存
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task SaveAsAsync(this IRequestFile file, string filename)
        {
            await ((TANetCoreHttpFile)file).SaveAsAsync(filename);
        }

        /// <summary>
        /// 允许CORS请求
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAllowCORS(this IApplicationBuilder appBuilder, CorsOptions options = null)
        {
            if (options == null)
            {
                options = CorsOptions.Default;
            }

            appBuilder.Use(next =>
            {
                return context =>
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        if (options.AllowCredentials == "true" && options.AllowOrigin == "*")
                        {
                            if (context.Request.Headers.TryGetValue("Origin", out var neworgg))
                            {
                                context.Response.Headers.Add("Access-Control-Allow-Origin", neworgg);
                            }
                            else
                            {
                                context.Response.Headers.Add("Access-Control-Allow-Origin", options.AllowOrigin);
                            }
                        }
                        else
                        {
                            context.Response.Headers.Add("Access-Control-Allow-Origin", options.AllowOrigin);
                        }
                        if (options.AllowMethods != "*")
                        {
                            context.Response.Headers.Add("Access-Control-Allow-Methods", options.AllowMethods);
                        }
                        if (options.AllowCredentials == "true")
                        {
                            context.Response.Headers.Add("Access-Control-Allow-Credentials", options.AllowCredentials);
                        }
                        if (string.IsNullOrEmpty(options.AllowHeaders))
                        {
                            if (context.Request.Headers.TryGetValue("Access-Control-Request-Headers", out var newsshh))
                            {
                                context.Response.Headers.Add("Access-Control-Allow-Headers", newsshh);
                            }
                        }
                        else
                        {
                            context.Response.Headers.Add("Access-Control-Allow-Headers", options.AllowHeaders);
                        }
                        return Task.CompletedTask;
                    }
                    else
                    {
                        if (options.AllowCredentials == "true" && options.AllowOrigin == "*")
                        {
                            if (context.Request.Headers.TryGetValue("Origin", out var neworgg))
                            {
                                context.Response.Headers.Add("Access-Control-Allow-Origin", neworgg);
                            }
                            else
                            {
                                context.Response.Headers.Add("Access-Control-Allow-Origin", options.AllowOrigin);
                            }
                        }
                        else
                        {
                            context.Response.Headers.Add("Access-Control-Allow-Origin", options.AllowOrigin);
                        }
                    }
                    return next(context);
                };
            });
            return appBuilder;
        }

        /// <summary>
        /// 设置使用TA的MVC
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTAMvcAsync(this IApplicationBuilder appBuilder, Action<TASiteApplication> init = null)
        {
            if (init == null)
            {
                init = app =>
                {
                    //设置路由
                    string defns = Assembly.GetEntryAssembly().GetName().Name;
                    app.UseRouters(new RouterBuilder().UsePlugin().UseDefault(defns).Build());
                };
            }
            TAEventDispatcher.Instance.RegisterLoadBefore(init);

            TAEventDispatcher.Instance.RegisterPluginAllLoad((evt) =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    TANetCoreHttpApplication taapp = context.Features.Get<TANetCoreHttpApplication>();
                    if (taapp != null)
                    {
                        string requestUrl = context.Request.Path;
                        if (requestUrl.EndsWith(TAUtility.FILE_EXT, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"Code\":-667,\"Message\":\"文件受限制不能访问\"}");
                            return;
                        }

                        TANetCoreHttpContext tacontext = new TANetCoreHttpContext(context);
                        TAActionBuilder builder = tacontext.Application.Route(tacontext);
                        if (builder != null)
                        {
                            await builder.OutputAsync();
                            return;
                        }

                    }
                    await next.Invoke();
                });
                return Task.CompletedTask;
            });

            return appBuilder;
        }

    }
}
