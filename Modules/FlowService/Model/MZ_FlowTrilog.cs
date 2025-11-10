using MyAccess.DB.Attr;
using System;

namespace FlowService.Model
{
    [TableName("mz_flow_trilog")]
    public class MZ_FlowTrilog
    {
        [ID(false)]
        public string FlowNumber { get; set; }
        /// <summary>
        /// 子流程创建记录
        /// </summary>
        public string TemplateIdSet { get; set; }
    }
}
