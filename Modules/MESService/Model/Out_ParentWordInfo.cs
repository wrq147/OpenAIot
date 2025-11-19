using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    public class Out_ParentWordInfo
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string WorkNumber { get; set; }
        public string ProductName { get; set; }
        public string PlanName { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime? PlannedStartOn { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime? PlannedEndOn { get; set; }
    }
}
