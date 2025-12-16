using ChannelUtility;
using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class FixVideoService : BackgroundService
    {
        private readonly TimeSpan _executionInterval = TimeSpan.FromSeconds(50);
        private Thread _timerThread;
        private IServiceProvider _provider;
        private HashSet<string> _videoIds = new HashSet<string>();
        public FixVideoService(IServiceProvider provider)
        {
            _provider = provider;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RuntimeInformation.OSArchitecture == Architecture.X64)
            {
                ffmpeg.RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpegLibs", "Windows", "x64");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if(RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    ffmpeg.RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpegLibs", "Linux", "x64");
                }
                else if(RuntimeInformation.OSArchitecture == Architecture.Arm64)
                {
                    ffmpeg.RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpegLibs", "Linux", "arm64");
                }
            }
            if (string.IsNullOrEmpty(ffmpeg.RootPath))
            {
                ffmpeg.RootPath = provider.GetService<IOptions<FixVideoOption>>().Value.ffmpeg_path;
            }
            ffmpeg.avformat_network_init();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();

            _timerThread = new Thread(() => KeepAliveTask(stoppingToken))
            {
                IsBackground = true,
                Name = "AliveTaskThread"
            };
            _timerThread.Start();


            await eventBus.Bus.PubSub.SubscribeAsync<string>("VideoCaptureItem", async (videoitem, tk) =>
            {
                var item = System.Text.Json.JsonSerializer.Deserialize<VideoCaptureItemcs>(videoitem);
                if (!_videoIds.Contains(item.Id))
                {
                    _videoIds.Add(item.Id);
                }
            }, cfg =>
            {
                cfg.WithTopic("/VideoCapture.push");
                cfg.WithAutoDelete(true);
            });

        }

        private async Task KeepAliveTask(CancellationToken cancellationToken)
        {
            try
            {
                await KeepAlive();

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // 等待50秒（监听取消信号，避免无意义的等待）
                        Thread.Sleep(_executionInterval);
                        await KeepAlive();
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
                            Thread.Sleep(TimeSpan.FromSeconds(5));
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
        private async Task KeepAlive()
        {
            var option = _provider.GetService<IOptions<FixVideoOption>>();
            var eventBus = _provider.GetService<ClientBusProxy>();
            await eventBus.RedisHelper.HashSetAsync("FixVideoNode", option.Value.node_name, DateTime.Now.AddSeconds(60).ToString("o"));
        }
    }
}
