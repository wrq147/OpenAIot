using ChannelUtility.Message;
using ChannelUtility.Tsl;
using IoTService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TemplateAction.Core;
namespace IoTRulesService.DataParser.Js
{
    /// <summary>
    /// 功能函数脚本用
    /// </summary>
    public class FuncMessageContext : MessageContext
    {
        public FuncMessageContext(BaseDeviceMessage msg, PackParser client, TslModel model, string funPrefix) : base(msg, client, model, funPrefix)
        {
        }
        /// <summary>
        /// 获取请求的功能消息
        /// </summary>
        /// <returns></returns>
        public FunctionInvokeMessage FuncMessage()
        {
            return Message() as FunctionInvokeMessage;
        }


        /// <summary>
        /// 刷新前端功能列表
        /// </summary>
        public void Refresh()
        {
            var res = _client.FunCmd(_msg.DeviceId, "update");
            res.Wait();
        }
        /// <summary>
        /// 重置事件周期
        /// </summary>
        /// <param name="code"></param>
        public void ResetSilenceTime(string code)
        {
            var res = _client.ResetSilenceTime(_msg.DeviceId, code);
            res.Wait();
        }
        /// <summary>
        /// 通知前端跳转到指定url
        /// </summary>
        /// <param name="url">要跳转的url</param>
        public void GoUrl(string url)
        {
            var res = _client.FunCmd(_msg.DeviceId, $"redirect {url}");
            res.Wait();
        }
        /// <summary>
        /// 通知前端展示信息
        /// </summary>
        /// <param name="msg"></param>
        public void ShowMsg(string msg)
        {
            var res = _client.FunCmd(_msg.DeviceId, $"show {msg}");
            res.Wait();
        }
        /// <summary>
        /// 获取当前设备的源协议Id
        /// </summary>
        /// <returns></returns>
        public string RawProductId()
        {
            var res = _client.GetRawProductId(_msg.DeviceId);
            return res.Result;
        }
        /// <summary>
        /// 获取当前设备的源dtuid
        /// </summary>
        /// <returns></returns>
        public string RawDtuId()
        {
            var res = _client.GetRawDtuId(_msg.DeviceId);
            return res.Result;
        }
        /// <summary>
        /// 变更所属协议
        /// </summary>
        /// <param name="targetId">协议编号</param>
        public void ChangeProduct(string targetId)
        {
            ChangeProductMessage msg = new ChangeProductMessage();
            msg.DeviceId = _msg.DeviceId;
            msg.ProductId = _msg.ProductId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.TargetProductId = targetId;
            var res = _client.ConfirmReply(string.Empty, msg);
            res.Wait();
        }
        /// <summary>
        /// 功能执行成功
        /// </summary>
        /// <param name="outputs"></param>
        public void ConfirmSuccess(IDictionary<string, object> outputs)
        {
            var reply = this.CreateFuncReply();
            reply.IsSuccess = true;
            reply.Outputs = outputs;
            var curMsg = FuncMessage();
            this.ConfirmReply(curMsg.MessageId, reply);
        }
        /// <summary>
        /// 功能执行失败
        /// </summary>
        /// <param name="error"></param>
        public void ConfirmError(string error)
        {
            var reply = this.CreateFuncReply();
            reply.IsSuccess = false;
            reply.Error = error;
            var curMsg = FuncMessage();
            this.ConfirmReply(curMsg.MessageId, reply);
        }
        ThreadLocal<int> _localVal = new ThreadLocal<int>();
        static readonly ConcurrentDictionary<Guid, System.Timers.Timer> _timers = new();
        /// <summary>
        /// 定时执行
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="millisecond"></param>
        public void SetTimeout(Action ac, int millisecond)
        {
            if (_localVal.Value == 1)
            {
                this.Print("定时器无法嵌套使用");
                return;
            }
            if (millisecond > 120000)
            {
                this.Print("定时器的参数millisecond不能大于120000");
                return;
            }
            var timerId = Guid.NewGuid();
            var timer = new System.Timers.Timer(millisecond);
            timer.Elapsed += delegate (object? sender, System.Timers.ElapsedEventArgs e)
            {
                _localVal.Value = 1;
                timer.Enabled = false;
                try
                {
                    ac();//调用方法
                }
                catch { }
                _localVal.Value = 0;
                _timers.TryRemove(timerId, out _);
            };
            timer.Enabled = true;
            _timers.TryAdd(timerId, timer); // 防止GC回收
        }

        /// <summary>
        /// 执行设备的其它功能
        /// </summary>
        /// <param name="funId">功能代码</param>
        /// <param name="inputs">参数</param>
        public object Execute(string funId, IDictionary<string, object> inputs)
        {
            var curfun = FuncMessage();
            string comparefun = curfun.DeviceId + "|" + funId;
            if (!string.IsNullOrEmpty(curfun.SourceFunction) && curfun.SourceFunction == comparefun)
            {
                var tmpxxs = _client.Print(_msg.DeviceId, "功能调用异常", "无法死循环调用同一个功能");
                return null;
            }
            FunctionInvokeMessage msg = new FunctionInvokeMessage();
            msg.DeviceId = curfun.DeviceId;
            msg.ProductId = curfun.ProductId;
            msg.FunctionId = funId;
            msg.Inputs = inputs ?? new Dictionary<string, object>();
            msg.SourceFunction = string.IsNullOrEmpty(curfun.SourceFunction) ? (curfun.DeviceId + "|" + curfun.FunctionId) : (curfun.SourceFunction);
            msg.MessageId = Guid.NewGuid().ToString("N");
            var tmprrr = _client.PublicWaitFuncReply(msg);
            var res = tmprrr.Result;
            if (res == null)
            {
                this.Print($"功能{funId}调用超时");
                return null;
            }
            return res.Outputs;
        }
        /// <summary>
        /// 执行其它设备的功能
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="funId"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public object ExecuteOther(string deviceId, string funId, IDictionary<string, object> inputs)
        {
            var curfun = FuncMessage();
            string comparefun = deviceId + "|" + funId;


            if (!string.IsNullOrEmpty(curfun.SourceFunction) && curfun.SourceFunction == comparefun)
            {
                var tmpss = _client.Print(_msg.DeviceId, "功能调用异常", "无法死循环调用同一个功能");
                return null;
            }

            if (deviceId == this._msg.DeviceId)
            {
                FunctionInvokeMessage msg = new FunctionInvokeMessage();
                msg.DeviceId = deviceId;
                msg.ProductId = this._msg.ProductId;
                msg.FunctionId = funId;
                msg.Inputs = inputs ?? new Dictionary<string, object>();
                msg.SourceFunction = string.IsNullOrEmpty(curfun.SourceFunction) ? (curfun.DeviceId + "|" + curfun.FunctionId) : (curfun.SourceFunction);
                msg.MessageId = Guid.NewGuid().ToString("N");

                var tmpres = _client.PublicWaitFuncReply(msg);
                var res = tmpres.Result;
                if (res == null)
                {
                    this.Print($"功能{funId}调用超时");
                    return null;
                }

                return res.Outputs;
            }
            else
            {
                var tmpres = this._client.Provider.GetService<ServerBusProxy>().DownFunction(string.Empty, deviceId, funId, inputs ?? new Dictionary<string, object>());
                var res = tmpres.Result;
                if (!res.IsSuccess())
                {
                    this.Print($"功能{funId}调用异常：{res.Message}");
                    return null;
                }

                return res.Data;
            }
        }
    }
}
