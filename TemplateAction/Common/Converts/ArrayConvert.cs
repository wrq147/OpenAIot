using System;
using System.Collections;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// 表示数组转换单元
    /// </summary>
    public class ArrayConvert : ITAConvert
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
            if (targetType.IsArray == false)
            {
                return this.NextConvert.Convert(value, targetType);
            }

            Type elementType = targetType.GetElementType();
            Type valueType = value.GetType();
            bool otherif = elementType.IsValueType && (valueType.IsValueType || (valueType == typeof(string) && elementType != typeof(char)));
            if (elementType == valueType || otherif)
            {
                Array tmparray = Array.CreateInstance(elementType, 1);
                tmparray.SetValue(TAConverter.Instance.Convert(value, elementType), 0);
                return tmparray;
            }
            IEnumerable items = value as IEnumerable;
            if (items == null)
            {
                return Array.CreateInstance(elementType, 0);
            }

            var length = 0;
            IList list = items as IList;
            if (list != null)
            {
                length = list.Count;
            }
            else
            {
                IEnumerator enumerator = items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    length = length + 1;
                }
            }

            int index = 0;
            Array array = Array.CreateInstance(elementType, length);
            foreach (object item in items)
            {
                object itemCast = TAConverter.Instance.Convert(item, elementType);
                array.SetValue(itemCast, index);
                index = index + 1;
            }
            return array;
        }
    }
}
