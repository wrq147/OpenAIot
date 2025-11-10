using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.ActionOfData
{
    public class DataCondition
    {
        /// <summary>
        /// 过滤目标字段
        /// </summary>
        public string TargetField { get; set; }
        /// <summary>
        /// 比较值类型：Form、Const
        /// </summary>
        public string ValueType { get; set; }
        /// <summary>
        /// 比较值
        /// </summary>
        public string Value { get; set; }

    }
}
