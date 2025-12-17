using ChannelUtility;
using ChannelUtility.Message;
using InfluxDB.Client.Api.Domain;
using IoTRulesService.Flow.Node;
using IoTService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 转发节点
    /// </summary>
    public class RedirectStep : RuleflowStep
    {
        public RedirectProps props { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            if (context.Source.ProductId == props.ProductId)
            {
                await context.Print("错误01,无法转发给触发源自己");
                return;
            }

            foreach (string targetDtuId in props.TargetDtuIdsList)
            {
                string newDeviceId = targetDtuId.Replace("$dtuId", context.Source.DeviceId).Trim();
                var serverBus = context.Provider.GetService<ServerBusProxy>();
                switch (context.Source.MsgType)
                {
                    case "Online":
                        {
                            var oldmsg = (DeviceOnlineMessage)context.Source;
                            if (oldmsg.RedirectFromProductId == context.Source.ProductId)
                            {
                                await context.Print("错误02,无法转发给触发源自己");
                                return;
                            }
                            await serverBus.SendConnect(props.ProductId, newDeviceId, oldmsg.IpAddress, context.Source.ProductId, context.GetStartRuleId(), context.Source.DeviceId);
                            if (context.IsDebug)
                            {
                                await context.Print($"向设备{newDeviceId}转发一条在线消息");
                            }
                        }
                        break;
                    case "Offline":
                        {
                            var oldmsg = (DeviceOfflineMessage)context.Source;
                            if (oldmsg.RedirectFromProductId == context.Source.ProductId)
                            {
                                await context.Print("错误02,无法转发给触发源自己");
                                return;
                            }
                            await serverBus.SendDisconnect(props.ProductId, newDeviceId, context.Source.ProductId, context.GetStartRuleId(), context.Source.DeviceId);
                            if (context.IsDebug)
                            {
                                await context.Print($"向设备{newDeviceId}转发一条离线消息");
                            }
                        }
                        break;
                    case "PropReply":
                        {
                            var oldmsg = (ReadPropertyMessageReply)context.Source;
                            if (oldmsg.RedirectFromProductId == context.Source.ProductId)
                            {
                                await context.Print("错误02,无法转发给触发源自己");
                                return;
                            }
                            if (props.Maping == null)
                            {
                                await context.Print("错误03,请设置转换标识符");
                                return;
                            }
                            else
                            {
                                Dictionary<string, object> newProperties = new Dictionary<string, object>();
                                foreach (var msg in oldmsg.Properties)
                                {
                                    if (props.Maping.TryGetValue(msg.Key, out string newkey))
                                    {
                                        newProperties.TryAdd(newkey, msg.Value);
                                    }
                                }
                                if (newProperties.Count > 0)
                                {
                                    await serverBus.SendPropertyReply(props.ProductId, newDeviceId, newProperties, context.Source.ProductId, false, context.GetStartRuleId(), context.Source.DeviceId);
                                    if (context.IsDebug)
                                    {
                                        await context.Print($"向设备{newDeviceId}转发一条属性消息：" + System.Text.Json.JsonSerializer.Serialize(newProperties, JsonMessageSerializerConfig.SerializeOptions));
                                    }
                                }

                            }
                        }
                        break;
                    case "Event":
                        {
                            var oldmsg = (DeviceEventMessage)context.Source;
                            if (oldmsg.RedirectFromProductId == context.Source.ProductId)
                            {
                                await context.Print("错误02,无法转发给触发源自己");
                                return;
                            }
                            if (props.Maping == null)
                            {
                                await context.Print("错误03,请设置转换标识符");
                                return;
                            }
                            else
                            {
                                string newEventId = null;
                                if (props.Maping.TryGetValue(oldmsg.EventId, out string newkey))
                                {
                                    newEventId = newkey;
                                }
                                if (newEventId != null)
                                {
                                    await serverBus.SendEvent(props.ProductId, newDeviceId, newEventId, oldmsg.Outputs, context.Source.ProductId, context.GetStartRuleId(), context.Source.DeviceId);
                                    if (context.IsDebug)
                                    {
                                        await context.Print($"向设备{newDeviceId}转发一条事件{newEventId}");
                                    }
                                }

                            }

                        }
                        break;
                }
            }
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
