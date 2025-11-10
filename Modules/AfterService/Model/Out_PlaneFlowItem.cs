using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class Out_PlaneFlowItem
    {
        /// <summary>
        /// 待处理数量
        /// </summary>
        public int WaitDeal { get; set; }
        /// <summary>
        /// 派工流程模板Id
        /// </summary>
        public long? FlowTemplateId { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string Background { get; set; }
        /// <summary>
        /// 流程模板名称
        /// </summary>
        public string FlowTemplateName { get; set; }
    }
}
