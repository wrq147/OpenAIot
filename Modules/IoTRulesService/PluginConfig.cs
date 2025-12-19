using Common;
using Common.EventBus;
using Common.Share;
using EasyNetQ;
using IoTRulesService.Business;
using IoTRulesService.DAL;
using IoTRulesService.DataParser;
using IoTRulesService.Flow;
using IoTService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MonitorService.Business;
using MonitorService.Model;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.NetCore;


namespace IoTRulesService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService", "IoTService", "MonitorService" };

        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<RuleBLL>();
            services.AddBLL<RuleGroupBLL>();
            services.AddDAL<RuleEventDAL>();
            services.AddDAL<RuleTemplateDAL>();
            services.AddDAL<RuleTriggerDAL>();
            services.AddDAL<RuleGroupDAL>();
            services.AddSingleton<RuleCache>();
            services.AddSingleton<DeviceCache>();
            services.AddSingleton<RuleWheelRuner>();
            services.AddSingleton<MessageRunner>();
            services.AddSingleton<PackParser>();
            services.AddSingleton<DeviceMessageHandler>();
            services.AddRuleflow();
        }

        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            var option = app.ServiceProvider.GetService<IOptions<IotOption>>();
            var generalOption = app.ServiceProvider.GetService<IOptions<GeneralOption>>();

            if (!string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
            {
                TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
                {
                    var bus = app.ServiceProvider.GetService<RabbitScope>().Bus;
                    await bus.PubSub.SubscribeAsync("IotRule", async (string msg) =>
                    {
                        await app.ServiceProvider.GetService<MessageRunner>().ParseExe(msg);
                    }, cfg =>
                    {
                        if (string.IsNullOrEmpty(option.Value.node_name))
                        {
                            cfg.WithTopic("/device.up");
                        }
                        else
                        {
                            cfg.WithTopic("/device.up." + option.Value.node_name);
                        }
                        cfg.WithAutoDelete(true);
                    });

                    await bus.PubSub.SubscribeAsync("IotRule", async (string msg) =>
                    {
                        await app.ServiceProvider.GetService<MessageRunner>().ParseDown(msg);
                    }, cfg =>
                    {
                        if (string.IsNullOrEmpty(option.Value.node_name))
                        {
                            cfg.WithTopic("/device.down");
                        }
                        else
                        {
                            cfg.WithTopic("/device.down." + option.Value.node_name);
                        }
                        cfg.WithAutoDelete(true);
                    });


                    if (Constants.General.quick_init != true)
                    {
                        //添加定时检测节点心跳
                        string heartjobname = "NodeHeartCheck";
                        string heartgroup = "SYSTEM";
                        var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                        if (!await jobBLL.ExistJob(heartjobname, heartgroup))
                        {
                            MZ_Job devjob = new MZ_Job();
                            devjob.concurrent = "0";
                            devjob.createId = 0;
                            devjob.create_time = DateTime.Now;
                            devjob.updateId = 0;
                            devjob.update_time = DateTime.Now;
                            devjob.cron_expression = "0 * * * * ?";
                            devjob.invoke_target = typeof(RuleBLL).FullName + ".ExecuteSendHeartbeat()";
                            devjob.job_group = heartgroup;
                            devjob.job_name = heartjobname;
                            devjob.misfire_policy = "2";
                            devjob.status = "0";

                            await jobBLL.InsertJob(devjob);
                        }
                    }
                });


                //监听规则变更
                TAAsyncHelper.RunSync(async () =>
                {
                    var bus = app.ServiceProvider.GetService<RabbitScope>().Bus;
                    string subid = string.IsNullOrEmpty(option.Value.node_name) ? "HelloWorld" : option.Value.node_name;
                    await bus.PubSub.SubscribeAsync<RuleChangeEvent>(subid, (msg, tk) =>
                    {
                        var tmpCache = app.ServiceProvider.GetService<RuleCache>();
                        if (msg.ChangeType == 1)
                        {
                            if (msg.IsDebug)
                            {
                                tmpCache.StartDebug(msg.RuleId.ToString());
                            }
                            else
                            {
                                tmpCache.StopDebug(msg.RuleId.ToString());
                            }
                        }
                        else
                        {
                            foreach (var ruleItem in msg.Triggers)
                            {
                                tmpCache.Clear(ruleItem.TopicDevice, ruleItem.TopicMsg);
                            }
                        }
                        return Task.CompletedTask;
                    }, cfg =>
                    {

                        cfg.WithTopic(RuleChangeEvent.EventKey);
                        cfg.WithAutoDelete(true);
                    }).ConfigureAwait(false);
                });
            }
            else
            {
                //监听规则变更
                plg.Dispatcher.Register<RuleChangeEvent>(RuleChangeEvent.EventKey, (evt) =>
                {
                    var tmpCache = app.ServiceProvider.GetService<RuleCache>();

                    if (evt.ChangeType == 1)
                    {
                        if (evt.IsDebug)
                        {
                            tmpCache.StartDebug(evt.RuleId.ToString());
                        }
                        else
                        {
                            tmpCache.StopDebug(evt.RuleId.ToString());
                        }
                    }
                    else
                    {
                        foreach (var ruleItem in evt.Triggers)
                        {
                            tmpCache.Clear(ruleItem.TopicDevice, ruleItem.TopicMsg);
                        }
                    }

                    return Task.CompletedTask;
                });
            }

            //监听清除缓存
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                var cache = app.ServiceProvider.GetService<CacheHelper>();
                var bus = app.ServiceProvider.GetService<RabbitScope>().Bus;
                bus.PubSub.Subscribe<string>("IotKeyDel" + Guid.NewGuid().ToString("N"), (msg) =>
                {
                    string tmpkey = msg;
                    if (tmpkey.StartsWith("ProductSys:"))
                    {
                        cache.RemoveCache(tmpkey);
                    }
                    else if (tmpkey.StartsWith("Device:") || tmpkey.StartsWith("Offline:"))
                    {
                        string devid = tmpkey.Split(":")[1];
                        app.ServiceProvider.GetService<DeviceCache>().ClearDevice(devid);
                    }
                    app.ServiceProvider.GetService<PackParser>().DelDevice(msg);
                }, cfg =>
                {
                    cfg.WithTopic("/IotKey.Del");
                    cfg.WithAutoDelete(true);
                });

            });


            //分发执行定时器协议规则
            plg.RegisterTime(async (bs) =>
            {
                await app.ServiceProvider.GetService<RuleBLL>().ExecuteProductTime(bs);
            });


            plg.RegisterQuartzTask();
        }
        public override void Unload(ITAApplication app, PluginObject plg)
        {
            base.Unload(app, plg);
            //程序退出时清理规则节点
            var option = app.ServiceProvider.GetService<IOptions<IotOption>>();
            var serverBus = app.ServiceProvider.GetService<ServerBusProxy>();
            TAAsyncHelper.RunSync(async () =>
            {
                await serverBus.ForcedDownNode(option.Value.node_name).ConfigureAwait(false);
            });

        }


    }
}
