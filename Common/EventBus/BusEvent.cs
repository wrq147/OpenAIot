using Common.Json;
using System;
using System.Collections.Generic;


namespace Common.EventBus
{
    /// <summary>
    /// 业务执行事件
    /// </summary>
    public class BusEvent
    {
        public const string EventKey = "EV.BUS.BUSSIN";
        private Dictionary<string, object> _tmpobj;
        public static BusEvent Create(string name, object data)
        {
            BusEvent evt = new BusEvent();
            evt.Name = name;
            evt.Params = System.Text.Json.JsonSerializer.Serialize(data, MyDefaultTextJsonConfig.DefaultOptions); 
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

}
