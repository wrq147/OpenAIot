using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    public class Out_WorkTaskInfo
    {
        /// <summary>
        /// 总报工良品数
        /// </summary>
        public decimal? TotalGoodNum { get; set; }
        /// <summary>
        /// 总报工不良品数
        /// </summary>
        public decimal? TotalDefectNum { get; set; }
        /// <summary>
        /// 总报工时长
        /// </summary>
        public decimal? TotalWorkTime { get; set; }
    }
}
