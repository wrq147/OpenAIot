using System;
using System.Threading.Tasks;
using ChannelUtility.Message;
using System.Collections.Generic;
using EasyNetQ;
using Jint;
using Jint.Native;
using ChannelUtility.Js;
using ChannelUtility.Buffers;
using ChannelUtility.Tsl;
using Jint.Runtime.Interop;
using System.Threading;
using Jint.Runtime;
using System.Linq;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using EasyNetQ.Consumer;
using EasyNetQ.DI;
using Esprima.Ast;


namespace ChannelUtility
{
    /// <summary>
    /// 客户端用事件总线代理
    /// </summary>
    public class ClientBusProxy : ChannelRegister
    {
        private IServiceProvider _provider;
        public delegate Task SubProductMessage(RequestMessage msg, TslReturn ret);
        /// <summary>
        /// 监听订阅的指定产品的消息
        /// </summary>
        public event SubProductMessage OnSubProductMessage;
        private IBus _bus;

        public ClientBusProxy(IServiceProvider provider) : base(provider)
        {
            _provider = provider;
            _bus = RabbitHutch.CreateBus(_option.EventConn, x =>
            {
                x.Register<IConsumerErrorStrategy, ChannelAlwaysRequeueErrorStrategy>();
            });
            _bus.PubSub.Subscribe<string>("IotDown" + _option.config.Code, async (msg, tk) =>
            {
                var rs = System.Text.Json.JsonSerializer.Deserialize<RequestMessage>(msg, JsonMessageSerializerConfig.DefaultOptions);
                await PublicMessage(rs, null);
            }, cfg =>
            {
                cfg.WithTopic("/device." + _option.config.Code + ".down");
                cfg.WithAutoDelete(true);
            });

            _bus.PubSub.Subscribe<string>("RuleNode" + Guid.NewGuid().ToString("N"), (msg) =>
            {
                UpdateUpList();
            }, cfg =>
            {
                cfg.WithTopic("/RuleNode.Change");
                cfg.WithAutoDelete(true);
            });

            _bus.PubSub.Subscribe<string>("IotKeyDel" + Guid.NewGuid().ToString("N"), (msg) =>
            {
                string tmpkey = msg;
                if (tmpkey.StartsWith("Device:"))
                {
                    tmpkey = tmpkey + "$ProductId";
                    _memoryCache.Remove(tmpkey);
                }
                else if (tmpkey.StartsWith("Offline:"))
                {
                    string devid = tmpkey.Split(":")[1];
                    tmpkey = "Device:" + devid + "$ProductId";
                    _memoryCache.Remove(tmpkey);

                    //清除待处理包
                    _lastReaderDict.TryRemove(devid, out FastReader tmpout);
                    _readerCache.TryRemove(devid, out ConcurrentDictionary<string, string> tmpcache);
                }
                else
                {
                    _memoryCache.Remove(tmpkey);
                }

            }, cfg =>
            {
                cfg.WithTopic("/IotKey.Del");
                cfg.WithAutoDelete(true);
            });

            UpdateUpList();
        }


        /// <summary>
        /// 直接调用下发消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task PublicMessage(RequestMessage msg, TslReturn ret)
        {
            if (OnSubProductMessage != null)
            {
                if (ret == null)
                {
                    ret = await this.GetTsl(msg.ProductId, msg.DeviceId).ConfigureAwait(false);
                    if (ret == null)
                    {
                        throw new Exception("物模型未初始化，请绑定设备编码后重启您的设备");
                    }
                }

                if (msg is RawDataMessage)
                {
                    await OnSubProductMessage(msg, ret).ConfigureAwait(false);
                    return;
                }

                var newmsg = await this.toRawData(msg, ret).ConfigureAwait(false);
                if (newmsg != null)
                {
                    await OnSubProductMessage(newmsg, ret).ConfigureAwait(false);
                }
                else
                {
                    if (msg is DeviceBindMessage bindMessage)
                    {
                        await this.ConfirmBindReply(bindMessage.ProductId, bindMessage.DeviceId, true, bindMessage.MessageId).ConfigureAwait(false);
                    }
                }
            }
        }


        public async Task<ReadPropertyMessageReply> PublicWaitReadProperty(ReadPropertyMessage msg)
        {
            await StartReadAllMessage(msg.ProductId, msg.DeviceId, msg.Properties);

            var tcs = new TaskCompletionSource<ReadPropertyMessageReply>(TaskCreationOptions.RunContinuationsAsynchronously);
            //8秒后自动取消
            var cts = new CancellationTokenSource(8000);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            var rs = await _bus.SendReceive.ReceiveAsync<ReadPropertyMessageReply>("bus.response." + msg.MessageId, msg =>
            {
                tcs.TrySetResult(msg);
            }, cfg =>
            {
                cfg.WithAutoDelete(true);
            }, cts.Token);
            try
            {
                await this.PublicMessage(msg, null);
                var reply = await tcs.Task.ConfigureAwait(false);
                rs.Dispose();
                return reply;
            }
            catch (Exception ex)
            {
                rs.Dispose();
                return null;
            }
        }

        public async Task<FunctionInvokeMessageReply> PublicWaitFuncReply(FunctionInvokeMessage msg)
        {
            var tcs = new TaskCompletionSource<FunctionInvokeMessageReply>(TaskCreationOptions.RunContinuationsAsynchronously);
            //8秒后自动取消
            var cts = new CancellationTokenSource(8000);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            var rs = await _bus.SendReceive.ReceiveAsync<FunctionInvokeMessageReply>("bus.response." + msg.MessageId, msg =>
            {
                tcs.TrySetResult(msg);
            }, cfg =>
            {
                cfg.WithAutoDelete(true);
            }, cts.Token).ConfigureAwait(false);
            try
            {
                await this.PublicMessage(msg, null).ConfigureAwait(false);
                var reply = await tcs.Task.ConfigureAwait(false);
                rs.Dispose();
                return reply;
            }
            catch (Exception ex)
            {
                rs.Dispose();
                return null;
            }
        }

        /// <summary>
        /// 向设备控制台打印消息
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="tip"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task Print(string devId, string tip, object msg)
        {
            List<string> data = new List<string>();
            data.Add("console/" + devId);
            data.Add(tip + ":" + System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.SerializeOptions));

            await _bus.PubSub.PublishAsync(data, "/MqttNotice.Msg").ConfigureAwait(false);
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
            await _bus.PubSub.PublishAsync(data, "/MqttNotice.Msg").ConfigureAwait(false);
        }
        /// <summary>
        /// 获取指定设备的当前所有属性
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetProps(string deviceId)
        {
            var tkey = "Device:" + deviceId;
            var redisdict = await _redis.HashGetAllAsync<string>(tkey).ConfigureAwait(false);
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
            var redisstr = await _redis.HashGetAsync<string>(tkey, code).ConfigureAwait(false);
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
            var redisstr = await _redis.HashGetAsync<string>(tkey, "$rawDtuId").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(redisstr))
            {
                return redisstr;
            }
            return null;
        }
        /// <summary>
        /// 获取当前设备的源产品Id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<string> GetRawProductId(string deviceId)
        {
            var tkey = "Device:" + deviceId;
            var redisstr = await _redis.HashGetAsync<string>(tkey, "$rawProductId").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(redisstr))
            {
                return redisstr;
            }
            return null;
        }
        
        /// <summary>
        /// 获取指定产品的物模型
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<TslReturn> GetTsl(string productId, string deviceId, bool sendconn = false)
        {
            if (string.IsNullOrEmpty(productId))
            {
                if (!_memoryCache.TryGetValue("Device:" + deviceId + "$ProductId", out productId))
                {
                    productId = await _redis.HashGetAsync<string>("Device:" + deviceId, "$ProductId").ConfigureAwait(false);
                    if (string.IsNullOrEmpty(productId))
                    {
                        if (sendconn)
                        {
                            //重新发送连接包
                            await Connected(deviceId).ConfigureAwait(false);
                            int tcount = 0;
                            while (tcount < 5)
                            {
                                await Task.Delay(50).ConfigureAwait(false);
                                productId = await _redis.HashGetAsync<string>("Device:" + deviceId, "$ProductId").ConfigureAwait(false);
                                if (!string.IsNullOrEmpty(productId))
                                {
                                    break;
                                }
                                ++tcount;
                            }

                            if (productId == null)
                            {
                                return null;
                            }
                            else
                            {
                                _memoryCache.Set("Device:" + deviceId + "$ProductId", productId, TimeSpan.FromMinutes(60));
                            }
                        }
                        else
                        {
                            //使用临时设备-产品关联
                            TempProductMessage msg = new TempProductMessage();
                            msg.ProductId = string.Empty;
                            msg.DeviceId = deviceId;
                            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

                            await ConfirmReplyAsync(null, msg).ConfigureAwait(false);
                            int tcount = 0;
                            while (tcount < 5)
                            {
                                await Task.Delay(50).ConfigureAwait(false);
                                productId = await _redis.StringGetAsync<string>("TempDevice:" + deviceId).ConfigureAwait(false);
                                if (productId != null)
                                {
                                    break;
                                }
                                ++tcount;
                            }
                            if (productId == null)
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        _memoryCache.Set("Device:" + deviceId + "$ProductId", productId, TimeSpan.FromMinutes(60));
                    }
                }
            }

            Dictionary<string, string> proDict;
            if (!_memoryCache.TryGetValue("ProductSys:" + productId, out proDict))
            {
                proDict = await _redis.HashGetAllAsync<string>("ProductSys:" + productId);
                if (proDict == null)
                {
                    return null;
                }
                else
                {
                    _memoryCache.Set("ProductSys:" + productId, proDict, TimeSpan.FromMinutes(60));
                }
            }

            string modeltsl;
            if (!proDict.TryGetValue("$ModelTSL", out modeltsl))
            {
                return null;
            }

            string modelscript;
            proDict.TryGetValue("$Script", out modelscript);

            string status;
            proDict.TryGetValue("$Status", out status);

            string netway;
            proDict.TryGetValue("$NetworkWay", out netway);

            var model = TslModel.CreateFrom(modeltsl);
            return new TslReturn(productId, model, status, modelscript, netway);
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
                msgId = await _redis.ListLeftPopAsync<string>($"DeviceMsgId:{deviceId}").ConfigureAwait(false);
            }
            string tkey = "subs:" + deviceId + msgId;
            await _bus.SendReceive.SendAsync("bus.response." + tkey, value).ConfigureAwait(false);
        }

        public async Task<string> WaitOnly(string deviceId, string msgId)
        {
            await _redis.ListRightPushAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);

            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            //8秒后自动取消
            var cts = new CancellationTokenSource(8000);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            string tkey = "subs:" + deviceId + msgId;

            var rs = await _bus.SendReceive.ReceiveAsync<string>("bus.response." + tkey, msg =>
            {
                tcs.TrySetResult(msg);
            }, cfg =>
            {
                cfg.WithAutoDelete(true);
            }, cts.Token).ConfigureAwait(false);

            try
            {
                var reply = await tcs.Task.ConfigureAwait(false);
                rs.Dispose();
                //清除系统消息Id
                await _redis.ListRemoveAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);
                return reply;
            }
            catch (Exception ex)
            {
                //清除系统消息Id
                await _redis.ListRemoveAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);
                rs.Dispose();
                await Print(deviceId, "异常", "未收到回复消息").ConfigureAwait(false);
                return null;
            }
        }

        /// <summary>
        /// 获取设备指定类型的返回值
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="msgId"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public async Task<string> PublicWait(string deviceId, string msgId, Func<Task> ac)
        {
            if (msgId == null)
            {
                msgId = Guid.NewGuid().ToString("N");
            }
            await _redis.ListRightPushAsync($"DeviceMsgId:{deviceId}", msgId);

            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            //8秒后自动取消
            var cts = new CancellationTokenSource(8000);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            string tkey = "subs:" + deviceId + msgId;

            var rs = await _bus.SendReceive.ReceiveAsync<string>("bus.response." + tkey, msg =>
            {
                tcs.TrySetResult(msg);
            }, cfg =>
            {
                cfg.WithAutoDelete(true);
            }, cts.Token).ConfigureAwait(false);

            await ac.Invoke();
            try
            {
                var reply = await tcs.Task.ConfigureAwait(false);
                rs.Dispose();
                //清除系统消息Id
                await _redis.ListRemoveAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);
                return reply;
            }
            catch (Exception ex)
            {
                rs.Dispose();
                await Print(deviceId, "异常", "下发的消息无回复").ConfigureAwait(false);
                //清除系统消息Id
                await _redis.ListRemoveAsync($"DeviceMsgId:{deviceId}", msgId).ConfigureAwait(false);
                return null;
            }
        }


        /// <summary>
        /// 解释自定义数据包
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="model"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public async Task<byte[]> ParseCustom(RequestMessage msg, TslModel model, string script)
        {
            if (!string.IsNullOrEmpty(script) && this.Option.config.CanScript)
            {
                var context = new MessageContext(msg, this, model, null);
                try
                {
                    var tmp = await Task.Run(() =>
                    {
                        var jsEngine = new Engine(option =>
                        {
                            option.LimitRecursion(5).TimeoutInterval(TimeSpan.FromMinutes(5));
                        });
                        return jsEngine.Execute(script).Invoke("toRawData", JsValue.FromObject(jsEngine, context));
                    });

                    if (tmp.IsNull()) return null;
                    var payload = tmp.As<ObjectWrapper>().Target as FastWriter;
                    if (payload == null) return null;
                    return payload.ToArray();
                }
                catch (JavaScriptException ex)
                {
                    var location = ex.Location;
                    await Print(msg.DeviceId, "toRawData执行错误", string.Format("在行{0}至行{1}发生异常:{2}", location.Start.Line, location.End.Line, ex.Message));
                    return null;
                }
                catch (Exception ex)
                {
                    await Print(msg.DeviceId, "toRawData异常", "脚本未知错误" + ex.Message);
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
        public async Task ParseFunc(string funcScript, FunctionInvokeMessage msg, TslModel model, string funprefix)
        {
            var context = new FuncMessageContext(msg, this, model, funprefix);
            try
            {
                await Task.Run(() =>
                {
                    var jsEngine = new Engine(option =>
                    {
                        option.LimitRecursion(5).TimeoutInterval(TimeSpan.FromMinutes(10));
                    });
                    jsEngine.Execute(funcScript).Invoke("exeFunc", JsValue.FromObject(jsEngine, context));
                });
            }
            catch (JavaScriptException ex)
            {
                var location = ex.Location;
                await Print(msg.DeviceId, "exeFunc执行错误", string.Format("在行{0}至行{1}发生异常:{2}", location.Start.Line, location.End.Line, ex.Message));
            }
            catch (Exception ex)
            {
                await Print(msg.DeviceId, "exeFunc异常", "脚本未知错误" + ex.Message);
            }
        }

        private List<string> _upList;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public void UpdateUpList()
        {
            var dict = _redis.HashGetAll<string>("RuleExeNodes");
            var tmplist = new List<string>();
            if (dict != null)
            {
                foreach (var item in dict)
                {
                    tmplist.Add(item.Key);
                }
            }

            _lock.EnterWriteLock();
            try
            {
                _upList = tmplist;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        private string GetUpKey(string deviceId)
        {
            _lock.EnterReadLock();
            try
            {
                if (_upList == null || _upList.Count == 0) return "/device.up";
                int pos = Math.Abs(deviceId.GetHashCode() % _upList.Count);
                return "/device.up." + _upList[pos];
            }
            finally
            {
                _lock.ExitReadLock();
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
        /// <returns></returns>
        public async Task<bool> PushCustom(string productId, string deviceId, IDictionary<string, object> properties, FastReader input, string script, TslModel model, string codeprefix)
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
            }

            if (string.IsNullOrEmpty(script) || !this.Option.config.CanScript)
            {
                if (msg != null)
                {
                    await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            try
            {
                var datacontext = new DataContext(msg, input, productId, deviceId, this, model, codeprefix);
                var func = await Task.Run(() =>
                {
                    var jsEngine = new Engine(option =>
                    {
                        option.LimitRecursion(5).TimeoutInterval(TimeSpan.FromMinutes(5));
                    });
                    var context = JsValue.FromObject(jsEngine, datacontext);
                    return jsEngine.Execute(script).Invoke("rawDataTo", context);
                });

                if (func.IsNull())
                {
                    if (msg != null)
                    {
                        await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
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
                        await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (newmsg is EmptyMessageReply)
                {
                    return true;
                }

                await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(newmsg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
                return true;
            }
            catch (JavaScriptException ex)
            {
                var location = ex.Location;
                await Print(deviceId, "rawDataTo执行错误", string.Format("在行{0}至行{1}发生异常:{2}", location.Start.Line, location.End.Line, ex.Message));
                return false;
            }
            catch (Exception ex)
            {
                await Print(deviceId, "rawDataTo异常", "脚本未知错误" + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// 发送设备在线给事件总线
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task Connected(string deviceId, string ip = null)
        {
            DeviceOnlineMessage msg = new DeviceOnlineMessage();
            msg.ProductId = string.Empty;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.IpAddress = ip;
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
        }

        /// <summary>
        /// 发送设备离线给事件总线
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task Disconnect(string deviceId)
        {
            await _redis.KeyDeleteAsync($"DeviceMsgId:{deviceId}");
            DeviceOfflineMessage msg = new DeviceOfflineMessage();
            msg.ProductId = string.Empty;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));

            //清除待处理包
            _lastReaderDict.TryRemove(deviceId, out FastReader tmpout);
            _readerCache.TryRemove(deviceId, out ConcurrentDictionary<string, string> tmpcache);
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
            await _bus.SendReceive.SendAsync("bus.response." + msgId, msg);
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
            await _bus.SendReceive.SendAsync("bus.response." + msgId, msg);
        }
        /// <summary>
        /// 发送ICCID回复包
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="iccid"></param>
        /// <returns></returns>
        public async Task PublishICCIDReply(string productId, string deviceId, string iccid)
        {
            QueryICCIDMessageReply msg = new QueryICCIDMessageReply();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.iccid = iccid;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
        }
        /// <summary>
        /// 上报事件
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="eventId"></param>
        /// <param name="outputs"></param>
        /// <returns></returns>
        public async Task PublishEvent(string productId, string deviceId, string eventId, IDictionary<string, object> outputs)
        {
            DeviceEventMessage msg = new DeviceEventMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.EventId = eventId;
            msg.Outputs = outputs;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
        }
        /// <summary>
        /// 发送确认回复包（同步）
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        public void ConfirmReply(string msgId, BaseUpDeviceMessage msg)
        {
            if (string.IsNullOrEmpty(msgId))
            {
                _bus.PubSub.Publish(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(msg.DeviceId));
            }
            else
            {
                _bus.SendReceive.Send("bus.response." + msgId, msg);
            }
        }
        /// <summary>
        /// 发送确认回复包（异步）
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task ConfirmReplyAsync(string msgId, BaseUpDeviceMessage msg)
        {
            if (string.IsNullOrEmpty(msgId))
            {
                await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(msg.DeviceId));
            }
            else
            {
                await _bus.SendReceive.SendAsync("bus.response." + msgId, msg);
            }
        }
        public async Task StartReadAllMessage(string productId, string deviceId, List<string> props)
        {
            StartReadAllMessage msg = new StartReadAllMessage();
            msg.ProductId = productId;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.props = props;
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
        }

        /// <summary>
        /// 重置事件周期
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task ResetSilenceTime(string deviceId, string code)
        {
            await _redis.KeyDeleteAsync("IotQuick::" + deviceId + "::" + code).ConfigureAwait(false);
        }
    }
}
