using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// 类属性缓存
    /// </summary>
    public class PropertyCache
    {
        private static readonly Dictionary<Type, PropertyInfo[]> cached = new Dictionary<Type, PropertyInfo[]>();
        public static PropertyInfo[] GetProperties(Type type)
        {
            lock (cached)
            {
                if (cached.ContainsKey(type))
                {
                    return cached[type];
                }
                else
                {
                    PropertyInfo[] rt= type.GetProperties();
                    cached.Add(type, rt);
                    return rt;
                }
            }
        }
    }
}
