using AuthService;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 名片使用的组织信息
    /// </summary>
    public class MZ_Card_Org_V : MZ_Org
    {
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
