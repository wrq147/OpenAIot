using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;

namespace ProducerService.Model
{
    /// <summary>
    /// 代理邀请、申请、升级流程
    /// </summary>
    [TableName("mz_agent_flow")]
    public class MZ_AgentFlow : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 上级代理Id
        /// </summary>
        public long? ParentOrgId { get; set; }
        /// <summary>
        /// 下级企业Id
        /// </summary>
        public long? BelowOrgId { get; set; }
        /// <summary>
        /// 代理级别，当为邀请时，空则为直销
        /// </summary>
        public string GradeId { get; set; }
        /// <summary>
        /// 生产商Id
        /// </summary>
        public long? FactoryId { get; set; }
        /// <summary>
        /// 代理区域代码（多个,号分隔）
        /// </summary>
        public string Regions { get; set; }
        /// <summary>
        /// I为邀请，A为申请，U为升级
        /// </summary>
        public string FromType { get; set; }
        /// <summary>
        /// 绑定的客户Id
        /// </summary>
        public string BindCustomerId { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string PhoneCode { get; set; }
        /// <summary>
        /// 状态：0为待处理，1为同意，2为拒绝
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? OverTime { get; set; }


        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 组织Logo
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string Logo { get; set; }
        /// <summary>
        /// 行业类型
        /// </summary>
        public int? Industry { get; set; }
        /// <summary>
        /// 员工规模
        /// </summary>
        public int? Size { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }
        /// <summary>
        /// 省市区代码
        /// </summary>
        public string AddressCode { get; set; }
        /// <summary>
        /// 地址名称
        /// </summary>
        public string AddressName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string AddressDetail { get; set; }
    }
}
