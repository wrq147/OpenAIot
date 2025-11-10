using MyAccess.DB.Attr;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 组织扩展信息
    /// </summary>
    [TableName("mz_card_org")]
    public class MZ_Card_Org
    {
        /// <summary>
        /// 对应的组织Id
        /// </summary>
        [ID(false)]
        public long? OrgId { get; set; }
        /// <summary>
        /// 绑定的组织Id
        /// </summary>
        public string BindId { get; set; }
        /// <summary>
        /// 产品模块配置
        /// </summary>
        public string ProConfig { get; set; }
        /// <summary>
        /// 案例模块配置
        /// </summary>
        public string CaseConfig { get; set; }
    }
}
