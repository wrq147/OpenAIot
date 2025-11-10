using Common.Share;
using EasyNetQ;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.Core.Dispatcher;

namespace Common.EventBus
{
    public static class PluginEventRegisterExtends
    {
        private static Dictionary<string, List<BusItem>> _busItems = new Dictionary<string, List<BusItem>>();
        /// <summary>
        /// 注册监听业务事件
        /// </summary>
        /// <param name="plg"></param>
        /// <param name="name"></param>
        /// <param name="ac"></param>
        public static void RegisterBus(this PluginObject plg, string name, Func<BusEvent, Task> ac)
        {
            if (_busItems.TryGetValue(plg.Name, out List<BusItem> items))
            {
                items.Add(new BusItem()
                {
                    name = name,
                    func = ac
                });
            }
            else
            {
                List<BusItem> newItems = new List<BusItem>();
                newItems.Add(new BusItem()
                {
                    name = name,
                    func = ac
                });
                _busItems.Add(plg.Name, newItems);

                var generalOption = plg.Collection.GetService<IOptions<GeneralOption>>();
                if (string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
                {
                    plg.Dispatcher.Register<BusEvent>(BusEvent.EventKey, async (tmpitem) =>
                    {
                        foreach (var item in newItems)
                        {
                            if (item.name == tmpitem.Name && item.func != null)
                            {
                                await item.func.Invoke(tmpitem);
                            }
                        }
                    });
                }
                else
                {
                    var bus = plg.Collection.GetService<RabbitScope>().Bus;
                    bus.PubSub.SubscribeAsync<BusEvent>("Bussin" + plg.Name, async (bs) =>
                    {
                        foreach (var item in newItems)
                        {
                            if (item.name == bs.Name && item.func != null)
                            {
                                await item.func.Invoke(bs);
                            }
                        }
                    }, cfg =>
                    {
                        cfg.WithTopic(BusEvent.EventKey);
                        cfg.WithAutoDelete(true);
                    });
                }
            }

        }

        private static Dictionary<string, List<CallItem>> _callItems = new Dictionary<string, List<CallItem>>();

        /// <summary>
        /// 注册监听回调业务
        /// </summary>
        /// <param name="plg"></param>
        /// <param name="name"></param>
        /// <param name="ac"></param>
        public static void RegisterCall(this PluginObject plg, string name, Func<CallEvent, Task<CallResponse>> ac)
        {
            if (_callItems.TryGetValue(plg.Name, out List<CallItem> items))
            {
                items.Add(new CallItem()
                {
                    name = name,
                    func = ac
                });
            }
            else
            {
                List<CallItem> newItems = new List<CallItem>();
                newItems.Add(new CallItem()
                {
                    name = name,
                    func = ac
                });
                _callItems.Add(plg.Name, newItems);

                var generalOption = plg.Collection.GetService<IOptions<GeneralOption>>();
                if (string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
                {
                    plg.Dispatcher.RegisterReponse(CallEvent.EventKey, new DefaultResponseHandler<CallEvent, CallResponse>(async (tmpitem) =>
                    {
                        foreach (var item in newItems)
                        {
                            if (item.name == tmpitem.Name && item.func != null)
                            {
                                return await item.func.Invoke(tmpitem);
                            }
                        }
                        return CallResponse.Next();
                    }));
                }
                else
                {
                    var bus = plg.Collection.GetService<RabbitScope>().Bus;
                    bus.PubSub.SubscribeAsync<CallEvent>("Bussin" + plg.Name, async (bs) =>
                    {
                        CallResponse rs = CallResponse.Next();
                        foreach (var item in newItems)
                        {
                            if (item.name == bs.Name && item.func != null)
                            {
                                rs = await item.func.Invoke(bs);
                                break;
                            }
                        }
                        if (rs.IsDone)
                        {
                            await bus.SendReceive.SendAsync("dispatch.response." + bs.MessageId, rs);
                        }
                    }, cfg =>
                    {
                        cfg.WithTopic(CallEvent.EventKey);
                        cfg.WithAutoDelete(true);
                    });
                }
            }

        }

        /// <summary>
        /// 注册监听消息事件
        /// </summary>
        /// <param name="plg"></param>
        /// <param name="ac"></param>
        public static void RegisterNotice(this PluginObject plg, Func<NoticeEvent, Task> ac)
        {
            var generalOption = plg.Collection.GetService<IOptions<GeneralOption>>();
            if (string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
            {
                plg.Dispatcher.Register<NoticeEvent>(NoticeEvent.EventKey, ac);
            }
            else
            {
                var bus = plg.Collection.GetService<RabbitScope>().Bus;
                bus.PubSub.SubscribeAsync<NoticeEvent>("Bussin" + plg.Name, async (bs) =>
                {
                    await ac.Invoke(bs);
                }, cfg =>
                {
                    cfg.WithTopic(NoticeEvent.EventKey);
                    cfg.WithAutoDelete(true);
                });
            }
        }

        public static void RegisterTime(this PluginObject plg, Func<TimeEvent, Task> ac)
        {
            var generalOption = plg.Collection.GetService<IOptions<GeneralOption>>();
            if (string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
            {
                plg.Dispatcher.Register<TimeEvent>(TimeEvent.EventKey, ac);
            }
            else
            {
                var bus = plg.Collection.GetService<RabbitScope>().Bus;
                bus.PubSub.SubscribeAsync<TimeEvent>("Bussin" + plg.Name, async (bs) =>
                {
                    await ac.Invoke(bs);
                }, cfg =>
                {
                    cfg.WithTopic(TimeEvent.EventKey);
                    cfg.WithAutoDelete(true);
                });
            }
        }
    }

    public struct CallItem
    {
        public string name { get; set; }
        public Func<CallEvent, Task<CallResponse>> func { get; set; }
    }
    public struct BusItem
    {
        public string name { get; set; }
        public Func<BusEvent, Task> func { get; set; }
    }
}
