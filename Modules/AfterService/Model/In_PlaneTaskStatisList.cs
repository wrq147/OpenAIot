using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class In_PlaneTaskStatisList
    {
        /// <summary>
        /// 过滤计划类型Id
        /// </summary>
        public string PlaneTypeId { get; set; }
        /// <summary>
        /// 过滤计划任务开始的起始时间
        /// </summary>
        public DateTime? CreaetStart { get; set; }
        /// <summary>
        /// 过滤计划任务开始的结束时间
        /// </summary>
        public DateTime? CreaetEnd { get; set; }
    }
}
