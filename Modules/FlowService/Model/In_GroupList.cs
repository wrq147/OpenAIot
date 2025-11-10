using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    public class In_GroupList
    {
        /// <summary>
        /// 是否包含Item信息
        /// </summary>
        public bool? withItems { get; set; }
        /// <summary>
        /// 过滤组织
        /// </summary>
        public long? orgId { get; set; }

    }
}
