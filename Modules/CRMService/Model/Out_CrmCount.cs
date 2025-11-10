using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class Out_CrmCount
    {
        /// <summary>
        /// 总客户数
        /// </summary>
        public int KfCount { get; set; }
        /// <summary>
        /// 总线索数
        /// </summary>
        public int ClueCount { get; set; }
        /// <summary>
        /// 总商机数
        /// </summary>
        public int OpportCount { get; set; }
    }
}
