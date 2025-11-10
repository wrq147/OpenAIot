using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class In_FollowPlanList : BaseQueryParam
    {
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime? PlanStartTime { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime? PlanEndTime { get; set; }
        /// <summary>
        /// A待完成、F已完成
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 过滤搜索关键字
        /// </summary>
        public string Key { get; set; }
    }
}
