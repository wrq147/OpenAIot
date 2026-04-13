using Common.EventBus;
using IoTVideoService.DAL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTVideoService
{
    public class ConsumConfigThread : IDisposable
    {
        private ITAServiceProvider _provider;
        private Thread _consumThread;
        private AutoResetEvent _event = new AutoResetEvent(false);
        private CancellationTokenSource _cts;
        private readonly ConcurrentQueue<string> _idQueue = new ConcurrentQueue<string>();
        private readonly HashSet<string> _existIds = new HashSet<string>();
        private readonly object _lock = new object();
        public ConsumConfigThread(ITAServiceProvider serviceProvider)
        {
            _provider = serviceProvider;


        }
        public void Start()
        {
            _cts = new CancellationTokenSource();
            _consumThread = new Thread(() => ConsumLoop());
            _consumThread.IsBackground = true;
            _consumThread.Start();
        }
        public void Push(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            lock (_lock)
            {
                // 已存在则不入队
                if (_existIds.Contains(id)) return;

                _existIds.Add(id);
                _idQueue.Enqueue(id);
            }

            _event.Set();
        }
        private void ConsumLoop()
        {

            while (!_cts.Token.IsCancellationRequested)
            {
                _event.WaitOne();
                while (_idQueue.TryDequeue(out string id))
                {
                    try
                    {
                        ConsumTask(id).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    finally
                    {
                        lock (_lock) _existIds.Remove(id);
                    }
                }
            }
        }

        private async Task ConsumTask(string id)
        {
            var videoConfig = await _provider.GetService<VideoConfigDAL>().Select(id);
            var tvslist = await _provider.GetService<VideoSourceDAL>().SelectList(x => x.ConfigId == id);
            var tnatsScope = _provider.GetService<NatsScope>();
            foreach (var titem in tvslist)
            {
                await tnatsScope.DownUpVideoItemMessage(titem.NodeId, titem, videoConfig.AITasks);
            }
        }

        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void Dispose()
        {
            _cts.Cancel();

            if (_consumThread != null && _consumThread.IsAlive)
            {
                _consumThread.Join(TimeSpan.FromSeconds(5));
            }
        }
    }
}
