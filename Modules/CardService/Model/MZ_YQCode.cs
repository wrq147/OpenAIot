using Newtonsoft.Json;
using System;
namespace CardService.Model
{
    /// <summary>
    /// 邀请码
    /// </summary>
    public class MZ_YQCode
    {
        /// <summary>
        /// 邀请加入的企业
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? OrgId { get; set; }
        /// <summary>
        /// 邀请的企业名称
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OrgName { get; set; }
        /// <summary>
        /// 邀请的部门
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? DeptId { get; set; }
        /// <summary>
        /// 邀请的部门名称
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DeptName { get; set; }
        /// <summary>
        /// 是否限制只能一人加入
        /// </summary>
        public bool Limit { get; set; }
        /// <summary>
        /// 已邀请人数
        /// </summary>
        public int YQCount { get; set; }
        /// <summary>
        /// 邀请码类型：0为4位邀请码,1为GUID
        /// </summary>
        public int YQCodeType { get; set; }
    }
}
