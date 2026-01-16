using Common.Share;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common.EventBus
{
    public class NatsScope : IDispatcher
    {
        private ITAServiceProvider _provider;
        private INatsConnection _bus;
        private IOptions<GeneralOption> _option;
        private object _lock = new object();
        public INatsConnection Bus
        {
            get
            {
                if (_bus == null)
                {
                    lock (_lock)
                    {
                        if (_bus == null)
                        {
                            var opts = new NatsOpts
                            {
                                Url = _option.Value.event_bus_conn,
                                AuthOpts = new NatsAuthOpts
                                {
                                    Username = _option.Value.event_bus_user,
                                    Password = _option.Value.event_bus_pass
                                },
                                ConnectTimeout = TimeSpan.FromSeconds(5)
                            };
                            _bus = new NatsConnection(opts);
                            _bus.ConnectAsync().AsTask().Wait();
                        }
                    }
                }
                return _bus;
            }
        }

        public NatsScope(ITAServiceProvider provider)
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
            await Bus.PublishAsync(new NatsMsg<T>()
            {
                Subject = key,
                Data = evt
            }, DefalutNatsJsonSerializer<T>.Default);
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
            var requestTimeout = TimeSpan.FromSeconds(8);
            if (evt is QuartzExeEvent)
            {
                requestTimeout = TimeSpan.FromSeconds(60);
            }
            try
            {
                var replyMsg = await Bus.RequestAsync<T, Z>(key, evt, null, DefalutNatsJsonSerializer<T>.Default, DefalutNatsJsonSerializer<Z>.Default, null, new NatsSubOpts()
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    ThrowIfNoResponders = true
                });
                if (replyMsg.Data == null)
                {
                    var reply = new EvtResponse();
                    reply.IsDone = false;
                    reply.Code = Constants.PARSE_ERR;
                    reply.Message = "Call的回复数据异常";
                    return (Z)reply;
                }
                replyMsg.Data.IsDone = true;
                return replyMsg.Data;
            }
            catch (Exception ex)
            {
                var reply = new EvtResponse();
                reply.IsDone = false;
                reply.Code = Constants.TIME_OUT;
                reply.Message = "Call请求超时被取消";
                return (Z)reply;
            }

        }
    }
}
