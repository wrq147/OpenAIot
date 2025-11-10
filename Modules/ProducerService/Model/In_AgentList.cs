using Common.Share;
using System;
namespace ProducerService.Model
{
    public class In_AgentList : BaseQueryParam
    {
        /// <summary>
        /// 过滤代理商
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 上级企业Id
        /// </summary>
        public long? ParentOrgId { get; set; }
    }
}
