using CRMService.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class In_ClueTrans
    {
        /// <summary>
        /// 线索Id
        /// </summary>
        public string ClueId { get; set; }
        /// <summary>
        /// 是否在客户中同步显示跟进
        /// </summary>
        public bool SyncFollow { get; set; }
        /// <summary>
        /// 客户信息
        /// </summary>
        public MZ_Customer Customer { get; set; }
    }
}
