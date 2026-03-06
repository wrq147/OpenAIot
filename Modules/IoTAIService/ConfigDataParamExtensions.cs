using ChannelUtility.Message;
using System;


namespace IoTAIService
{
    public static class ConfigDataParamExtensions
    {
        public static float GetFloat(this AIConfigData data, string key, float def = 0)
        {
            if (data.DetParams.TryGetValue(key, out object tval))
            {
                return Convert.ToSingle(tval);
            }
            else
            {
                return def;
            }
        }
        public static bool GetBool(this AIConfigData data, string key, bool def = false)
        {
            if (data.DetParams.TryGetValue(key, out object tval))
            {
                return Convert.ToBoolean(tval);
            }
            else
            {
                return def;
            }
        }
        public static string GetString(this AIConfigData data, string key, string def = "")
        {
            if (data.DetParams.TryGetValue(key, out object tval))
            {
                return Convert.ToString(tval);
            }
            else
            {
                return def;
            }
        }
        public static object Get(this AIConfigData data, string key)
        {
            if (data.DetParams.TryGetValue(key, out object tval))
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
