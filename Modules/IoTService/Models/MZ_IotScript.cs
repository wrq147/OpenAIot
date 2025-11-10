using Common.Share;
using MyAccess.DB.Attr;
using System;
namespace IoTService.Models
{
    /// <summary>
    /// 物联网脚本模板
    /// </summary>
    [TableName("mz_iot_script")]
    public class MZ_IotScript : BaseEntity
    {
        /// <summary>
        /// 模板Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 创建者组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 是否为系统模板
        /// </summary>
        public string IsSystem { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 模板介绍
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 搜索用关键字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 脚本内容
        /// </summary>
        public string ScriptContent { get; set; }
        /// <summary>
        /// 物模型初始化
        /// </summary>
        public string InitModelTSL { get; set; }
    }
}
