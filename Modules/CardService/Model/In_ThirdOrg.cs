using System;
using System.Collections.Generic;

namespace CardService.Model
{
    /// <summary>
    /// 第三方组织信息
    /// </summary>
    public class In_ThirdOrg
    {
        /// <summary>
        /// 第三方组织Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 第三方组织名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 第三方组织Logo
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// 第三方地址
        /// </summary>
        public string AddressDetail { get; set; }
    }
}
