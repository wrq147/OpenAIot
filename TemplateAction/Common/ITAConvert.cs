using System;
namespace TemplateAction.Common
{
    public interface ITAConvert
    {
        /// <summary>
        /// 设置下一个转换单元
        /// </summary>
        ITAConvert NextConvert { set; }

        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns></returns>
        object Convert(object value, Type targetType);
    }
}
