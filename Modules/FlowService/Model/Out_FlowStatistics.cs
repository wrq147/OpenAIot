using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    /// <summary>
    /// 工作台简报
    /// </summary>
    public class Out_FlowStatistics
    {
        /// <summary>
        /// 我创建的流程数
        /// </summary>
        public int MyFlowCount { get; set; }
        /// <summary>
        /// 我创建的未开始数
        /// </summary>
        public int MyWaitCount { get; set; }
        /// <summary>
        /// 我创建的进行中数
        /// </summary>
        public int MyDoingCount { get; set; }
        /// <summary>
        /// 我创建的已完成数
        /// </summary>
        public int MyFinishCount { get; set; }
        /// <summary>
        /// 我创建的已取消数
        /// </summary>
        public int MyCancelCount { get; set; }
        /// <summary>
        /// 待我处理的数
        /// </summary>
        public int PendingCount { get; set; }
        /// <summary>
        /// 抄送我的数
        /// </summary>
        public int CopyCount { get; set; }
        /// <summary>
        /// 我处理完的数
        /// </summary>
        public int FinishCount { get; set; }
    }
}
