using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    /// <summary>
    /// 异常检测节点
    /// </summary>
    public class ExceptNode : RuleBaseNode
    {
        public ExceptProps props { get; set; }
    }

    public class ExceptProps
    {
        /// <summary>
        /// 节拍器Id
        /// </summary>
        public string CountId { get; set; }
        /// <summary>
        /// 异常检测类型：峰值spike、更改change、range范围
        /// </summary>
        public string ExceptType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int windowSize { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public double Min { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public double Max { get; set; }
    }
}
