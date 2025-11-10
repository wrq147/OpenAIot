using System;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// 表示简单类型转换单元
    /// 支持基元类型、guid和枚举相互转换
    /// </summary>
    public class SimpleContert : ITAConvert
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
            if (targetType.IsEnum == true)
            {
                return Enum.Parse(targetType, value.ToString(), true);
            }

            if (typeof(string) == targetType)
            {
                return value.ToString();
            }
            else if (typeof(Guid) == targetType)
            {
                return new Guid(value.ToString());
            }


            IConvertible convertible = value as IConvertible;
            if (convertible != null && targetType.IsValueType)
            {
                return convertible.ToType(targetType, null);
            }

            return this.NextConvert.Convert(value, targetType);
        }
    }
}
