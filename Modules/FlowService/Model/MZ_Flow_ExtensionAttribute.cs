using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    /// <summary>
    /// 节点扩展信息
    /// </summary>
    [TableName("mz_flow_extension_attr")]
    public class MZ_Flow_ExtensionAttribute
    {
        [ID(true)]
        public long? Id { get; set; }
        /// <summary>
        /// 对应的节点Id
        /// </summary>
        public long? ExecutionNodeId { get; set; }

        public string AttributeKey { get; set; }

        public string AttributeValue { get; set; }
        public long? FlowId { get; set; }
    }
}
