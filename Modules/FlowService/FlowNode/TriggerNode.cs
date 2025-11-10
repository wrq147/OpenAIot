
namespace FlowService.FlowNode
{
    public class TriggerNode : FlowBaseNode
    {
        public TriggerProps props { get; set; }
    }
    public class TriggerProps
    {
        /// <summary>
        /// 默认为WEBHOOK,也可以是EMAIL、NEWFLOW
        /// </summary>
        public string type { get; set; }
        public TriggerHttpProps http { get; set; }
        public TriggerEmailProps email { get; set; }
        public NewFlowProps flow { get; set; }
    }
    /// <summary>
    /// 流程触发的属性配置
    /// </summary>
    public class NewFlowProps
    {
        /// <summary>
        /// 流程模板Id
        /// </summary>
        public long templateId { get; set; }
        /// <summary>
        /// 目标流程发起人：为空则为当前流程的发起人，传入人员表单id
        /// </summary>
        public string creator { get; set; }
        /// <summary>
        /// 目标流程自选人
        /// </summary>
        public NewAssignUser[] assign { get; set; }
        /// <summary>
        /// 目标流程表单初始化
        /// </summary>
        public NewFlowItem[] items { get; set; }
        /// <summary>
        /// 返回目标流程编码
        /// </summary>
        public string flowas { get; set; }
    }
    public class NewAssignUser
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        public string NodeId { get; set; }
        /// <summary>
        /// 自选节点名称
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 映射的人员表单id
        /// </summary>
        public string fieldid { get; set; }
    }
    public class NewFlowItem
    {
        /// <summary>
        /// 目标表单id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 目标表单名
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 目标表单类型
        /// </summary>
        public string eltype { get; set; }
        /// <summary>
        /// 值映射的表单id
        /// </summary>
        public string fieldid { get; set; }
    }
    public class TriggerEmailProps
    {
        public string subject { get; set; }
        public string[] to { get; set; }
        public string content { get; set; }
    }
    public class TriggerHttpProps
    {
        /// <summary>
        /// 请求方法 支持GET/POST
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// URL地址，可以直接带参数
        /// </summary>
        public string url { get; set; }
        public TriggerDataItem[] headers { get; set; }
        /// <summary>
        /// 请求参数类型
        /// </summary>
        public string contentType { get; set; }
        public TriggerDataItem[] xparams { get; set; }
        public int retry { get; set; }
        /// <summary>
        /// 是否自定义脚本
        /// </summary>
        public bool handlerByScript { get; set; }
        public string script { get; set; }
    }

    public class TriggerDataItem
    {
        public string name { get; set; }
        public bool isField { get; set; }
        /// <summary>
        /// 支持表达式 ${xxx} xxx为表单字段名称
        /// </summary>
        public string value { get; set; }
    }
}
