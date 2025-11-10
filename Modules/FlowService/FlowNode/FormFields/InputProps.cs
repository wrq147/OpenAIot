using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FlowService.FlowNode.FormFields
{
    public class InputProps : BaseProps
    {
        /// <summary>
        /// 占位符
        /// </summary>
        public string placeholder { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string defaultValue { get; set; }
    }
}
