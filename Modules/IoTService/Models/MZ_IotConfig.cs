using MyAccess.DB.Attr;
using System;


namespace IoTService.Models
{
    [TableName("mz_iot_config")]
    public class MZ_IotConfig
    {
        /// <summary>
        /// 企业Id
        /// </summary>
        [ID(false)]
        public long? OrgId { get; set; }
        /// <summary>
        /// 是否自动增加物联卡
        /// </summary>
        public bool? EnableAutoAdd { get; set; }
        /// <summary>
        /// 移动物联卡接口配置
        /// </summary>
        public string YiDongOption { get; set; }
        /// <summary>
        /// SimBoss物联卡接口配置
        /// </summary>
        public string SimBossOption { get; set; }
        /// <summary>
        /// SohanOption物联卡接口配置
        /// </summary>
        public string SohanOption { get; set; }
        /// <summary>
        /// UnicomOption联通卡接口配置
        /// </summary>
        public string UnicomOption { get; set; }
    }

    public class YiDongOption
    {
        public string app_id { get; set; }
        public string password { get; set; }
        /// <summary>
        /// 初始到期时间
        /// </summary>
        public int expire_month { get; set; }
        /// <summary>
        /// 是否启用过期
        /// </summary>
        public bool enable_expire { get; set; }
    }

    public class SimBossOption
    {
        public string app_id { get; set; }
        public string app_secret { get; set; }
    }

    public class SohanOption
    {
        public string app_id { get; set; }
        public string app_secret { get; set; }
        /// <summary>
        /// 初始到期时间
        /// </summary>
        public int expire_month { get; set; }
        /// <summary>
        /// 是否启用过期
        /// </summary>
        public bool enable_expire { get; set; }
    }
    public class UnicomOption
    {
        public string appUrl { get; set; }
        public string app_id { get; set; }
        public string app_secret { get; set; }
        /// <summary>
        /// 协议Id
        /// </summary>
        public string tenantId { get; set; }
        public string encodeKey{ get; set; }
        public string ivKey { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        /// <summary>
        /// 初始到期时间
        /// </summary>
        public int expire_month { get; set; }
        /// <summary>
        /// 是否启用过期
        /// </summary>
        public bool enable_expire { get; set; }
    }
}
