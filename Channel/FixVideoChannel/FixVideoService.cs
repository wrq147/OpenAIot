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
using System.Runtime.InteropServices;


namespace FixVideoChannel
{
    public class FixVideoService : BackgroundService
    {
        private readonly TimeSpan _executionInterval = TimeSpan.FromSeconds(50);
        private Thread _timerThread;
        private IServiceProvider _provider;
        private ConcurrentDictionary<string, StreamProcessor> _processorDict = new ConcurrentDictionary<string, StreamProcessor>();
        private ConcurrentDictionary<string, string> _resetProcessors = new ConcurrentDictionary<string, string>();
        private FixVideoDeviceEventListener _deviceEventListener;
        private FixVideoOption _option;
        public FixVideoService(IServiceProvider provider)
        {
            _provider = provider;
            _deviceEventListener = new FixVideoDeviceEventListener(provider, this);
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
            ffmpeg.avformat_network_init();

        }
        public async Task VideoCaptureItemEvent(UpVideoItemMessage msg)
        {
            var item = msg.Item;
            if (_processorDict.TryGetValue(item.Id, out StreamProcessor tmp))
            {
                tmp.UpdateItem(item);
            }
            else
            {
                List<AIDetectorTask> tasklist = new List<AIDetectorTask>();
                if (msg.DetectList != null)
                {
                    foreach (var ditem in msg.DetectList)
                    {
                        tasklist.Add(new AIDetectorTask(ditem));
                    }
                }

                var tmpProccess = new StreamProcessor(msg.Item, tasklist, _deviceEventListener);
                if (!await tmpProccess.StartAsync())
                {
                    tmpProccess.CleanupFFmpegResources();
                    _resetProcessors.TryAdd(item.Id, item.Id);
                }
                _processorDict.TryAdd(item.Id, tmpProccess);
            }
        }
        public async Task DelCaptureItemEvent(DelVideoItemMessage msg)
        {
            if (_processorDict.TryRemove(msg.ItemId, out StreamProcessor tmp))
            {
                tmp.Dispose();
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += _deviceEventListener.OnDeviceDownMessage;

            _timerThread = new Thread(() => KeepAliveTask(stoppingToken))
            {
                IsBackground = true,
                Name = "AliveTaskThread"
            };
            _timerThread.Start();


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
            var eventBus = _provider.GetService<ClientBusProxy>();
            await eventBus.RedisHelper.HashSetAsync("FixVideoNode", eventBus.NodeGuid, DateTime.Now.AddSeconds(60).ToString("o"));


            //重新处理
            var tmparr = _resetProcessors.ToArray();
            foreach (var item in tmparr)
            {
                if (_processorDict.TryGetValue(item.Key, out StreamProcessor tmpProccess))
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
