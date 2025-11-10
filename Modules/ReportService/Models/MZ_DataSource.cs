using Common.Share;
using MyAccess.DB.Attr;

namespace ReportService.Models
{
    /// <summary>
    /// 数据库源
    /// </summary>
    [TableName("mz_data_source")]
    public class MZ_DataSource : BaseEntity
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 连接名称
        /// </summary>
        public string LinkName { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DatabaseType { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int? Port { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
