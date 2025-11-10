using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    [TableName("mz_flow_query")]
    public class MZ_FlowQuery
    {
        [ID(false)]
        public long? FlowId { get; set; }
        /// <summary>
        /// 请求参数名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 请求参数值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 流程模板
        /// </summary>
        public long? TemplateId { get; set; }
    }
}
