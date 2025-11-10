using System;
using System.Collections.Generic;

namespace FlowService.Model
{
    /// <summary>
    /// 发起流程的参数
    /// </summary>
    public class In_TaskAdd
    {
        /// <summary>
        /// 流程模板ID
        /// </summary>
        public long templateId { get; set; }
        /// <summary>
        /// 提交的表单数据
        /// </summary>
        public Dictionary<string, object> model { get; set; }
        /// <summary>
        /// 自选审核人节点数据
        /// </summary>
        public Dictionary<string, List<Out_UserItem>> assign { get; set; }
        /// <summary>
        /// 0表示预生成,1表示保存，2表示提交
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 要编辑的工作流
        /// </summary>
        public long flowId { get; set; }
        /// <summary>
        /// 是否为嵌入式流程
        /// </summary>
        public bool? isEmbed { get; set; }
    }
}
