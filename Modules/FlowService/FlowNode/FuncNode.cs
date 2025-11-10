using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 功能节点
    /// </summary>
    public class FuncNode : FlowBaseNode
    {
        public FuncProps props { get; set; }
    }
    public class FuncProps
    {
        /// <summary>
        /// 设备表单项
        /// </summary>
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public FuncExeItem[] Items { get; set; }

    }
    public class FuncExeItem
    {
        /// <summary>
        /// 匹配的产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 匹配的产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 功能标识
        /// </summary>
        public string FunctionId { get; set; }
        /// <summary>
        /// 功能名称
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 执行功能传的值
        /// </summary>
        public Dictionary<string, object> InputData { get; set; }
        public int IsFunc { get; set; } = 0;
    }
}
