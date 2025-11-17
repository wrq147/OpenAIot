using AuthService;
using FlowService.FlowNode.FormFields;
using System;
using System.Collections.Generic;

namespace FlowService.Model
{
    public class MZ_FormData
    {
        /// <summary>
        /// 流程已创建则传
        /// </summary>
        public long flowId { get; set; }
        /// <summary>
        /// 批次编号
        /// </summary>
        public string FlowNumber { get; set; }
        /// <summary>
        /// 表单值列表
        /// </summary>
        public Dictionary<string, object> Model { get; set; }
        /// <summary>
        /// 创建用户ID
        /// </summary>
        public long CreateUserId { get; set; }
        /// <summary>
        /// 流程表单数据
        /// </summary>
        public string FormFields { get; set; }
        /// <summary>
        /// 是否为嵌入式流程
        /// </summary>
        public bool isEmbed { get; set; }
        public FormField[] Fields { get; set; }
        /// <summary>
        /// 流程参数
        /// </summary>
        public Dictionary<string, string> inputParams { get; set; }
        /// <summary>
        /// 自选用户
        /// </summary>
        public Dictionary<string, List<Out_UserItem>> Assign { get; set; }
        /// <summary>
        /// 可使用用户部门
        /// </summary>
        public List<MZ_UserDept> UserDeptList { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        public MZ_FlowTemplate Template { get; set; }

    }

}
