using System;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// 表示最后一个转换单元
    /// </summary>
    class NotSupportedConvert : ITAConvert
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
            string message = string.Format("不支持将{0}转换为{1}", value, targetType.Name);
            throw new NotSupportedException(message);
        }
    }
}
