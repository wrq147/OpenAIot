using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Text.Json.Serialization;

namespace CardService.Model
{
    /// <summary>
    /// 名片
    /// </summary>
    [TableName("mz_card")]
    public class MZ_Card : BaseEntity
    {
        /// <summary>
        /// Id主键
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string WxNumber { get; set; }
        /// <summary>
        /// 官网
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// 名片头像
        /// </summary>
        [JsonConverter(typeof(AvatarUrl))]
        public string Avatar { get; set; }
        /// <summary>
        /// 名片风格
        /// </summary>
        public int? TemplateId { get; set; }
        /// <summary>
        /// 名片风格背景图
        /// </summary>
        public int? TemplateBk { get; set; }
        /// <summary>
        /// 所属用户Id
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 所属组织,为0则无所属企业
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属组织名称
        /// </summary>
        [DataIgnore]
        public string OrgName { get; set; }
        /// <summary>
        /// 所属部门Id
        /// </summary>
        [DataIgnore]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? DeptId { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 职位,多个职位用逗号分隔
        /// </summary>
        public string PostName { get; set; }
        /// <summary>
        /// 分享标题
        /// </summary>
        public string ShareTitle { get; set; }
        /// <summary>
        /// 个人简介
        /// </summary>
        public string Intro { get; set; }
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
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        [JsonIgnore]
        public string del_flag { get; set; }
    }
}
