using AuthService;
using Common;
using MonitorService.Business;
using MonitorService.Controller;
using MonitorService.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;
using WeiXinService.Business;
using WeiXinService.Model;

namespace WeiXinService
{
    public class CorpWxSyncThread
    {
        private ConcurrentBag<CorpSyncItem> _syncList;
        private ITAServiceProvider _provider;
        private Thread _thread;
        private ManualResetEvent _mre = new ManualResetEvent(false);
        private bool _isAborted;
        public CorpWxSyncThread(ITAServiceProvider provider)
        {
            _isAborted = false;
            _syncList = new ConcurrentBag<CorpSyncItem>();
            _provider = provider;
            _thread = new Thread(new ThreadStart(Excute));
            _thread.Start();
        }
        public void Push(CorpSyncItem data)
        {
            _syncList.Add(data);
            _mre.Set();
        }

        private async void Excute()
        {
            while (!_isAborted)
            {
                try
                {
                    CorpSyncItem syncItem;
                    while (_syncList.TryTake(out syncItem))
                    {
                        await _provider.GetService<CorpSyncBLL>().ExecuteSync(syncItem.task, syncItem.data);
                    }
                    _mre.WaitOne();
                    _mre.Reset();
                }
                catch (ThreadAbortException)
                {
                    _isAborted = true;
                }
                catch (ThreadInterruptedException)
                {
                    _isAborted = true;
                }
            }

        }
    }

    public class CorpSyncItem
    {
        public MZ_CorpSync data { get; set; }
        public MZ_CorpTask task { get; set; }
    }
}
