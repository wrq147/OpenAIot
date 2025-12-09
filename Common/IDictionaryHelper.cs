using System;
using System.Collections.Generic;
using System.Linq;
namespace Common
{
    public static class IDictionaryHelper
    {
        public static T Value<T>(this IDictionary<string, object> ipt, string key, T defval)
        {
            if (ipt.TryGetValue(key, out object tmpval))
            {
                return (T)Convert.ChangeType(tmpval, typeof(T));
            }
            return defval;
        }
    }
}
