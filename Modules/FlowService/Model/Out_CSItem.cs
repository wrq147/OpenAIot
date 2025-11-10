using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    public class Out_CSItem : MZ_Flow_ExtensionAttribute
    {
        public long TemplateId { get; set; }
        public string FlowName { get; set; }
        public string StepName { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public string startRealName { get; set; }
        /// <summary>
        /// 发起人部门
        /// </summary>
        public string startDeptName { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        public long createId { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string Background { get; set; }
    }
}
