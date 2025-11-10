using MyAccess.DB.Attr;
using System;

namespace AuthService
{
    /// <summary>
    /// 登录日志
    /// </summary>
    [TableName("mz_login_log")]
    public class MZ_LoginLog
    {
        [ID(true)]
        public long? SysLogID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public long? UserId { get; set; }
        [DataIgnore]
        public string UserName { get; set; }
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 登录状态（0成功 1失败）
        /// </summary>
        public byte? Status { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 登录地点
        /// </summary>
        public string IPLocation { get; set; }
        /// <summary>
        /// 浏览器类型
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 操作系统
        /// </summary>
        public string OS { get; set; }
        /// <summary>
        /// 终端设备
        /// </summary>
        public string Terminal { get; set; }
        /// <summary>
        /// 提示消息
        /// </summary>
        public string Info { get; set; }
    }
}
