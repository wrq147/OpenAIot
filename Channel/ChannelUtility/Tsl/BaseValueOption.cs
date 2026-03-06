using System;
using System.Collections.Generic;
using System.Text;

namespace ChannelUtility.Tsl
{
    public class BaseValueOption
    {
        /// <summary>
        /// 数据类型（必填项）
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 预处理表达式（格式：data>0?1:0）
        /// </summary>
        public string express { get; set; }
     
     
  
        public virtual object InnerRawTo(object input)
        {
            return input;
        }
  
    }
}
