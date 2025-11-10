using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// 表示字典转换单元
    /// </summary>
    public class DictionaryConvert : ITAConvert
    {

        /// <summary>
        /// 下一个转换单元
        /// </summary>
        public ITAConvert NextConvert { get; set; }

        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType)
        {
            IDictionary<string, object> dic = value as IDictionary<string, object>;
            if (dic == null)
            {
                return this.NextConvert.Convert(value, targetType);
            }
            object instance = Activator.CreateInstance(targetType);
            PropertyInfo[] setters = PropertyCache.GetProperties(targetType);

            foreach (PropertyInfo setter in setters)
            {
                if (setter.CanWrite == false)
                {
                    continue;
                }
                object targetValue;
                if (dic.TryGetValue(setter.Name, out targetValue) == false)
                {
                    continue;
                }

                object valueCast = TAConverter.Instance.Convert(targetValue, setter.PropertyType);
                setter.SetValue(instance, valueCast, null);
            }

            return instance;
        }
    }
}
