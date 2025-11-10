using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class RuleItem
    {
        public long? Id { get; set; }
        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数json字符串
        /// </summary>
        public string HttpParams { get; set; }
        /// <summary>
        /// 规则优先级(越小越前）
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 状态（0正常 1暂停）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
