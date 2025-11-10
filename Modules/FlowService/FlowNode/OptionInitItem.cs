using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    public class OptionInitItem
    {
        /// <summary>
        /// 选项名称
        /// </summary>
        public string optionName { get; set; }
        /// <summary>
        /// 表单Id
        /// </summary>
        public string fieldid { get; set; }
        /// <summary>
        /// 表单值
        /// </summary>
        public object InitValue { get; set; }
    }
}
