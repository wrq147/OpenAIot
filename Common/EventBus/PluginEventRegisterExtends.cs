using Common.Share;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                    Task.Run(async () =>
                    {
                        var bus = plg.Collection.GetService<NatsScope>().Bus;
                        await foreach (var msg in bus.SubscribeAsync(BusEvent.EventKey, "Bussin" + plg.Name, DefalutNatsJsonSerializer<BusEvent>.Default))
                        {
                            if (msg.Data == null)
                            {
                                continue;
                            }
                            foreach (var item in newItems)
                            {
                                if (item.name == msg.Data.Name && item.func != null)
                                {
                                    await item.func.Invoke(msg.Data);
                                }
                            }
                        }
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
                    Task.Run(async () =>
                    {
                        var bus = plg.Collection.GetService<NatsScope>().Bus;
                        await foreach (var msg in bus.SubscribeAsync(CallEvent.EventKey, "Bussin" + plg.Name, DefalutNatsJsonSerializer<CallEvent>.Default))
                        {
                            if (msg.Data == null)
                            {
                                continue;
                            }
                            CallResponse rs = CallResponse.Next();
                            foreach (var item in newItems)
                            {
                                if (item.name == msg.Data.Name && item.func != null)
                                {
                                    rs = await item.func.Invoke(msg.Data);
                                    break;
                                }
                            }
                            if (rs.IsDone && !string.IsNullOrEmpty(msg.ReplyTo))
                            {
                                await bus.PublishAsync<CallResponse>(new NatsMsg<CallResponse>()
                                {
                                    Subject = msg.ReplyTo,
                                    Data = rs
                                }, DefalutNatsJsonSerializer<CallResponse>.Default);
                            }
                        }
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
                Task.Run(async () =>
                {
                    var bus = plg.Collection.GetService<NatsScope>().Bus;
                    await foreach (var msg in bus.SubscribeAsync(NoticeEvent.EventKey, "Bussin" + plg.Name, DefalutNatsJsonSerializer<NoticeEvent>.Default))
                    {
                        if (msg.Data == null)
                        {
                            continue;
                        }
                        await ac.Invoke(msg.Data);
                    }
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
                Task.Run(async () =>
                {
                    var bus = plg.Collection.GetService<NatsScope>().Bus;
                    await foreach (var msg in bus.SubscribeAsync(TimeEvent.EventKey, "Bussin" + plg.Name, DefalutNatsJsonSerializer<TimeEvent>.Default))
                    {
                        if (msg.Data == null)
                        {
                            continue;
                        }
                        await ac.Invoke(msg.Data);
                    }
                });

            }
        }



        /// <summary>
        /// 注册监听Quartz执行任务
        /// </summary>
        /// <param name="plg"></param>
        public static void RegisterQuartzTask(this PluginObject plg)
        {
            var generalOption = plg.Collection.GetService<IOptions<GeneralOption>>();
            if (string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
            {
                plg.Dispatcher.Register<QuartzExeEvent>($"{QuartzExeEvent.EventKey}.{plg.Name}", async (tmpitem) =>
                {
                    try
                    {
                        List<object> methodParams = tmpitem.GetMethodParams();
                        object obj = plg.Collection.GetService(tmpitem.ClassName);
                        if (obj == null)
                        {
                            throw new Exception("获取不到类：" + tmpitem.ClassName);
                        }
                        object rt;
                        if (methodParams == null)
                        {
                            rt = obj.GetType().GetMethod(tmpitem.MethodName)?.Invoke(obj, null);
                        }
                        else
                        {
                            rt = obj.GetType().GetMethod(tmpitem.MethodName)?.Invoke(obj, methodParams.ToArray());
                        }

                        if (rt is Task t)
                        {
                            await t;
                        }
                    }
                    catch { }
                });

                plg.Dispatcher.RegisterReponse($"{QuartzExeEvent.EventKey}.{plg.Name}", new DefaultResponseHandler<QuartzExeEvent, QuartzExeResponse>(async (tmpitem) =>
                {
                    try
                    {
                        List<object> methodParams = tmpitem.GetMethodParams();
                        object obj = plg.Collection.GetService(tmpitem.ClassName);
                        if (obj == null)
                        {
                            throw new Exception("获取不到类：" + tmpitem.ClassName);
                        }
                        object rt;
                        if (methodParams == null)
                        {
                            rt = obj.GetType().GetMethod(tmpitem.MethodName)?.Invoke(obj, null);
                        }
                        else
                        {
                            rt = obj.GetType().GetMethod(tmpitem.MethodName)?.Invoke(obj, methodParams.ToArray());
                        }

                        if (rt is Task t)
                        {
                            await t;
                        }
                        return QuartzExeResponse.Success();
                    }
                    catch (Exception ex)
                    {
                        return QuartzExeResponse.Error(99, ex.Message);
                    }
                }));
            }
            else
            {
                Task.Run(async () =>
                {
                    var bus = plg.Collection.GetService<NatsScope>().Bus;
                    await foreach (var msg in bus.SubscribeAsync($"{QuartzExeEvent.EventKey}.{plg.Name}", "Quartz_" + plg.Name, DefalutNatsJsonSerializer<QuartzExeEvent>.Default))
                    {
                        if (msg.Data == null)
                        {
                            continue;
                        }
                        try
                        {
                            List<object> methodParams = msg.Data.GetMethodParams();
                            object obj = plg.Collection.GetService(msg.Data.ClassName);
                            if (obj == null)
                            {
                                throw new Exception("获取不到类：" + msg.Data.ClassName);
                            }
                            object rt;
                            if (methodParams == null)
                            {
                                rt = obj.GetType().GetMethod(msg.Data.MethodName)?.Invoke(obj, null);
                            }
                            else
                            {
                                rt = obj.GetType().GetMethod(msg.Data.MethodName)?.Invoke(obj, methodParams.ToArray());
                            }

                            if (rt is Task t)
                            {
                                await t;
                            }
                            if (msg.Data.DisConcurrent)
                            {
                                await bus.PublishAsync<QuartzExeResponse>(new NatsMsg<QuartzExeResponse>()
                                {
                                    Subject = msg.ReplyTo,
                                    Data = QuartzExeResponse.Success()
                                }, DefalutNatsJsonSerializer<QuartzExeResponse>.Default);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (msg.Data.DisConcurrent)
                            {
                                await bus.PublishAsync<QuartzExeResponse>(new NatsMsg<QuartzExeResponse>()
                                {
                                    Subject = msg.ReplyTo,
                                    Data = QuartzExeResponse.Error(99, ex.Message)
                                }, DefalutNatsJsonSerializer<QuartzExeResponse>.Default);
                            }
                            Console.Write(ex.Message);
                        }
                    }
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
