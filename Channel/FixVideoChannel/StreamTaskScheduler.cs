using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    internal class WorkerQueue<T>
    {
        /// <summary>
        /// 线程本地任务队列
        /// </summary>
        public ConcurrentQueue<T> Queue { get; } = new ConcurrentQueue<T>();

        /// <summary>
        /// 线程本地信号量
        /// </summary>
        public AutoResetEvent Signal { get; } = new AutoResetEvent(false);

        /// <summary>
        /// 线程名称
        /// </summary>
        public string Name { get; set; }
    }
    public class StreamTaskScheduler
    {
        #region 流处理线程相关
        // 每个流处理线程的独立队列集合
        private WorkerQueue<StreamProcessor>[] _processWorkerQueues;
        // 流处理工作线程数量
        private const int ProcessWorkerCount = 5;
        #endregion

        #region 推流线程相关
        // 每个推流线程的独立队列集合
        private WorkerQueue<PushTask>[] _pushWorkerQueues;
        // 推流工作线程数量
        private const int PushWorkerCount = 5;
        // 推流任务分发索引（用于轮询分发）
        private int _pushTaskIndex = 0;
        #endregion

        // 取消令牌
        private CancellationToken _stopToken;

        // 单例实例（线程安全延迟初始化）
        private static readonly Lazy<StreamTaskScheduler> _instance =
            new Lazy<StreamTaskScheduler>(() => new StreamTaskScheduler());
        public static StreamTaskScheduler Instance => _instance.Value;

        // 私有构造函数
        private StreamTaskScheduler()
        {
            // 初始化流处理线程的队列和信号量
            _processWorkerQueues = new WorkerQueue<StreamProcessor>[ProcessWorkerCount];
            for (int i = 0; i < ProcessWorkerCount; i++)
            {
                _processWorkerQueues[i] = new WorkerQueue<StreamProcessor>
                {
                    Name = $"StreamProcessWorker_{i}"
                };
            }

            // 初始化推流线程的队列和信号量
            _pushWorkerQueues = new WorkerQueue<PushTask>[PushWorkerCount];
            for (int i = 0; i < PushWorkerCount; i++)
            {
                _pushWorkerQueues[i] = new WorkerQueue<PushTask>
                {
                    Name = $"StreamPushWorker_{i}"
                };
            }
        }

        /// <summary>
        /// 启动所有工作线程
        /// </summary>
        /// <param name="stoppingToken">停止令牌</param>
        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _stopToken = stoppingToken;

            // 启动流处理工作线程（每个线程对应自己的队列）
            for (int i = 0; i < ProcessWorkerCount; i++)
            {
                var workerQueue = _processWorkerQueues[i];
                new Thread(() => ProcessWorkerLoop(workerQueue))
                {
                    IsBackground = true,
                    Name = workerQueue.Name
                }.Start();
            }

            // 启动推流工作线程（每个线程对应自己的队列）
            for (int i = 0; i < PushWorkerCount; i++)
            {
                var workerQueue = _pushWorkerQueues[i];
                new Thread(() => PushWorkerLoop(workerQueue))
                {
                    IsBackground = true,
                    Name = workerQueue.Name
                }.Start();
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 提交流处理任务（轮询分发到不同线程的队列）
        /// </summary>
        /// <param name="processor">流处理器</param>
        public void EnqueueProcessTask(StreamProcessor processor)
        {
            if (processor == null) throw new ArgumentNullException(nameof(processor));

            // 轮询选择目标工作线程队列（也可使用哈希策略，比如按任务ID哈希）
            var targetIndex = Math.Abs(processor.VideoId.GetHashCode() % ProcessWorkerCount);
            var targetQueue = _processWorkerQueues[targetIndex];

            // 将任务加入目标线程的队列
            targetQueue.Queue.Enqueue(processor);
            // 唤醒目标线程的信号量
            targetQueue.Signal.Set();
        }

        /// <summary>
        /// 提交推流任务（轮询分发到不同线程的队列）
        /// </summary>
        /// <param name="task">推流任务</param>
        public void EnqueuePushTask(PushTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            // 轮询选择目标工作线程队列
            var targetIndex = Math.Abs(task.Pusher.VideoId.GetHashCode() % PushWorkerCount);
            var targetQueue = _pushWorkerQueues[targetIndex];

            // 将任务加入目标线程的队列
            targetQueue.Queue.Enqueue(task);
            // 唤醒目标线程的信号量
            targetQueue.Signal.Set();
        }

        /// <summary>
        /// 流处理工作线程循环（绑定到指定的线程队列）
        /// </summary>
        /// <param name="workerQueue">线程本地队列</param>
        private async void ProcessWorkerLoop(WorkerQueue<StreamProcessor> workerQueue)
        {
            while (!_stopToken.IsCancellationRequested)
            {
                try
                {
                    // 等待当前线程的信号量（设置1秒超时，避免取消令牌无法及时响应）
                    var waitResult = workerQueue.Signal.WaitOne(1000);

                    // 如果是取消请求，直接退出
                    if (_stopToken.IsCancellationRequested) break;

                    // 批量处理当前线程队列中的任务
                    while (workerQueue.Queue.TryDequeue(out var task))
                    {
                        try
                        {
                            await task.ProcessStreamAsync();
                        }
                        catch (OperationCanceledException)
                        {
                            // 取消操作忽略
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{workerQueue.Name}] 流处理任务异常: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{workerQueue.Name}] 工作线程异常: {ex.Message}");
                }
            }

            Console.WriteLine($"[{workerQueue.Name}] 流处理工作线程已停止");
        }

        /// <summary>
        /// 推流工作线程循环（绑定到指定的线程队列）
        /// </summary>
        /// <param name="workerQueue">线程本地队列</param>
        private void PushWorkerLoop(WorkerQueue<PushTask> workerQueue)
        {
            while (!_stopToken.IsCancellationRequested)
            {
                try
                {
                    // 等待当前线程的信号量（设置1秒超时）
                    var waitResult = workerQueue.Signal.WaitOne(1000);

                    // 如果是取消请求，直接退出
                    if (_stopToken.IsCancellationRequested) break;

                    // 批量处理当前线程队列中的任务
                    while (workerQueue.Queue.TryDequeue(out var task))
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
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{workerQueue.Name}] 推流任务异常: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{workerQueue.Name}] 工作线程异常: {ex.Message}");
                }
            }

            Console.WriteLine($"[{workerQueue.Name}] 推流工作线程已停止");
        }
    }
}
