using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService
{
    public class DataDetectParam
    {
        private Dictionary<string, string> _param;
        public DataDetectParam(Dictionary<string, string> param)
        {
            _param = param;
        }
        public float GetFloat(string key, float def = 0)
        {
            if (_param.TryGetValue(key, out string tval))
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
            if (_param.TryGetValue(key, out string tval))
            {
                return Convert.ToBoolean(tval);
            }
            else
            {
                return def;
            }
        }
    }
}
