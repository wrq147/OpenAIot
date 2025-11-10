using System;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// 表示不作转换的转换单元
    /// </summary>
    public class NoConvert : ITAConvert
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
            if (targetType == typeof(object))
            {
                return value;
            }

            if (targetType == value.GetType())
            {
                return value;
            }

            return this.NextConvert.Convert(value, targetType);
        }
    }
}
