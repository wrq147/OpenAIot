using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    /// <summary>
    /// 销售简报
    /// </summary>
    public class Out_CrmStatistics
    {
        /// <summary>
        /// 新增客户数
        /// </summary>
        public int NewKfCount { get; set; }
        /// <summary>
        /// 新增商机数
        /// </summary>
        public int NewOpportCount { get; set; }
        /// <summary>
        /// 新增跟进数
        /// </summary>
        public int NewFollowCount { get; set; }
        /// <summary>
        /// 预测总金额
        /// </summary>
        public decimal MaybeTotalPrice { get; set; }
        /// <summary>
        /// 跟进线索数
        /// </summary>
        public int FollowClueCount { get; set; }
        /// <summary>
        /// 跟进客户数
        /// </summary>
        public int FollowCustomerCount { get; set; }
        /// <summary>
        /// 跟进商机数
        /// </summary>
        public int FollowOpportCount { get; set; }
        /// <summary>
        /// 商机赢单数
        /// </summary>
        public int WinOpportCount { get; set; }
    }

}
