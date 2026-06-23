using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpHost
    {
        protected IWebHost _webHost;
        protected IConfiguration _config;
        protected string _webroot = "www";
        protected string _workroot;
        public const string WORK_PATH = "TA_WorkPath";
        private ILogger<TANetCoreHttpHost> _log;
        private IServiceCollection _rootServiceCollection;
        private TANetCoreHttpApplication _app;
        public TANetCoreHttpHost(IConfiguration config)
        {
            _workroot = config[WORK_PATH];
            _config = config;

        }
        public TANetCoreHttpHost Init(Action<IServiceCollection> configServices)
        {

            string webrootPath = Path.Combine(_workroot, _webroot);
            if (!Directory.Exists(webrootPath))
                Directory.CreateDirectory(webrootPath);

            IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseContentRoot(_workroot)
                .UseWebRoot(webrootPath)
                .ConfigureAppConfiguration(cfg =>
                {
                    cfg.AddConfiguration(_config);
                })
                .ConfigureServices((ctx, svc) =>
                {
                    configServices.Invoke(svc);
                    _rootServiceCollection = svc;
                })
                .UseKestrel(opts =>
                {
                    TANetConfigLoader.CreateKestrelOptionsFrom(_config, opts.ApplicationServices);
                    TAAsyncHelper.RunSync(async () =>
                    {
                        await TemplateAction.Core.TAEventDispatcher.Instance.DispatchInternal(opts).ConfigureAwait(false);
                    });

                })
                .Configure(appBuilder =>
                {
                    appBuilder.UseMiddleware<TANetCoreHttpMiddleware>();
                    TAAsyncHelper.RunSync(async () =>
                    {
                        await TemplateAction.Core.TAEventDispatcher.Instance.DispatchInternal(appBuilder).ConfigureAwait(false);
                    });
                    TemplateApp.Instance.Init(webrootPath);
                    _app = new TANetCoreHttpApplication(appBuilder, _rootServiceCollection);
                    _rootServiceCollection.AddSingleton(_app);
                    _app.Init(_workroot, Assembly.GetEntryAssembly());
                    appBuilder.ApplicationServices = _app.GetServiceProvider();

                });

            _webHost = webHostBuilder.Build();

            var sp = _webHost.Services;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            _log = loggerFactory.CreateLogger<TANetCoreHttpHost>();
            _log.LogInformation("UseKestrel 初始化完成");

            return this;
        }
        public void Run()
        {
            RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task RunAsync(CancellationToken token = default)
        {
            try
            {
                await StartAsync(token).ConfigureAwait(false);
                await WaitForTokenShutdownAsync(token).ConfigureAwait(false);
            }
            finally
            {
                await StopAsync().ConfigureAwait(false);
            }
        }

        private async Task StartAsync(CancellationToken token)
        {
            var hostedServices = _webHost.Services.GetServices<IHostedService>();
            foreach (var svc in hostedServices)
                await svc.StartAsync(token);

            await _webHost.StartAsync(token).ConfigureAwait(false);
            _log.LogInformation("服务已启动");
        }

        private async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _log.LogInformation("正在停止服务...");
            _app?.UnloadAllPlugin();

            var hostedServices = _webHost.Services.GetServices<IHostedService>();
            foreach (var svc in hostedServices)
                await svc.StopAsync(cancellationToken);

            await _webHost.StopAsync(cancellationToken).ConfigureAwait(false);
            _log.LogInformation("服务已完全停止");
        }

        private async Task WaitForTokenShutdownAsync(CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            token.Register(obj => ((TaskCompletionSource<object>)obj).TrySetResult(null), tcs);
            await tcs.Task.ConfigureAwait(false);
        }
    }
}
