using Common.Share;
using EasyNetQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common.EventBus
{
    /// <summary>
    /// 业务执行事件
    /// </summary>
    [QueueAttribute("bus_queue", ExchangeName = "bus_exchange")]
    public class BusEvent
    {
        public const string EventKey = "/EV.BUS.BUSSIN";
        private Dictionary<string, object> _tmpobj;
        public static BusEvent Create(string name, object data)
        {
            BusEvent evt = new BusEvent();
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

}
