using Common.Share;
using EasyNetQ;
using Minio;
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
        public static CallEvent Create(string name, object data)
        {
            CallEvent evt = new CallEvent();
            evt.Name = name;
            evt.Params = JsonConvert.SerializeObject(data);
            return evt;
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

        public List<X> GetList<X>(string key)
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
                return (List<X>)val;
            }
            return null;
        }
        public object GetObject(string key)
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
                return val;
            }
            return null;
        }
        /// <summary>
        /// 业务名称
        /// </summary>
        public string Name { get; set; }
        public string Params { get; set; }
    }
    public class CallResponse : EvtResponse
    {
        public static CallResponse Create(object result)
        {
            CallResponse response = new CallResponse();
            if (result != null)
            {
                response.Result = JsonConvert.SerializeObject(result);
            }
            response.IsDone = true;
            return response;
        }

        public static CallResponse Next()
        {
            CallResponse rsp = Create(string.Empty);
            rsp.IsDone = false;
            return rsp;
        }

        public string Result { get; set; }
        public T GetResult<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.Result);
        }
    }
}
