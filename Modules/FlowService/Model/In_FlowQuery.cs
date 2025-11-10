using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    public class In_FlowQuery
    {
        /// <summary>
        /// 过滤流程Id
        /// </summary>
        public long? FlowId { get; set; }
        /// <summary>
        /// 过滤模板Id
        /// </summary>
        public long? TemplateId { get; set; }
        /// <summary>
        /// 过滤参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 过滤参数值
        /// </summary>
        public string Value { get; set; }
    }
}
