using FlowService.FlowNode.ActionOfData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 执行修改、删除指定数据库表的记录
    /// </summary>
    public class DataNode : FlowBaseNode
    {
        public DataProps props { get; set; }
    }
    public class DataProps
    {
        /// <summary>
        /// 目标表单
        /// </summary>
        public string targetform { get; set; }
        /// <summary>
        /// Update、Delete
        /// </summary>
        public string action { get; set; }
        /// <summary>
        /// 过滤条件
        /// </summary>
        public DataCondition[] conditions { get; set; }
        /// <summary>
        /// 执行插入或更新的字段
        /// </summary>
        public DataAction[] fields { get; set; }
    }
}
