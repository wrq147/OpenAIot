using Common.Share;
using EasyNetQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TemplateAction.Core;

namespace Common.EventBus
{
    /// <summary>
    /// 功能调用事件
    /// </summary>
    [QueueAttribute("call_queue", ExchangeName = "call_important")]
    public class CallEvent : ResponseEvent
    {
        public const string EventKey = "/EV.BUS.CALL";
        private Dictionary<string, object> _tmpobj;
        public CallEvent(string name, object data)
        {
            this.Name = name;
            this.Params = JsonConvert.SerializeObject(data);
        }
        public string GetValue(string key)
        {
            if (_tmpobj == null)
            {
                _tmpobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(this.Params);
            }
            if (_tmpobj == null)
            {
                return null;
            }
            object val;
            if (_tmpobj.TryGetValue(key, out val))
            {
                return Convert.ToString(val);
            }
            return null;
        }
        public long GetLong(string key)
        {
            if (_tmpobj == null)
            {
                _tmpobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(this.Params);
            }
            if (_tmpobj == null)
            {
                return 0;
            }
            object val;
            if (_tmpobj.TryGetValue(key, out val))
            {
                return Convert.ToInt64(val);
            }
            return 0;
        }
        /// <summary>
        /// 业务名称
        /// </summary>
        public string Name { get; set; }
        public string Params { get; set; }
    }
    public class CallResponse : EvtResponse
    {
        public CallResponse(object result)
        {
            if (result != null)
            {
                Result = JsonConvert.SerializeObject(result);
            }
            this.IsDone = true;
        }
        public static CallResponse Next()
        {
            CallResponse rsp = new CallResponse(string.Empty);
            rsp.IsDone = false;
            return rsp;
        }

        public string Result { get; set; }
        public T GetResult<T>()
        {
            JToken jToken = JToken.Parse(Result);
            JObject jObject = JObject.Parse((string)jToken);
            return jObject.ToObject<T>();
        }
    }
}
