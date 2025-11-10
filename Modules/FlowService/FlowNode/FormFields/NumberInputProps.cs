using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.FormFields
{
    public class NumberInputProps : BaseProps
    {
        public string placeholder { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public double min { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public double max { get; set; }
        /// <summary>
        /// 数值精度
        /// </summary>
        public double precision { get; set; }
    }
}
