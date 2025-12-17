using ChannelUtility;
using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<string, RtspStreamProcessor> _processorDict = new ConcurrentDictionary<string, RtspStreamProcessor>();
        private ConcurrentDictionary<string, string> _resetProcessors = new ConcurrentDictionary<string, string>();
        private FixVideoDeviceEventListener _deviceEventListener;
        public FixVideoService(IServiceProvider provider)
        {
            _provider = provider;
            _deviceEventListener = new FixVideoDeviceEventListener(provider);
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


            await eventBus.Bus.PubSub.SubscribeAsync<string>("AVideoCaptureItem", async (videoitem, tk) =>
            {
                var item = System.Text.Json.JsonSerializer.Deserialize<VideoCaptureItem>(videoitem);
                if (_processorDict.TryGetValue(item.Id, out RtspStreamProcessor tmp))
                {
                    tmp.Item = item;
                }
                else
                {
                    var tmpProccess = new RtspStreamProcessor(item, null, _deviceEventListener);
                    if (!await tmpProccess.StartAsync())
                    {
                        tmpProccess.CleanupFFmpegResources();
                        _resetProcessors.TryAdd(item.Id, item.Id);
                    }
                    _processorDict.TryAdd(item.Id, tmpProccess);
                }
            }, cfg =>
            {
                cfg.WithTopic("/VideoCapture.push");
                cfg.WithAutoDelete(true);
            });

            await eventBus.Bus.PubSub.SubscribeAsync<string>("BVideoCaptureItem", (itemId, tk) =>
            {
                if (_processorDict.TryRemove(itemId, out RtspStreamProcessor tmp))
                {
                    tmp.Dispose();
                }
                return Task.CompletedTask;
            }, cfg =>
            {
                cfg.WithTopic("/VideoCapture.del");
                cfg.WithAutoDelete(true);
            });

            await StreamTaskScheduler.Instance.StartAsync(stoppingToken);
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
            var option = _provider.GetService<IOptions<FixVideoOption>>();
            var eventBus = _provider.GetService<ClientBusProxy>();
            await eventBus.RedisHelper.HashSetAsync("FixVideoNode", option.Value.node_name, DateTime.Now.AddSeconds(60).ToString("o"));


            //重新处理
            var tmparr = _resetProcessors.ToArray();
            foreach (var item in tmparr)
            {
                if (_processorDict.TryGetValue(item.Key, out RtspStreamProcessor tmpProccess))
                {
                    if (!await tmpProccess.StartAsync())
                    {
                        tmpProccess.CleanupFFmpegResources();
                    }
                    else
                    {
                        _resetProcessors.TryRemove(item);
                    }
                }

            }

        }
    }
}
