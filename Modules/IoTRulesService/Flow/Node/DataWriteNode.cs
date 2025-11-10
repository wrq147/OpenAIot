using ChannelUtility;
using System;
using System.Collections.Generic;

namespace IoTRulesService.Flow.Node
{
    /// <summary>
    /// 服务端修改参数
    /// </summary>
    public class DataWriteNode : RuleBaseNode
    {
        public WriteProps props { get; set; }
    }

    public class WriteProps
    {
        public List<WriteCount> Counts { get; set; }
        public List<WriteCode> Writes { get; set; }
    }
    public class WriteCode
    {
        /// <summary>
        /// 参数标识符
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 值或计算表达式,$data表示原数据
        /// </summary>
        public string express { get; set; }
    }
    public class WriteCount
    {
        /// <summary>
        /// 参数标识符
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 聚合数据Id
        /// </summary>
        public string nodeId { get; set; }
        /// <summary>
        /// 计算方式：sum求和、average平均
        /// </summary>
        public string calway { get; set; }
    }
}
