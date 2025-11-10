using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    /// <summary>
    /// 企业的CRM配置
    /// </summary>
    public class MZ_CrmConf
    {
        /// <summary>
        /// 跨阶段推进
        /// </summary>
        public bool EnableKPer { get; set; }
        /// <summary>
        /// 进行中阶段回退
        /// </summary>
        public bool EnableIngBack { get; set; }
        /// <summary>
        /// 终点阶段回退
        /// </summary>
        public bool EnableWinBack { get; set; }
        /// <summary>
        /// 设置几天未跟进将被回收，为0不回收
        /// </summary>
        public int FollowReturnDay { get; set; }

    }
}
