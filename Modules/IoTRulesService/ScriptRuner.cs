using IoTService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading;
using TemplateAction.Core;

namespace IoTRulesService
{
    public class ScriptRuner
    {
        private readonly ITAServiceProvider _provider;
        private readonly ILogger<ScriptRuner> _log;
        private readonly int _threadCount;
        private readonly ScriptWorkerThread[] _workerThreads;

        public ScriptRuner(ITAServiceProvider provider, ILoggerFactory logfactory)
        {
            _log = logfactory.CreateLogger<ScriptRuner>();
            _provider = provider;

            var option = _provider.GetService<IOptions<IotOption>>();
            _threadCount = option.Value.runer_count > 0 ? option.Value.runer_count : 4;

            _workerThreads = new ScriptWorkerThread[_threadCount];
            for (int i = 0; i < _threadCount; i++)
            {
                _workerThreads[i] = new ScriptWorkerThread(i, _log);
            }
        }

        public void PushConcurrentTask(string key, Action ac)
        {
            var index = Math.Abs(key.GetHashCode()) % _workerThreads.Length;
            _workerThreads[index].EnqueueTask(ac);
        }
    }

    public class ScriptWorkerThread
    {
        private readonly Thread _thread;
        private readonly ConcurrentQueue<Action> _taskQueue = new();
        private readonly AutoResetEvent _waiter = new(false);
        private readonly ILogger _log;
        private readonly int _id;

        public ScriptWorkerThread(int id, ILogger log)
        {
            _id = id;
            _log = log;
            _thread = new Thread(Run)
            {
                IsBackground = true,
                Name = $"ScriptWorker_{id}"
            };
            _thread.Start();
        }

        public void EnqueueTask(Action action)
        {
            _taskQueue.Enqueue(action);
            _waiter.Set(); // 通知线程有新任务
        }

        private void Run()
        {
            while (true)
            {
                try
                {
                    // 批量消费所有任务
                    while (_taskQueue.TryDequeue(out Action task))
                    {
                        try
                        {
                            task(); // 执行任务
                        }
                        catch (Exception ex)
                        {
                            _log.LogError(ex, "线程 {Id} 执行单个任务异常", _id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "线程 {Id} 主循环异常", _id);
                }

                // 等待下一个任务
                _waiter.WaitOne();
            }
        }
    }
}