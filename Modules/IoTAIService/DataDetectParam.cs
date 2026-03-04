using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService
{
    public class DataDetectParam
    {
        private Dictionary<string, object> _param;
        public DataDetectParam(Dictionary<string, object> param)
        {
            _param = param;
        }
        public float GetFloat(string key, float def = 0)
        {
            if (_param.TryGetValue(key, out object tval))
            {
                return Convert.ToSingle(tval);
            }
            else
            {
                return def;
            }
        }
        public bool GetBool(string key, bool def = false)
        {
            if (_param.TryGetValue(key, out object tval))
            {
                return Convert.ToBoolean(tval);
            }
            else
            {
                return def;
            }
        }
        public string GetString(string key, string def = "")
        {
            if (_param.TryGetValue(key, out object tval))
            {
                return Convert.ToString(tval);
            }
            else
            {
                return def;
            }
        }
        public object Get(string key)
        {
            if (_param.TryGetValue(key, out object tval))
            {
                return tval;
            }
            else
            {
                return null;
            }
        }
    }
}
