using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    /// <summary>
    /// 企业微信部门成员
    /// </summary>
    public class Out_CorpDeptUser
    {
        /// <summary>
        /// 成员UserID。对应管理端的账号
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 成员名称
        /// </summary>
        public string name { get; set; }
    }
}
