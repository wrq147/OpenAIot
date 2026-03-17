
using Common.Json;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace Common
{
    public static class Constants
    {
        /// <summary>
        /// 成功代码
        /// </summary>
        public const int SUCCESS_CODE = 0;
        /// <summary>
        /// 错误代码
        /// 超时未回复
        /// </summary>
        public const int TIME_OUT = -1;
        /// <summary>
        /// 错误代码
        /// 表示业务繁忙,请稍候重试
        /// </summary>
        public const int ERROR_BUSY = 100;
        /// <summary>
        /// 错误代码
        /// 数据解释异常
        /// </summary>
        public const int PARSE_ERR = 200;
        /// <summary>
        /// 错误代码
        /// 表示输入参数错误
        /// </summary>
        public const int PARAM_ERR = 9001;

        /// <summary>
        /// 接口串行化与反串行化设置
        /// </summary>
        public static JsonSerializerOptions ApiJsonSetting = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = new SerializationControlContractResolver(),
            Converters = { new DateTimeConverter(), new MyStringToNumberConverter(), new MyNumberToStringConverter(), new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// 解释参数的json引擎
        /// </summary>
        public static DecodeJson ParamDecodeJson = (json, t) =>
        {
            return System.Text.Json.JsonSerializer.Deserialize(json, t, Constants.ApiJsonSetting);
        };
        public static IDictionary<Type, string> TpMappings = new Dictionary<Type, string>()
        {
             {typeof(DateTime?), "DateTime"},
             {typeof(DateTime), "DateTime"},
        };
        /// <summary>
        /// 默认参数映射
        /// </summary>
        public static Func<TAAction, Type, object, object> DefaultMappingResolver = (ac, t, input) =>
        {
            string mapval;
            if (TpMappings.TryGetValue(t, out mapval))
            {
                switch (mapval)
                {
                    case "DateTime":
                        {
                            DateTime? val = input as DateTime?;
                            if (val != null)
                            {
                                string clientTZ = ac.Context.Request.Header["TZ"];
                                if (!string.IsNullOrEmpty(clientTZ))
                                {
                                    int tz;
                                    if (int.TryParse(clientTZ, out tz))
                                    {
                                        var clientTime = DateTime.SpecifyKind(val.Value.AddMinutes(tz), DateTimeKind.Utc);
                                        return TimeZoneInfo.ConvertTimeFromUtc(clientTime, TimeZoneInfo.Local);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return input;
        };
        /// <summary>
        /// 全局配置信息
        /// </summary>
        public static GeneralOption General { get; set; }
    }
}
