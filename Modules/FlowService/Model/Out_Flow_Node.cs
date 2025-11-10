using FlowService.FlowNode.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    public class Out_Flow_Node : MZ_Flow_Node
    {
        /// <summary>
        /// 执行处理的用户列表
        /// </summary>
        public List<Out_ActionUser> ActionUsers { get; set; }
        /// <summary>
        /// 待执行用户列表
        /// </summary>
        public List<Out_ActionUser> WaitUsers { get; set; }
        /// <summary>
        /// 子元素
        /// </summary>
        public List<Out_Flow_Node> Children { get; set; } = new List<Out_Flow_Node>();
    }
}
