using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 抄送节点
    /// </summary>
    public class CSNode : FlowBaseNode
    {
        public CSUserProps props { get; set; }

    }


    /// <summary>
    /// 抄送节点的属性
    /// </summary>
    public class CSUserProps
    {
        /// <summary>
        /// 是否发起人可选择
        /// </summary>
        public bool shouldAdd { get; set; }
        /// <summary>
        /// 目标对象人员
        /// </summary>
        public ObjData[] assignedUser { get; set; }
        /// <summary>
        /// 表单权限
        /// </summary>
        public FormPermsItem[] formPerms { get; set; }
    }
}
