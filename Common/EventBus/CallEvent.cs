using Common.Json;
using EasyNetQ;
using Newtonsoft.Json;
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
        private Dictionary<string, object> GetObjDict()
        {
            if (_tmpobj == null)
            {
                _tmpobj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(this.Params, MyDefaultTextJsonConfig.DefaultOptions);
            }
            return _tmpobj;
        }
        public string GetValue(string key)
        {
            var tmpdict = GetObjDict();
            if (tmpdict == null)
            {
                return null;
            }
            object val;
            if (tmpdict.TryGetValue(key, out val))
            {
                return Convert.ToString(val);
            }
            return null;
        }
        public long GetLong(string key)
        {
            var tmpdict = GetObjDict();
            if (tmpdict == null)
            {
                return 0;
            }
            object val;
            if (tmpdict.TryGetValue(key, out val))
            {
                return Convert.ToInt64(val);
            }
            return 0;
        }

        public List<X> GetList<X>(string key)
        {
            var tmpdict = GetObjDict();
            if (tmpdict == null)
            {
                return null;
            }
            object val;
            if (tmpdict.TryGetValue(key, out val))
            {
                return (List<X>)val;
            }
            return null;
        }
        public object GetObject(string key)
        {
            var tmpdict = GetObjDict();
            if (tmpdict == null)
            {
                return null;
            }
            object val;
            if (tmpdict.TryGetValue(key, out val))
            {
                return val;
            }
            return null;
        }
        public T To<T>()
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(this.Params, MyDefaultTextJsonConfig.DefaultOptions);
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
                response.Result = System.Text.Json.JsonSerializer.Serialize(result, MyDefaultTextJsonConfig.DefaultOptions);
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
            return System.Text.Json.JsonSerializer.Deserialize<T>(this.Result, MyDefaultTextJsonConfig.DefaultOptions);
        }
    }
}
