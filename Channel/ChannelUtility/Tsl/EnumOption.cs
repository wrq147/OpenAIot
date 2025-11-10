using System;
using System.Collections.Generic;
using System.Linq;

namespace ChannelUtility.Tsl
{
    public class EnumOption : BaseValueOption
    {
        public EnumOption()
        {
            this.type = "enum";
        }
        /// <summary>
        /// 枚举元素
        /// </summary>
        public Dictionary<string, string> elements { get; set; }

        protected override object InnerRawTo(object input)
        {
            string key = (input ?? "").ToString();

            string[] tmparr = key.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (key.IndexOf(',') != -1)
            {
                List<string> targetList = new List<string>(50);
                foreach (string tmp in tmparr)
                {
                    string val;
                    if (elements.TryGetValue(tmp, out val))
                    {
                        targetList.Add(val);
                    }
                }
                return string.Join(',', targetList);
            }
            else
            {
                string val;
                if (elements.TryGetValue(key, out val))
                {
                    return val;
                }
                else
                {
                    return string.Empty;
                }
            }

        }
    }
}
