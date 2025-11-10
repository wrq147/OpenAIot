using MyAccess.DB.Attr;
using System;

namespace FlowService.Model
{
    [TableName("mz_flow_template_link")]
    public class MZ_FlowTemplateLink
    {
        public long? TemplateId { get; set; }
        /// <summary>
        /// U为用户，D为部门
        /// </summary>
        public string LinkType { get; set; }
        public long? LinkId { get; set; }
    }
}
