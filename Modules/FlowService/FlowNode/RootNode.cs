
using System.Collections.Generic;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 根节点
    /// </summary>
    public class RootNode : FlowBaseNode
    {
        public RootProps props { get; set; }

    }
    public class RootProps
    {
        public ObjData[] assignedUser { get; set; }
        /// <summary>
        /// 表单权限
        /// </summary>
        public FormPermsItem[] formPerms { get; set; }
    }
}
