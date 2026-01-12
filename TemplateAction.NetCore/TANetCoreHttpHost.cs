using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.Label;
using ServiceCollection = Microsoft.Extensions.DependencyInjection.ServiceCollection;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpHost
    {
        protected ServiceCollection _servicecollection;
        protected ILoggerFactory _logger;
        protected IApplicationBuilder _appBuilder;
        protected TANetCoreHttpApplication _app;
        protected IWebHostEnvironment _hostingEnvironment;
        protected IConfiguration _config;

        private SocketTransportOptions _socketOptions;
        private KestrelServer _server;
        private bool _startedServer = false;
        private bool _stopped = false;
        private string _webroot = "www";
        private string _workroot;
        private KestrelServerOptions _kestrelOptions;
        public const string WORK_PATH = "TA_WorkPath";

        private ILogger<TANetCoreHttpHost> _log;
        public TANetCoreHttpHost(IConfiguration config, ServiceCollection services)
        {
            _workroot = config[WORK_PATH];
            _config = config;
            _servicecollection = services;

            //初始化主机环境
            _hostingEnvironment = new TANetWebHostingEnvironment();
            _hostingEnvironment.ContentRootFileProvider = new PhysicalFileProvider(_workroot);
            _hostingEnvironment.ContentRootPath = _workroot;
            _hostingEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
            string webroot = Path.Combine(_workroot, _webroot);
            if (!Directory.Exists(webroot))
            {
                Directory.CreateDirectory(webroot);
            }
            _hostingEnvironment.WebRootFileProvider = new PhysicalFileProvider(webroot);
            _hostingEnvironment.WebRootPath = webroot;
            _servicecollection.AddSingleton<IHostEnvironment>(_hostingEnvironment);
            _servicecollection.AddSingleton<IWebHostEnvironment>(_hostingEnvironment);

            //初始化模板
            TemplateApp.Instance.Init(webroot);

            _servicecollection.AddSingleton<IConfiguration>(_config);

            ServiceProvider serviceprovider = _servicecollection.BuildServiceProvider();
            _logger = serviceprovider.GetRequiredService<ILoggerFactory>();

            _log = _logger.CreateLogger<TANetCoreHttpHost>();
            _log.LogInformation("服务初始化中...");

            _kestrelOptions = TANetConfigLoader.CreateKestrelOptionsFrom(_config, serviceprovider);

            _socketOptions = new SocketTransportOptions();
            _server = new KestrelServer(Options.Create(_kestrelOptions), new SocketTransportFactory(Options.Create(_socketOptions), _logger), _logger);
            _appBuilder = new ApplicationBuilderFactory(serviceprovider).CreateBuilder(_server.Features);
            _appBuilder.ApplicationServices = serviceprovider;

            TAAsyncHelper.RunSync(async () =>
            {
                await TAEventDispatcher.Instance.DispatchInternal(_appBuilder).ConfigureAwait(false);
            });
            _app = new TANetCoreHttpApplication(_appBuilder, _kestrelOptions, services);
            _app.Init(_workroot, Assembly.GetEntryAssembly());
        }


        public void Run()
        {
            RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        private async Task StartAsync(CancellationToken token)
        {
            if (_startedServer) return;
            _startedServer = true;

            //先启动其它服务
            var otherHosts = _app.ServiceProvider.GetServices<IHostedService>();
            foreach (var host in otherHosts)
            {
                await host.StartAsync(token);
            }

            await _server.StartAsync(_app, token).ConfigureAwait(false);
            _log.LogInformation("服务已启动");
        }
        private async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_stopped) return;
            _stopped = true;
            _log.LogInformation("服务已停止");
            if (_startedServer)
            {
                //先结束TAApp服务
                _app.UnloadAllPlugin();
                //最后结束其它服务
                var otherHosts = _app.ServiceProvider.GetServices<IHostedService>();
                foreach (var host in otherHosts)
                {
                    await host.StopAsync(cancellationToken);
                }
                await _server.StopAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        private async Task RunAsync(CancellationToken token = default)
        {
            // Wait for token shutdown if it can be canceled
            if (token.CanBeCanceled)
            {
                await RunAsyncNeedToken(token).ConfigureAwait(false);
                return;
            }

            //如果token不能被取消，则使用 Ctrl+C和SIGTERM 关闭应用程序
            ManualResetEventSlim done = new ManualResetEventSlim(false);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                using (TANetCoreHttpShutdownTrigger lifetime = new TANetCoreHttpShutdownTrigger(cts, done))
                {
                    try
                    {
                        await RunAsyncNeedToken(cts.Token).ConfigureAwait(false);
                        lifetime.SetExitedGracefully();
                    }
                    finally
                    {
                        done.Set();
                    }
                }
            }
        }
        private async Task RunAsyncNeedToken(CancellationToken token)
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

        private async Task WaitForTokenShutdownAsync(CancellationToken token)
        {
            TaskCompletionSource<object> waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            token.Register((obj) =>
            {
                var tcs = (TaskCompletionSource<object>)obj;
                tcs.TrySetResult(null);
            }, waitForStop);
            await waitForStop.Task.ConfigureAwait(false);
        }
    }
}
