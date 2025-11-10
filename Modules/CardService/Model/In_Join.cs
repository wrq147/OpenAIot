using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Model
{
    /// <summary>
    /// 加入企业的参数
    /// </summary>
    public class In_Join
    {
        /// <summary>
        /// 企业Id
        /// </summary>
        public long orgId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public long? depId { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string postName { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string realName { get; set; }
    }
}
