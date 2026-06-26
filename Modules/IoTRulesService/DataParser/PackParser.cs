using ChannelUtility;
using ChannelUtility.Buffers;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using Common;
using Common.EventBus;
using IoTRulesService.DataParser.GraphScript;
using IoTRulesService.DataParser.Js;
using IoTService;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;


namespace IoTRulesService.DataParser
{
    public class PackParser
    {
        private ConcurrentDictionary<string, string> _deviceToNodeGuid = new ConcurrentDictionary<string, string>();
        public void UpdateDeviceGuid(string dtuId, string guid)
        {
            if (_deviceToNodeGuid.TryGetValue(dtuId, out string existingGuid))
            {
                if (existingGuid == guid)
                {
                    return;
                }
            }
            _deviceToNodeGuid.AddOrUpdate(dtuId, guid, (k, v) => guid);
        }
        public string GetNodeGuid(string dtuId)
        {
            if (_deviceToNodeGuid.TryGetValue(dtuId, out string tguid))
            {
                return tguid;
            }
            return null;
        }
        /// <summary>
        /// 拆包、拼包用
        /// </summary>
        protected ConcurrentDictionary<string, FastReader> _lastReaderDict = new ConcurrentDictionary<string, FastReader>();
        private ITAServiceProvider _provider;
        public ITAServiceProvider Provider
        {
            get { return _provider; }
        }
        private ConcurrentDictionary<string, CacheJsEngine> _scriptEngine = new ConcurrentDictionary<string, CacheJsEngine>();
        private IotRedisHelper _iotRedis;
        private ILogger<PackParser> _log;
        public class CacheJsEngine
        {
            public Engine Engine { get; set; }
            public string Script { get; set; }
        }
        public PackParser(ITAServiceProvider provider, IotRedisHelper iotRedis, ILoggerFactory logFactory)
        {
            _provider = provider;
            _iotRedis = iotRedis;
            _log = logFactory.CreateLogger<PackParser>();
        }

        private Engine GetJsEngine(string deviceId, string script)
        {
            var tmpcache = _scriptEngine.GetOrAdd(deviceId, (k) =>
            {
                CacheJsEngine newcache = new CacheJsEngine();
                newcache.Engine = new Engine(option =>
                {
                    option.LimitRecursion(5).LimitMemory(10 * 1024 * 1024).TimeoutInterval(TimeSpan.FromMinutes(5));
                });
                newcache.Engine = newcache.Engine.Execute(script);
                newcache.Script = script;
                return newcache;
            });

            if (!string.Equals(script, tmpcache.Script))
            {
                CacheJsEngine newcache = new CacheJsEngine();
                newcache.Engine = new Engine(option =>
                {
                    option.LimitRecursion(5).LimitMemory(10 * 1024 * 1024).TimeoutInterval(TimeSpan.FromMinutes(5));
                });
                newcache.Engine = newcache.Engine.Execute(script);
                newcache.Script = script;
                _scriptEngine.AddOrUpdate(deviceId, newcache, (key, oldValue) =>
                {
                    return newcache;
                });
                return newcache.Engine;
            }
            else
            {
                return tmpcache.Engine;
            }
        }
        /// <summary>
        /// 将原二进制转FastReader
        /// </summary>
        /// <param name="input"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private FastReader BytesToReader(byte[] input, string deviceId)
        {
            FastReader lastReader;
            if (_lastReaderDict.TryRemove(deviceId, out lastReader))
            {
                return lastReader.Contact(input);
            }
            else
            {
                return new FastReader(input);
            }
        }

        /// <summary>
        /// 保存剩余
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="lastReader"></param>
        private void SaveFastReader(string deviceId, FastReader lastReader)
        {
            _lastReaderDict.AddOrUpdate(deviceId, lastReader, (key, existv) => lastReader);
        }
        /// <summary>
        /// 重置事件周期
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task ResetSilenceTime(string deviceId, string code)
        {
            await _iotRedis.KeyDeleteAsync("IotQuick::" + deviceId + "::" + code).ConfigureAwait(false);
        }
        /// <summary>
        /// 发送确认回复包（同步）
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        public async Task ConfirmReply(string msgId, BaseUpDeviceMessage msg)
        {
            if (string.IsNullOrEmpty(msgId))
            {
                await _provider.GetService<DeviceMessageHandler>().ExeMessage(msg).ConfigureAwait(false);
            }
            else
            {
                var bus = _provider.GetService<NatsScope>().Bus;
                string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
                await bus.PublishAsync(new NatsMsg<string>()
                {
                    Subject = msgId,
                    Data = msgbody
                }, DefalutNatsJsonSerializer<string>.Default);
            }
        }
        public async Task StartReadAllMessage(string productId, string deviceId, List<string> props)
        {
            StartReadAllMessage msg = new StartReadAllMessage();
            msg.ProductId = productId;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.props = props;
            await _provider.GetService<DeviceMessageHandler>().ExeMessage(msg);
        }
        private async Task<T> WaitDownPackage<I, T>(I msg) where I : BaseDeviceMessage where T : BaseUpDeviceMessage
        {
            INatsSub<T> resSub = null;
            try
            {
                TslReturn ret = null;
                if (string.IsNullOrEmpty(msg.ProductId))
                {
                    ret = await TslCache.GetTslModelByDtuId(msg.DeviceId, false, _provider).ConfigureAwait(false);
                    msg.ProductId = ret.ProductId;
                }
                else
                {
                    ret = await TslCache.GetTslModel(msg.ProductId, _provider).ConfigureAwait(false);
                }

                var bus = _provider.GetService<NatsScope>().Bus;
                var requestTimeout = TimeSpan.FromSeconds(5);
                resSub = await bus.SubscribeCoreAsync<T>(msg.MessageId, null, DefalutNatsJsonSerializer<T>.Default, new NatsSubOpts
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    ThrowIfNoResponders = true
                }).ConfigureAwait(false);

                string tnodeguid = GetNodeGuid(msg.DeviceId);
                if (tnodeguid == null)
                {
                    throw new TimeoutException($"通道节点 {tnodeguid} 不存在");
                }
                string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
                await bus.PublishAsync("node." + tnodeguid, msgbody, null, msg.MessageId, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);

                await foreach (var responseMsg in resSub.Msgs.ReadAllAsync().ConfigureAwait(false))
                {
                    return responseMsg.Data;
                }
                throw new TimeoutException($"等待 {requestTimeout.TotalSeconds} 秒后未收到回复");

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return null;
            }
            finally
            {
                if (resSub != null)
                {
                    await resSub.DisposeAsync();
                }
            }
        }
        public async Task PublicWaitReadProperty(ReadPropertyMessage msg, Func<ReadPropertyMessageReply, Task> ac)
        {
            await StartReadAllMessage(msg.ProductId, msg.DeviceId, msg.Properties);
            var reply = await WaitDownPackage<ReadPropertyMessage, ReadPropertyMessageReply>(msg).ConfigureAwait(false);
            await ac.Invoke(reply);
        }

        public async Task<FunctionInvokeMessageReply> PublicWaitFuncReply(FunctionInvokeMessage msg)
        {
            var reply = await WaitDownPackage<FunctionInvokeMessage, FunctionInvokeMessageReply>(msg).ConfigureAwait(false);
            return reply;
        }
        /// <summary>
        /// 向前端发送功能命令
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public async Task FunCmd(string devId, string cmd)
        {
            List<string> data = new List<string>();
            data.Add("newfun/" + devId);
            data.Add(cmd);
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<List<string>>()
            {
                Subject = "MqttNotice.Msg",
                Data = data
            }, DefalutNatsJsonSerializer<List<string>>.Default).ConfigureAwait(false);
        }
        /// <summary>
        /// 获取指定设备的当前所有属性
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetProps(string deviceId)
        {
            var tkey = "Device:" + deviceId;
            var redisdict = await _iotRedis.HashGetAllAsync<string>(tkey).ConfigureAwait(false);
            if (redisdict != null && redisdict.Count > 0)
            {
                var dcit = DevicePropertyValue.FromDictStr(redisdict);
                return DevicePropertyValue.ToDict(dcit);
            }
            return new Dictionary<string, object>();
        }

        /// <summary>
        /// 获取指定设备的指定属性
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> GetPropOfCode(string deviceId, string code)
        {
            var tkey = "Device:" + deviceId;
            var redisstr = await _iotRedis.HashGetAsync<string>(tkey, code).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(redisstr))
            {
                return redisstr;
            }
            return null;
        }
        /// <summary>
        /// 获取指定设备的原dtuid
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<string> GetRawDtuId(string deviceId)
        {
            var tkey = "Device:" + deviceId;
            var redisstr = await _iotRedis.HashGetAsync<string>(tkey, "$rawDtuId").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(redisstr))
            {
                return redisstr;
            }
            return null;
        }
        /// <summary>
        /// 获取当前设备的源协议Id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<string> GetRawProductId(string deviceId)
        {
            var tkey = "Device:" + deviceId;
            var redisstr = await _iotRedis.HashGetAsync<string>(tkey, "$rawProductId").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(redisstr))
            {
                return redisstr;
            }
            return null;
        }

        public async Task Print(string devId, string tip, object msg)
        {
            List<string> data = new List<string>();
            data.Add("console/" + devId);
            data.Add(tip + ":" + System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.SerializeOptions));
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<List<string>>()
            {
                Subject = "MqttNotice.Msg",
                Data = data
            }, DefalutNatsJsonSerializer<List<string>>.Default).ConfigureAwait(false);
        }

        /// <summary>
        /// 写入回复数据
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="value"></param>
        /// <param name="msgId">如果msgId为null,则自动取系统保存的第一条msgId</param>
        /// <returns></returns>
        public async Task PushReply(string deviceId, string value, string msgId = null)
        {
            if (string.IsNullOrEmpty(msgId))
            {
                msgId = await _iotRedis.ListLeftPopAsync<string>($"DeviceMsgId:{deviceId}").ConfigureAwait(false);
            }
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = value
            }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        public async Task<string> WaitOnly(string deviceId, string msgId)
        {
            await _iotRedis.ListRightPushAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);
            try
            {
                var requestTimeout = TimeSpan.FromSeconds(5);
                var bus = _provider.GetService<NatsScope>().Bus;
                await foreach (var msg in bus.SubscribeAsync(msgId, null, DefalutNatsJsonSerializer<string>.Default, new NatsSubOpts
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    ThrowIfNoResponders = true
                }).ConfigureAwait(false))
                {
                    //清除系统消息Id
                    await _iotRedis.ListRemoveAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);
                    return msg.Data;
                }
                throw new TimeoutException($"等待 {requestTimeout.TotalSeconds} 秒后未收到回复");
            }
            catch (Exception ex)
            {
                try
                {
                    await _iotRedis.KeyDeleteAsync($"DeviceMsgId:{deviceId}").ConfigureAwait(false);
                }
                catch
                {
                    Console.Write(ex.Message);
                }
                await Print(deviceId, "异常", "未收到回复消息").ConfigureAwait(false);
                _log.LogError(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 获取设备指定类型的返回值
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="msgId"></param>
        /// <param name="ac"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<string> PublicWait(string deviceId, string msgId, Func<Task> ac, Action<string> callback = null)
        {
            if (msgId == null)
            {
                msgId = Guid.NewGuid().ToString("N");
            }

            await _iotRedis.ListRightPushAsync($"DeviceMsgId:{deviceId}", msgId);
            INatsSub<string> resSub = null;
            try
            {
                var bus = _provider.GetService<NatsScope>().Bus;
                var requestTimeout = TimeSpan.FromSeconds(5);
                resSub = await bus.SubscribeCoreAsync<string>(msgId, null, DefalutNatsJsonSerializer<string>.Default, new NatsSubOpts
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    ThrowIfNoResponders = true
                }).ConfigureAwait(false);

                await ac.Invoke();

                await foreach (var responseMsg in resSub.Msgs.ReadAllAsync().ConfigureAwait(false))
                {
                    //清除系统消息Id
                    await _iotRedis.ListRemoveAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);
                    callback?.Invoke(responseMsg.Data);
                    return responseMsg.Data;
                }
                throw new TimeoutException($"等待 {requestTimeout.TotalSeconds} 秒后未收到回复");

            }
            catch (Exception ex)
            {
                try
                {
                    //清除系统消息Id
                    await _iotRedis.ListRemoveAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);
                }
                catch
                {
                    Console.Write(ex.Message);
                }
                await Print(deviceId, "异常", "下发的消息无回复").ConfigureAwait(false);
                _log.LogError(ex.Message);
                callback?.Invoke(null);
                return null;
            }
            finally
            {
                if (resSub != null)
                {
                    await resSub.DisposeAsync();
                }
            }
        }

        public async Task DownModbusMatch(string deviceId, string nodeguid, List<ModbusMatch> list)
        {
            ModbusMatchMessage msg = new ModbusMatchMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = string.Empty;
            msg.MatchList = list;

            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "node." + nodeguid,
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        /// <summary>
        /// 直接调用下发消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task PublicMessage(BaseDeviceMessage msg, TslReturn ret)
        {
            if (ret == null)
            {
                if (string.IsNullOrEmpty(msg.ProductId))
                {
                    ret = await TslCache.GetTslModelByDtuId(msg.DeviceId, false, _provider).ConfigureAwait(false);
                    msg.ProductId = ret.ProductId;
                }
                else
                {
                    ret = await TslCache.GetTslModel(msg.ProductId, _provider).ConfigureAwait(false);
                }
            }


            if (msg is RawDataMessage rawdata)
            {
                //未发布打印
                if (ret.Status == "0")
                {
                    await Print(msg.DeviceId, "设备下发消息", FastBufferHelper.ByteToHexStr(rawdata.Data));
                }
                string tnodeguid = GetNodeGuid(msg.DeviceId);
                if (tnodeguid == null)
                {
                    return;
                }

                string msgbody = System.Text.Json.JsonSerializer.Serialize(rawdata, JsonMessageSerializerConfig.DefaultOptions);
                var bus = _provider.GetService<NatsScope>().Bus;
                await bus.PublishAsync(new NatsMsg<string>()
                {
                    Subject = "node." + tnodeguid,
                    Data = msgbody
                }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
                return;
            }


            var newmsg = await this.toRawData(msg, ret).ConfigureAwait(false);
            if (newmsg != null)
            {
                string tnodeguid = GetNodeGuid(msg.DeviceId);
                if (tnodeguid == null)
                {
                    return;
                }
                var bus = _provider.GetService<NatsScope>().Bus;
                string msgbody = System.Text.Json.JsonSerializer.Serialize(newmsg, JsonMessageSerializerConfig.DefaultOptions);

                await bus.PublishAsync(new NatsMsg<string>()
                {
                    Subject = "node." + tnodeguid,
                    Data = msgbody
                }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
            }
            else
            {
                if (msg is DeviceBindMessage bindMessage)
                {
                    await this.ConfirmBindReply(bindMessage.ProductId, bindMessage.DeviceId, true, bindMessage.MessageId).ConfigureAwait(false);
                }
            }

        }

        /// <summary>
        /// 发送设备功能回复给事件总线
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="isSuccess"></param>
        /// <param name="outputs"></param>
        /// <param name="errInfo"></param>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public async Task ConfirmFuncReply(string productId, string deviceId, bool isSuccess, IDictionary<string, object> outputs, string errInfo, string msgId)
        {
            FunctionInvokeMessageReply msg = new FunctionInvokeMessageReply();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.MessageId = msgId;
            msg.IsSuccess = isSuccess;
            msg.Error = errInfo;
            msg.Outputs = outputs;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        /// <summary>
        /// 发送设备绑定回复
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="isSuccess"></param>
        /// <param name="msgId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task ConfirmBindReply(string productId, string deviceId, bool isSuccess, string msgId, string reason = "")
        {
            DeviceBindMessageReply msg = new DeviceBindMessageReply();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.IsSuccess = isSuccess;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.Reason = reason;

            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        /// <summary>
        /// 解释自定义数据包
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="model"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        private byte[] ParseCustom(BaseDeviceMessage msg, TslModel model, string script)
        {
            if (!string.IsNullOrEmpty(script))
            {
                var context = new MessageContext(msg, this, model, null);
                try
                {
                    var jsEngine = this.GetJsEngine(msg.DeviceId, script);
                    var tmp = jsEngine.Invoke("toRawData", JsValue.FromObject(jsEngine, context));
                    if (tmp.IsNull())
                    {
                        return null;
                    }
                    var payload = tmp.As<ObjectWrapper>().Target as FastWriter;
                    if (payload == null)
                    {
                        return null;
                    }
                    return payload.ToArray();
                }
                catch (JavaScriptException ex)
                {
                    var location = ex.Location;
                    Print(msg.DeviceId, "toRawData执行错误", string.Format("在行{0}至行{1}发生异常:{2}", location.Start.Line, location.End.Line, ex.Message)).ConfigureAwait(false);
                    return null;
                }
                catch (Exception ex)
                {
                    Print(msg.DeviceId, "toRawData异常", "脚本未知错误" + ex.Message).ConfigureAwait(false);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 解释功能脚本
        /// </summary>
        /// <param name="funcScript"></param>
        /// <param name="msg"></param>
        /// <param name="model"></param>
        /// <param name="funprefix"></param>
        /// <returns></returns>
        private void ParseFunc(string funcScript, FunctionInvokeMessage msg, TslModel model, string funprefix)
        {
            var context = new FuncMessageContext(msg, this, model, funprefix);
            try
            {
                Task.Run(() =>
                {
                    var jsEngine = new Engine(option =>
                    {
                        option.LimitRecursion(5).TimeoutInterval(TimeSpan.FromMinutes(10));
                    });
                    jsEngine.Execute(funcScript).Invoke("exeFunc", JsValue.FromObject(jsEngine, context));
                }).ConfigureAwait(false);
            }
            catch (JavaScriptException ex)
            {
                var location = ex.Location;
                Print(msg.DeviceId, "exeFunc执行错误", string.Format("在行{0}至行{1}发生异常:{2}", location.Start.Line, location.End.Line, ex.Message)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Print(msg.DeviceId, "exeFunc异常", "脚本未知错误" + ex.Message).ConfigureAwait(false);
            }
        }



        /// <summary>
        /// 发送自定义数据包
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="properties"></param>
        /// <param name="input"></param>
        /// <param name="script"></param>
        /// <param name="model"></param>
        /// <param name="codeprefix"></param>
        /// <param name="nodeGuid"></param>
        /// <returns></returns>
        private bool PushCustom(string productId, string deviceId, IDictionary<string, object> properties, FastReader input, string script, TslModel model, string codeprefix, string nodeGuid)
        {
            ReadPropertyMessageReply msg = null;
            if (properties != null && properties.Count > 0)
            {
                msg = new ReadPropertyMessageReply();
                msg.DeviceId = deviceId;
                msg.ProductId = productId;
                msg.Properties = properties;
                msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                msg.RedirectFromProductId = string.Empty;
                msg.IsTagSync = false;
                msg.NodeId = nodeGuid;
            }

            if (string.IsNullOrEmpty(script))
            {
                if (msg != null)
                {
                    _provider.GetService<DeviceMessageHandler>().ExeMessage(msg).ConfigureAwait(false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            try
            {
                var datacontext = new DataContext(msg, input, productId, deviceId, this, model, codeprefix, nodeGuid);
                var jsEngine = this.GetJsEngine(deviceId, script);
                var context = JsValue.FromObject(jsEngine, datacontext);
                var func = jsEngine.Invoke("rawDataTo", context);

                if (func.IsNull())
                {
                    if (msg != null)
                    {
                        _provider.GetService<DeviceMessageHandler>().ExeMessage(msg).ConfigureAwait(false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                var newmsg = func.As<ObjectWrapper>().Target as BaseUpDeviceMessage;
                if (newmsg == null)
                {
                    if (msg != null)
                    {
                        _provider.GetService<DeviceMessageHandler>().ExeMessage(msg).ConfigureAwait(false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                newmsg.NodeId = nodeGuid;
                if (newmsg is EmptyMessageReply)
                {
                    return true;
                }
                _provider.GetService<DeviceMessageHandler>().ExeMessage(newmsg).ConfigureAwait(false);
                return true;
            }
            catch (JavaScriptException ex)
            {
                var location = ex.Location;
                Print(deviceId, "rawDataTo执行错误", string.Format("在行{0}至行{1}发生异常:{2}", location.Start.Line, location.End.Line, ex.Message)).ConfigureAwait(false);
                return false;
            }
            catch (Exception ex)
            {
                Print(deviceId, "rawDataTo异常", "脚本未知错误" + ex.Message).ConfigureAwait(false);
                return false;
            }
        }

        public async Task rawDataTo(RawUpDataMessage datamsg, bool sendconn = true)
        {
            try
            {
                List<ModbusMatch> newmmlist = null;
                //获取物模型，防止在线设备未添加
                var ret = await TslCache.GetTslModelByDtuId(datamsg.DeviceId, sendconn, _provider);
                if (ret == null)
                {
                    await Print(datamsg.DeviceId, "设备上报消息", "尝试初始化失败，请绑定设备编码后重启您的设备");
                    return;
                }
                if (ret.Status == "0")
                {
                    if (datamsg.Data.Length > 4096)
                    {
                        await Print(datamsg.DeviceId, "设备上报消息", "因数据超过4096字节，无法在控制台显示");
                    }
                    else
                    {
                        await Print(datamsg.DeviceId, "设备上报消息", FastBufferHelper.ByteToHexStr(datamsg.Data));
                    }
                }
                if (ret.Model == null)
                {
                    await Print(datamsg.DeviceId, "设备上报消息", "物模型不存在");
                    return;
                }
                var tsl = ret.Model;
                string productId = ret.ProductId;
                bool isCute = false;
                FastReader lastReader = BytesToReader(datamsg.Data, datamsg.DeviceId);
                while (!lastReader.EndOfBuffer)
                {
                    Dictionary<string, object> propsDict = null;
                    if (lastReader.Length > 5 && tsl.modbus != null)
                    {
                        int mmIdx = -1;
                        bool needcrc = true;
                        bool iscc = true;
                        if (tsl.modbus.Mode == "TCP")
                        {
                            needcrc = false;
                            mmIdx = lastReader.ReadUInt16BE();
                            lastReader.ReadUInt16BE();
                            int mmlen = lastReader.ReadUInt16BE();
                            if (lastReader.Length < (mmlen + 4))
                            {
                                lastReader.MergeRead();
                                iscc = false;
                            }
                        }

                        if (iscc == true)
                        {
                            byte slaveAddress = lastReader.ReadByte();
                            byte funcByte = lastReader.ReadByte();
                            if (funcByte == 5 || funcByte == 6 || funcByte == 15 || funcByte == 16)
                            {
                                #region 解释modbus写回复
                                if (lastReader.Length >= 8)
                                {
                                    ushort startAddress = lastReader.ReadUInt16BE();
                                    lastReader.ReadBytes(2);
                                    bool crcrs = true;
                                    if (needcrc)
                                    {
                                        ushort u = lastReader.ReadUInt16LE();
                                        ushort crc;
                                        if (tsl.modbus.Mode == "RTU")
                                        {
                                            crc = FastBufferHelper.CalcCRC16(lastReader.ToArray(), 0, 6);
                                        }
                                        else
                                        {
                                            crc = FastBufferHelper.CalcLRC(lastReader.ToArray(), 0, 6);
                                        }
                                        if (u != crc)
                                        {
                                            crcrs = false;
                                        }
                                    }
                                    if (crcrs)
                                    {
                                        string callkey = "Func#" + slaveAddress + "#" + funcByte + "#" + startAddress + "#" + datamsg.prefix;
                                        await PushReply(datamsg.DeviceId, "ok", callkey);
                                        if (newmmlist == null)
                                        {
                                            newmmlist = new List<ModbusMatch>();
                                        }
                                        newmmlist.Add(new ModbusMatch()
                                        {
                                            SlaveId = slaveAddress,
                                            Name = "Func",
                                            FuncCode = funcByte,
                                            StartAddress = startAddress
                                        });
                                    }
                                    else
                                    {
                                        lastReader.Reset();
                                    }
                                }
                                else
                                {
                                    lastReader.MergeRead();
                                }
                                #endregion
                            }
                            else if (funcByte > 0 && funcByte < 7)
                            {
                                byte bdlen = lastReader.ReadByte();
                                //校验长度
                                if ((lastReader.Length - 5) >= bdlen)
                                {
                                    var body = new FastReader(lastReader.ReadBytes(bdlen));
                                    int tcount = lastReader.Position + 1;

                                    bool crcrs = true;
                                    if (needcrc)
                                    {
                                        ushort u = lastReader.ReadUInt16LE();
                                        ushort crc;
                                        if (tsl.modbus.Mode == "RTU")
                                        {
                                            crc = FastBufferHelper.CalcCRC16(lastReader.ToArray(), 0, tcount);
                                        }
                                        else
                                        {
                                            crc = FastBufferHelper.CalcLRC(lastReader.ToArray(), 0, tcount);
                                        }
                                        //校验码验证
                                        if (u != crc)
                                        {
                                            crcrs = false;
                                        }
                                    }
                                    if (crcrs)
                                    {
                                        propsDict = new Dictionary<string, object>();
                                        #region 开始解释modbus读回复
                                        if (mmIdx >= 0 && mmIdx < tsl.modbus.Matches.Count)
                                        {
                                            if (newmmlist == null)
                                            {
                                                newmmlist = new List<ModbusMatch>();
                                            }
                                            newmmlist.Add(tsl.modbus.Matches[mmIdx]);
                                        }
                                        else
                                        {
                                            newmmlist = tsl.modbus.Matches.Where(x => x.SlaveId == slaveAddress && x.FuncCode == funcByte && x.GetByteLength() == bdlen).ToList();
                                        }

                                        if (newmmlist == null || newmmlist.Count == 0)
                                        {
                                            lastReader.Reset();
                                            await Print(datamsg.DeviceId, "设备上报消息", $"modbus无匹配规则:{FastBufferHelper.ByteToHexStr(lastReader.ReadToEnd())}");
                                            return;
                                        }

                                        foreach (var matchItem in newmmlist)
                                        {
                                            body.Reset();
                                            foreach (var prop in matchItem.Items)
                                            {
                                                if (string.IsNullOrEmpty(prop.PropertyCode))
                                                {
                                                    body.ReadBytes(prop.GetRegisterLen());
                                                    continue;
                                                }

                                                var prpitem = tsl.properties.Where(x => x.code == prop.PropertyCode).FirstOrDefault();
                                                if (prpitem == null)
                                                {
                                                    body.ReadBytes(prop.GetRegisterLen());
                                                    continue;
                                                }

                                                if (!string.IsNullOrEmpty(prpitem.prefixcode) && prpitem.prefixcode != datamsg.prefix)
                                                {
                                                    body.ReadBytes(prop.GetRegisterLen());
                                                    continue;
                                                }


                                                if (prop.NumRegister == "b")
                                                {
                                                    int tmpbit = body.ReadBitLE();
                                                    if (propsDict.ContainsKey(prop.PropertyCode))
                                                    {
                                                        propsDict[prop.PropertyCode] = tmpbit;
                                                    }
                                                    else
                                                    {
                                                        propsDict.Add(prop.PropertyCode, tmpbit);
                                                    }
                                                }
                                                else
                                                {
                                                    if (prop.ByteOrder == "C")
                                                    {
                                                        byte[] tmpb = body.ReadBytes(prop.GetRegisterLen());
                                                        if (propsDict.ContainsKey(prop.PropertyCode))
                                                        {
                                                            propsDict[prop.PropertyCode] = ASCIIEncoding.ASCII.GetString(tmpb);
                                                        }
                                                        else
                                                        {
                                                            propsDict.Add(prop.PropertyCode, ASCIIEncoding.ASCII.GetString(tmpb));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        int datalen = prop.GetRegisterLen();
                                                        if (datalen == 1)
                                                        {
                                                            byte tmpb = body.ReadByte();
                                                            if (propsDict.ContainsKey(prop.PropertyCode))
                                                            {
                                                                propsDict[prop.PropertyCode] = tmpb;
                                                            }
                                                            else
                                                            {
                                                                propsDict.Add(prop.PropertyCode, tmpb);
                                                            }

                                                        }
                                                        else if (datalen == 2)
                                                        {
                                                            short tmps;
                                                            if (prop.ByteOrder == "H")
                                                            {
                                                                tmps = body.ReadInt16BE();
                                                            }
                                                            else
                                                            {
                                                                tmps = body.ReadInt16LE();
                                                            }

                                                            if (propsDict.ContainsKey(prop.PropertyCode))
                                                            {
                                                                propsDict[prop.PropertyCode] = tmps;
                                                            }
                                                            else
                                                            {
                                                                propsDict.Add(prop.PropertyCode, tmps);
                                                            }
                                                        }
                                                        else if (datalen == 4)
                                                        {
                                                            int tmpi;
                                                            switch (prop.ByteOrder)
                                                            {
                                                                case "L":
                                                                    tmpi = body.ReadInt32LE();
                                                                    break;
                                                                case "CDAB":
                                                                    tmpi = body.ReadInt32CDAB();
                                                                    break;
                                                                case "BADC":
                                                                    tmpi = body.ReadInt32BADC();
                                                                    break;
                                                                default:
                                                                    tmpi = body.ReadInt32BE();
                                                                    break;
                                                            }

                                                            if (propsDict.ContainsKey(prop.PropertyCode))
                                                            {
                                                                propsDict[prop.PropertyCode] = tmpi;
                                                            }
                                                            else
                                                            {
                                                                propsDict.Add(prop.PropertyCode, tmpi);
                                                            }

                                                        }
                                                        else if (datalen == 8)
                                                        {
                                                            long tmpi;
                                                            switch (prop.ByteOrder)
                                                            {
                                                                case "L":
                                                                    tmpi = body.ReadInt64LE();
                                                                    break;
                                                                default:
                                                                    tmpi = body.ReadInt64BE();
                                                                    break;
                                                            }

                                                            if (propsDict.ContainsKey(prop.PropertyCode))
                                                            {
                                                                propsDict[prop.PropertyCode] = tmpi;
                                                            }
                                                            else
                                                            {
                                                                propsDict.Add(prop.PropertyCode, tmpi);
                                                            }

                                                        }
                                                    }
                                                }

                                            }
                                        }

                                        #endregion


                                    }
                                    else
                                    {
                                        lastReader.Reset();
                                    }
                                }
                                else
                                {
                                    lastReader.MergeRead();
                                }
                            }
                            else
                            {
                                lastReader.Reset();
                            }
                        }

                    }

                    //自定义解释
                    PushCustom(productId, datamsg.DeviceId, propsDict, lastReader, ret.script, tsl, datamsg.prefix, datamsg.NodeId);
                    if (lastReader.Position > -1)
                    {
                        //有剩余数据包，则下个循环处理
                        if (!lastReader.EndOfBuffer)
                        {
                            lastReader = lastReader.CopyTo(lastReader.Position + 1);
                            isCute = true;
                        }
                    }
                    else
                    {
                        if (lastReader.IsMergeRead || isCute)
                        {
                            //保存下次使用
                            SaveFastReader(datamsg.DeviceId, lastReader);
                        }
                        if (newmmlist != null && newmmlist.Count > 0 && datamsg.IsReturn)
                        {
                            await DownModbusMatch(datamsg.DeviceId, datamsg.NodeId, newmmlist);
                        }
                        return;
                    }
                }

                if (newmmlist != null && newmmlist.Count > 0 && datamsg.IsReturn)
                {
                    await DownModbusMatch(datamsg.DeviceId, datamsg.NodeId, newmmlist);
                }
            }
            catch (Exception ex)
            {
                await Print(datamsg.DeviceId, "rawDataTo异常", ex.Message);
            }
        }
        public async Task<RawDataMessage> toRawData(BaseDeviceMessage msg, TslReturn ret)
        {

            if (msg is ModbusMessage modbusMessage)
            {
                var tsl = ret.Model;
                if (tsl.modbus == null)
                {
                    throw new Exception("modbus未配置");
                }
                int searchIdx = 0;
                ModbusMatch matchItem = null;
                foreach (var tmpitem in tsl.modbus.Matches)
                {
                    if (tmpitem.Name == modbusMessage.MatchName)
                    {
                        matchItem = tmpitem;
                        break;
                    }
                    ++searchIdx;
                }
                if (matchItem == null)
                {
                    throw new Exception("modbus无匹配规则");
                }

                byte[] data = FastBufferHelper.ModbusMatch2Bytes(tsl.modbus.Mode, matchItem, searchIdx);
                RawDataMessage rawdata = new RawDataMessage();
                rawdata.DeviceId = msg.DeviceId;
                rawdata.MessageId = msg.MessageId;
                rawdata.ProductId = msg.ProductId;
                var items = matchItem.Items.Where(x => !string.IsNullOrEmpty(x.PropertyCode)).ToList();
                if (items.Count > 0)
                {
                    var tmpfff = tsl.properties.Where(x => x.code == items[0].PropertyCode).FirstOrDefault();
                    if (tmpfff != null)
                    {
                        rawdata.prefix = tmpfff.prefixcode;
                    }
                }

                rawdata.Data = data;
                return rawdata;
            }
            else if (msg is ReadPropertyMessage proMsg)
            {
                if (proMsg.Properties == null)
                {
                    return null;
                }
                var hs = proMsg.Properties.ToHashSet();
                List<ModbusMatch> retmmList = new List<ModbusMatch>();
                List<int> retIdx = new List<int>();
                if (ret.Model.modbus != null)
                {
                    var mm = ret.Model.modbus.Matches;
                    for (int i = 0; i < mm.Count; i++)
                    {
                        var mitem = mm[i];
                        foreach (var mmi in mitem.Items)
                        {
                            if (hs.Contains(mmi.PropertyCode))
                            {
                                retmmList.Add(mitem);
                                retIdx.Add(i);
                                break;
                            }
                        }
                    }
                }

                if (retmmList.Count == 0)
                {
                    RawDataMessage rawdata = new RawDataMessage();
                    rawdata.DeviceId = msg.DeviceId;
                    rawdata.MessageId = msg.MessageId;
                    rawdata.ProductId = msg.ProductId;
                    rawdata.Data = ParseCustom(msg, ret.Model, ret.script);
                    if (rawdata.Data == null)
                    {
                        return null;
                    }
                    return rawdata;
                }
                else
                {
                    //存在匹配的modbus指令，则下发modbus指令
                    for (int x = 0; x < retmmList.Count; x++)
                    {
                        RawDataMessage rawdata = new RawDataMessage();
                        rawdata.DeviceId = msg.DeviceId;
                        rawdata.MessageId = msg.MessageId + "_" + x;
                        rawdata.ProductId = msg.ProductId;
                        rawdata.Data = FastBufferHelper.ModbusMatch2Bytes(ret.Model.modbus.Mode, retmmList[x], retIdx[x]);
                        var firstmm = retmmList[x].Items.Where(y => !string.IsNullOrEmpty(y.PropertyCode)).FirstOrDefault();
                        if (firstmm != null)
                        {
                            var tmpfff = ret.Model.properties.Where(y => y.code == firstmm.PropertyCode).FirstOrDefault();
                            if (tmpfff != null)
                            {
                                rawdata.prefix = tmpfff.prefixcode;
                            }
                        }
                        await this.PublicMessage(rawdata, ret);
                    }
                    return null;
                }
            }
            else if (msg is FunctionInvokeMessage funcMessage)
            {
                string comparefun = funcMessage.DeviceId + "|" + funcMessage.FunctionId;
                if (!string.IsNullOrEmpty(funcMessage.SourceFunction) && funcMessage.SourceFunction == comparefun)
                {
                    await this.Print(msg.DeviceId, "功能调用异常", $"无法死循环调用同一个功能{funcMessage.FunctionId}");
                    return null;
                }
                RawDataMessage rawdata = new RawDataMessage();
                rawdata.DeviceId = msg.DeviceId;
                rawdata.MessageId = msg.MessageId;
                rawdata.ProductId = msg.ProductId;
                if (ret.Status == "0")
                {
                    await this.Print(msg.DeviceId, "解释下发消息", $"开始执行功能{funcMessage.FunctionId}");
                }
                var funModel = ret.Model.functions.Where(x => x.code == funcMessage.FunctionId).FirstOrDefault();
                if (funModel == null)
                {
                    throw new Exception($"物模型未配置功能：{funcMessage.FunctionId}");
                }
                rawdata.prefix = funModel.prefixcode;
                if (funModel.downway == 1)
                {
                    IDictionary<string, object> inputs = funModel.CreateInputs();
                    foreach (var kvp in funcMessage.Inputs)
                    {
                        if (inputs.ContainsKey(kvp.Key))
                        {
                            inputs[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            inputs.Add(kvp);
                        }
                    }
                    try
                    {
                        rawdata.Data = funModel.GetModbusBytes(ret.Model.modbus.Mode, inputs);
                    }
                    catch (Exception funEx)
                    {
                        await this.Print(funcMessage.DeviceId, "modbus异常", funEx.Message);
                        await this.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, false, null, funEx.Message, funcMessage.MessageId);
                        return null;
                    }
                    if (rawdata.Data != null && !string.IsNullOrEmpty(msg.MessageId))
                    {
                        var tmmm = funModel.GetModbusMatch();
                        string callkey = "Func#" + tmmm.SlaveId + "#" + tmmm.FuncCode + "#" + tmmm.StartAddress + "#" + funModel.prefixcode;
                        if (funModel.waitreturn == true)
                        {
                            _ = this.PublicWait(funcMessage.DeviceId, callkey, async () =>
                            {
                                //未发布打印
                                if (ret.Status == "0")
                                {
                                    await this.Print(funcMessage.DeviceId, "解释下发消息", FastBufferHelper.ByteToHexStr(rawdata.Data));
                                }

                                await this.PublicMessage(rawdata, ret);
                            },async (rss) =>
                            {
                                await this.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, true, null, string.Empty, funcMessage.MessageId);
                            });
                        }
                        else
                        {
                            await this.PublicMessage(rawdata, ret);
                            await this.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, true, null, string.Empty, funcMessage.MessageId);
                        }


                        return null;
                    }
                    else
                    {
                        return rawdata;
                    }
                }
                else if (funModel.downway == 2)
                {
                    var mmidx = ret.Model.modbus.Matches.FindIndex(x => x.Name == funModel.downdata);
                    if (mmidx == -1)
                    {
                        return null;
                    }
                    var mm = ret.Model.modbus.Matches[mmidx];
                    try
                    {
                        rawdata.Data = FastBufferHelper.ModbusMatch2Bytes(ret.Model.modbus.Mode, mm, mmidx);
                    }
                    catch (Exception funEx)
                    {
                        await this.Print(funcMessage.DeviceId, "modbus异常", funEx.Message);
                        await this.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, false, null, funEx.Message, funcMessage.MessageId);
                        return null;
                    }
                    await this.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, true, null, string.Empty, funcMessage.MessageId);
                    return rawdata;
                }
                else if (funModel.downway == 3)
                {
                    var props = System.Text.Json.JsonSerializer.Deserialize<List<string>>(funModel.downdata);
                    var propslist = props.ToList();
                    propslist.Sort();
                    ReadPropertyMessage newmsg = new ReadPropertyMessage();
                    newmsg.DeviceId = msg.DeviceId;
                    newmsg.ProductId = msg.ProductId;
                    newmsg.Properties = propslist;
                    newmsg.MessageId = $"Rd{msg.DeviceId}-{propslist.Count}-{UtilityTool.MD5(string.Join('#', props))}";
                    _ = PublicWaitReadProperty(newmsg, async rpm =>
                    {
                        if (rpm == null)
                        {
                            await this.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, false, null, "直接读属性执行失败", funcMessage.MessageId);
                        }
                        else
                        {
                            await this.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, true, rpm.Properties, string.Empty, funcMessage.MessageId);
                        }
                    });
                    return null;
                }
                else if (funModel.downway == 4)
                {
                    _ = Task.Run(() =>
                    {
                        var context = new FuncMessageContext(msg, this, ret.Model, rawdata.prefix);
                        LiteGraphParser.Instance.Run(context, funModel.downdata);
                    });
                    return null;
                }
                else
                {
                    ParseFunc(funModel.downdata, funcMessage, ret.Model, rawdata.prefix);
                    return null;
                }
            }
            else
            {
                var data = ParseCustom(msg, ret.Model, ret.script);
                if (data != null)
                {
                    RawDataMessage rawdata = new RawDataMessage();
                    rawdata.DeviceId = msg.DeviceId;
                    rawdata.MessageId = msg.MessageId;
                    rawdata.ProductId = msg.ProductId;
                    rawdata.Data = data;
                    return rawdata;
                }
                else
                {
                    return null;
                }

            }
        }

        public void DelDevice(string msg)
        {
            var cache = _provider.GetService<CacheHelper>();
            string tmpkey = msg;
            if (tmpkey.StartsWith("Device:"))
            {
                tmpkey = tmpkey + "$ProductId";
                cache.RemoveCache(tmpkey);
            }
            else if (tmpkey.StartsWith("Offline:"))
            {
                string devid = tmpkey.Split(":")[1];
                tmpkey = "Device:" + devid + "$ProductId";
                cache.RemoveCache(tmpkey);

                //清除待处理包
                _lastReaderDict.TryRemove(devid, out FastReader tmpout);
                _scriptEngine.TryRemove(devid, out CacheJsEngine tmpcache);
                _deviceToNodeGuid.TryRemove(devid, out string tmpguid);
            }
            else
            {
                cache.RemoveCache(tmpkey);
            }

        }
    }
}
