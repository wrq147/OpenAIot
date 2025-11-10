using System;
using System.Threading.Tasks;
using System.Linq;
namespace FlowService.FlowNode
{
    public abstract class FlowBaseNode
    {
        /// <summary>
        /// 节点id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 父节点id
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 节点描述
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// 下一个节点
        /// </summary>
        public FlowBaseNode children { get; set; }

    }
}
