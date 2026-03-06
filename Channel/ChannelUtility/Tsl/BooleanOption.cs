using System;
namespace ChannelUtility.Tsl
{
    public class BooleanOption : BaseValueOption
    {
        public BooleanOption()
        {
            this.type = "boolean";
        }
        /// <summary>
        /// true显示值
        /// </summary>
        public string trueText { get; set; }
        /// <summary>
        /// false显示值
        /// </summary>
        public string falseText { get; set; }
        public override object InnerRawTo(object input)
        {
            return Convert.ToBoolean(input);
        }
    }
}
