using MyAccess.DB.Attr;
using System;


namespace MonitorService.Model
{
    [TableName("mz_oper_log")]
    public class MZ_OperLog
    {
        /// <summary>
        /// 日志主键
        /// </summary>
        [ID(true)]
        public long? oper_id { get; set; }
        /// <summary>
        /// 模块标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public string request_method { get; set; }
        /// <summary>
        /// 操作类别（pc weixin android ios）
        /// </summary>
        public string operator_type { get; set; }
        /// <summary>
        /// 操作人员
        /// </summary>
        public string oper_name { get; set; }
        /// <summary>
        /// 操作人员ID
        /// </summary>
        public long? oper_uid { get; set; }
        /// <summary>
        /// 操作人员所属组织
        /// </summary>
        public long? oper_org { get; set; }
        /// <summary>
        /// 请求URL
        /// </summary>
        public string oper_url { get; set; }
        /// <summary>
        /// 主机地址
        /// </summary>
        public string oper_ip { get; set; }
        /// <summary>
        /// 操作地点
        /// </summary>
        public string oper_location { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string oper_param { get; set; }
        /// <summary>
        /// 返回参数
        /// </summary>
        public string json_result { get; set; }
        /// <summary>
        /// 操作状态（0正常 1异常）
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string error_msg { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime oper_time { get; set; }
        [DataIgnore]
        public object[] TmpParamObject { get; set; }
    }
}
