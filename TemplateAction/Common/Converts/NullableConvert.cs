using System;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// 表示可空类型转换单元
    /// </summary>
    public class NullableConvert : ITAConvert
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
            Type underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType == null)
            {
                return this.NextConvert.Convert(value, targetType);
            }
            else
            {
                return TAConverter.Instance.Convert(value, underlyingType);
            }
        }
    }
}
