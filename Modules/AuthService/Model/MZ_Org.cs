using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
namespace AuthService
{
    [TableName("mz_org")]
    public class MZ_Org : BaseEntity
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 组织关键字
        /// </summary>
        public string KeyWords { get; set; }
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
        /// 经纬度的geo编码
        /// </summary>
        public string Geo { get; set; }
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
        /// 企业简介
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// 组织认证状态（0未认证 1为认证中 2为已认证）
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public string del_flag { get; set; }
        /// <summary>
        /// 创建人信息
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo Creator { get; set; }
    }
}
