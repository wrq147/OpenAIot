using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class In_DevPlaneTask : BaseQueryParam
    {
        /// <summary>
        /// 过滤流程发起人
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 过滤设备Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 计划类型Id
        /// </summary>
        public string PlanTypeId { get; set; }
        /// <summary>
        /// 状态：0进行中、1待执行、2执行中、3已完成、4已过期、5已验收、6验收失败、7已作废
        /// </summary>
        public byte? TaskStatus { get; set; }
        /// <summary>
        /// 过滤流程模板Id
        /// </summary>
        public long? FlowTemplateId { get; set; }
    }
}
