using System;

namespace FlowService.Model
{
    public class Out_FlowTodoItem: MZ_Flow_ExtensionAttribute
    {
        public long TemplateId { get; set; }
        public string FlowName { get; set; }
        public string StepName { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public string startRealName { get; set; }
        /// <summary>
        /// 发起人部门
        /// </summary>
        public string startDeptName { get; set; }
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        public long createId { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string Background { get; set; }
    }
}
