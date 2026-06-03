using System;
using System.Text;

namespace ChannelUtility.Tsl
{
    public class StringOption : BaseValueOption
    {
        public StringOption()
        {
            this.type = "string";
        }
        /// <summary>
        /// 最大长度
        /// </summary>
        public int maxLen { get; set; }
        public override object InnerRawTo(object input)
        {
            if (maxLen < 1)
            {
                if(input == null)
                {
                    return null;
                }
                return input.ToString();
            }
            else
            {
                var tmpstr = input.ToString();
                if (tmpstr.Length > maxLen)
                {
                    return tmpstr.Substring(0, maxLen);
                }
                else
                {
                    return tmpstr;
                }
            }
        }
    }
}
