using ChannelUtility;
using ChannelUtility.Message;
using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace FixVideoChannel
{
    public class FixVideoService : BackgroundService
    {
        private readonly TimeSpan _executionInterval = TimeSpan.FromSeconds(20);
        private Thread _timerThread;
        private IServiceProvider _provider;
        private FixVideoDeviceEventListener _deviceEventListener;
        private FixVideoOption _option;
        public FixVideoService(IServiceProvider provider)
        {
            _provider = provider;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RuntimeInformation.OSArchitecture == Architecture.X64)
            {
                ffmpeg.RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpegLibs", "Windows", "x64");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    ffmpeg.RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpegLibs", "Linux", "x64");
                }
                else if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
                {
                    ffmpeg.RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpegLibs", "Linux", "arm64");
                }
            }
            _option = provider.GetService<IOptions<FixVideoOption>>().Value;
            if (string.IsNullOrEmpty(ffmpeg.RootPath))
            {
                ffmpeg.RootPath = _option.ffmpeg_path;
            }
            _deviceEventListener = new FixVideoDeviceEventListener(provider);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ffmpeg.avformat_network_init();
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += _deviceEventListener.OnDeviceDownMessage;

            _timerThread = new Thread(() => KeepAliveTask(stoppingToken))
            {
                IsBackground = true,
                Name = "AliveTaskThread"
            };
            _timerThread.Start();

            ZLMediaKitServer.Instance.Start(_option, _provider, _deviceEventListener);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            ffmpeg.avformat_network_deinit();
            ZLMediaKitServer.Instance.Stop();
            return base.StopAsync(cancellationToken);
        }

        private async Task KeepAliveTask(CancellationToken cancellationToken)
        {
            try
            {
                await KeepAlive(cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(_executionInterval, cancellationToken);
                        await KeepAlive(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // 收到取消信号，退出循环
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"定时线程内异常：{ex.Message}", ex);
                        // 异常后短暂延迟，避免频繁报错
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"独立定时线程异常：{ex.Message}", ex);
            }
            finally
            {
                Console.WriteLine("独立定时线程已退出");
            }
        }
        private async Task KeepAlive(CancellationToken cancellationToken)
        {
            //节点保活
            var eventBus = _provider.GetService<ClientBusProxy>();
            await eventBus.RedisHelper.HashSetAsync("FixVideoNode", eventBus.NodeGuid, DateTime.Now.AddSeconds(30).ToString("o"));
        }
    }

}
