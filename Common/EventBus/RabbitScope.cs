using Common.Share;
using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.DI;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common.EventBus
{
    public class RabbitScope : IDispatcher
    {
        private ITAServiceProvider _provider;
        private IBus _bus;
        private IOptions<GeneralOption> _option;
        private object _lock = new object();
        public IBus Bus
        {
            get
            {
                if (_bus == null)
                {
                    lock (_lock)
                    {
                        if (_bus == null)
                        {
                            _bus = RabbitHutch.CreateBus(_option.Value.event_bus_conn, x =>
                            {
                                x.Register<IConsumerErrorStrategy, AlwaysRequeueErrorStrategy>();
                            });
                        }
                    }
                }
                return _bus;
            }
        }
        public RabbitScope(ITAServiceProvider provider)
        {
            _provider = provider;
            _option = _provider.GetService<IOptions<GeneralOption>>();
        }
        /// <summary>
        /// 分发事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="evt"></param>
        /// <returns></returns>
        public async Task Dispatch<T>(string key, T evt) where T : class
        {
            if (key.StartsWith("/"))
            {
                await Bus.PubSub.PublishAsync(evt, key);
            }
        }
        /// <summary>
        /// 分发事件并等待处理结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Z"></typeparam>
        /// <param name="key"></param>
        /// <param name="evt"></param>
        /// <returns></returns>
        public async Task<Z> DispathWait<T, Z>(string key, T evt)
            where T : ResponseEvent
            where Z : EvtResponse
        {
            var tcs = new TaskCompletionSource<Z>(TaskCreationOptions.RunContinuationsAsynchronously);
            //8秒后自动取消
            var cts = new CancellationTokenSource(8000);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            var rs = await Bus.SendReceive.ReceiveAsync<Z>("dispatch.response." + evt.MessageId, msg =>
            {
                tcs.TrySetResult(msg);
            }, cfg =>
            {
                cfg.WithAutoDelete(true);
            }, cts.Token);
            try
            {
                await Bus.PubSub.PublishAsync(evt, key);
                var reply = await tcs.Task.ConfigureAwait(false);
                reply.IsDone = true;
                rs.Dispose();
                return reply;
            }
            catch (Exception ex)
            {
                rs.Dispose();
                return null;
            }
        }
    }
}
