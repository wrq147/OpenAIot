using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class StreamTaskScheduler
    {

        // 流处理任务队列
        private ConcurrentQueue<StreamProcessor> _processQueue;
        // 推流任务队列
        private ConcurrentQueue<PushTask> _globalPushQueue;

        // 信号量用于唤醒工作线程
        private AutoResetEvent _processEvent;
        private AutoResetEvent _pushEvent;

        // 工作线程数量
        private const int ProcessWorkerCount = 5;
        private const int PushWorkerCount = 5;
        private CancellationToken _stopToken;
        // 静态构造函数初始化工作线程

        // 使用Lazy<T>保证线程安全且延迟初始化
        private static readonly Lazy<StreamTaskScheduler> _instance = new Lazy<StreamTaskScheduler>(() => new StreamTaskScheduler());
        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static StreamTaskScheduler Instance => _instance.Value;

        // 私有构造函数防止外部实例化
        private StreamTaskScheduler()
        {
            // 初始化信号量
            _processEvent = new AutoResetEvent(false);
            _pushEvent = new AutoResetEvent(false);

            // 初始化队列
            _processQueue = new ConcurrentQueue<StreamProcessor>();
            _globalPushQueue = new ConcurrentQueue<PushTask>();
        }
        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _stopToken = stoppingToken;
            // 启动流处理工作线程
            for (int i = 0; i < ProcessWorkerCount; i++)
            {
                new Thread(ProcessWorkerWrapper)
                {
                    IsBackground = true,
                    Name = $"StreamProcessWorker_{i}"
                }.Start();
            }

            // 启动推流工作线程
            for (int i = 0; i < PushWorkerCount; i++)
            {
                new Thread(PushWorkerLoop)
                {
                    IsBackground = true,
                    Name = $"StreamPushWorker_{i}"
                }.Start();
            }
        }
        /// <summary>
        /// 提交流处理任务到全局队列
        /// </summary>
        public void EnqueueProcessTask(StreamProcessor processor)
        {
            _processQueue.Enqueue(processor);
            _processEvent.Set();
        }

        /// <summary>
        /// 提交推流任务到全局队列
        /// </summary>
        public void EnqueuePushTask(PushTask task)
        {
            _globalPushQueue.Enqueue(task);
            _pushEvent.Set();
        }
        private void ProcessWorkerWrapper()
        {
            ProcessWorkerLoop().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 流处理工作线程循环
        /// </summary>
        private async Task ProcessWorkerLoop()
        {
            while (!_stopToken.IsCancellationRequested)
            {
                // 等待任务信号（超时避免无任务时阻塞）
                _processEvent.WaitOne(100);

                // 批量处理队列中的任务
                while (_processQueue.TryDequeue(out var task))
                {
                    try
                    {
                        // 执行流处理
                        await task.ProcessStreamAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        // 取消操作忽略
                    }
                    catch (Exception ex)
                    {
                        // 记录异常（建议使用日志框架）
                        Console.WriteLine($"流处理任务异常: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 推流工作线程循环
        /// </summary>
        private void PushWorkerLoop()
        {
            while (!_stopToken.IsCancellationRequested)
            {
                _pushEvent.WaitOne(100);

                while (_globalPushQueue.TryDequeue(out var task))
                {
                    try
                    {
                        var pusher = task.Pusher;
                        if (pusher == null || !pusher.IsConnected)
                        {
                            continue;
                        }

                        switch (task.Type)
                        {
                            case PushTaskType.H264Frame:
                                pusher.ProcessH264Frame(task);
                                break;
                            case PushTaskType.AACFrame:
                                pusher.ProcessAacFrame(task);
                                break;
                            case PushTaskType.Disconnect:
                                pusher.ProcessDisconnect();
                                break;
                        }
                    }
                    catch (Exception ex) { }
                }
            }
        }
    }
}
