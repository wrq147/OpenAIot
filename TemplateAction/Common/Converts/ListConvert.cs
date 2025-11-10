using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// 表示List转换单元
    /// </summary>
    public class ListConvert : ITAConvert
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
            if (targetType.IsGenericType == false)
            {
                return this.NextConvert.Convert(value, targetType);
            }

            Type defindtionType = targetType.GetGenericTypeDefinition();
            if (defindtionType != typeof(List<>))
            {
                return this.NextConvert.Convert(value, targetType);
            }

            IEnumerable items = value as IEnumerable;
            IList list = Activator.CreateInstance(targetType) as IList;
            if (items == null)
            {
                return list;
            }

            int length = 0;
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
            Type elementType = targetType.GetGenericArguments().FirstOrDefault();
            foreach (object item in items)
            {
                object itemCast = TAConverter.Instance.Convert(item, elementType);
                list.Add(itemCast);
                index = index + 1;
            }
            return list;
        }
    }
}
