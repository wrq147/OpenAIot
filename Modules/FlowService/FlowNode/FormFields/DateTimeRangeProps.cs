using System;
using System.Collections.Generic;

namespace FlowService.FlowNode.FormFields
{
    /// <summary>
    /// 日期范围控件属性
    /// </summary>
    public class DateTimeRangeProps : BaseProps
    {
        public string[] placeholder { get; set; }
        public string format { get; set; }
        public bool showLength { get; set; }
    }
}
