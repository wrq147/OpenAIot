using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_AgentFlowList : BaseQueryParam
    {
        /// <summary>
        /// 邀请来源：0不过滤，1为生产商的代理邀请，2为CRM的邀请客户
        /// </summary>
        public int YQFrom { get; set; }
    }
}
