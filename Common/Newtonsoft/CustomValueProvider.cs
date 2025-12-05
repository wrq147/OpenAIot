using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using TemplateAction.Core;

namespace Common.Newtonsoft
{
    public class CustomValueProvider : IValueProvider
    {
        private readonly PropertyInfo _memberInfo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memberInfo"></param>
        public CustomValueProvider(PropertyInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }

        /// <inheritdoc />
        /// <summary>
        /// 获取Value
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public object GetValue(object target)
        {
            var result = _memberInfo.GetValue(target);
            string mapval;
            if (Constants.TpMappings.TryGetValue(_memberInfo.PropertyType, out mapval))
            {
                switch (mapval)
                {
                    case "string":
                        if (result == null)
                        {
                            result = string.Empty;
                        }
                        break;
                    case "DateTime":
                        {
                            TAAction ac = TAAction.Current;
                            if (ac != null)
                            {
                                string clientTZ = ac.Context.Request.Header["TZ"];
                                if (!string.IsNullOrEmpty(clientTZ))
                                {
                                    int tz;
                                    if (int.TryParse(clientTZ, out tz))
                                    {
                                        DateTime? val = result as DateTime?;
                                        if (val != null)
                                        {
                                            var clientTime = TimeZoneInfo.ConvertTimeToUtc(val.Value).AddMinutes(-tz);
                                            result = clientTime;
                                        }
                                    }

                                }

                            }
                        }
                        break;
                }
            }

            return result;

        }

        /// <inheritdoc />
        /// <summary>
        /// 设置Value
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void SetValue(object target, object value)
        {
            try
            {
                string mapval;
                if (Constants.TpMappings.TryGetValue(_memberInfo.PropertyType, out mapval))
                {
                    switch (mapval)
                    {
                        case "DateTime":
                            {
                                TAAction ac = TAAction.Current;
                                if (ac != null)
                                {
                                    DateTime? val = value as DateTime?;
                                    if (val != null)
                                    {
                                        string clientTZ = ac.Context.Request.Header["TZ"];
                                        if (!string.IsNullOrEmpty(clientTZ))
                                        {
                                            int tz;
                                            if (int.TryParse(clientTZ, out tz))
                                            {

                                                _memberInfo.SetValue(target, TimeZoneInfo.ConvertTimeFromUtc(val.Value.AddMinutes(tz), TimeZoneInfo.Local));
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }

                _memberInfo.SetValue(target, value);
            }
            catch { }
        }
    }
}
