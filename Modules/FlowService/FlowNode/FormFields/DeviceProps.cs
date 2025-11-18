using System;
namespace FlowService.FlowNode.FormFields
{
    public class DeviceProps : BaseProps
    {
        public string placeholder { get; set; }
        /// <summary>
        /// 限制选择个数
        /// </summary>
        public int limit { get; set; }
        /// <summary>
        /// 限制的协议
        /// </summary>
        public string[] limit_product { get; set; }
        /// <summary>
        /// 同步控制列表
        /// </summary>
        public SelectSyncItem[] synclist { get; set; }
    }
    public class SelectSyncItem
    {
        /// <summary>
        /// 物模型的属性标识
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 表单字段ID
        /// </summary>
        public string field_id { get; set; }
    }
}
