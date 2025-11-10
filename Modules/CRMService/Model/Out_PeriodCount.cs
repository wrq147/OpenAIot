using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    /// <summary>
    /// 销售阶段数据
    /// </summary>
    public class Out_PeriodCount
    {
        /// <summary>
        /// 阶段Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 阶段名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}
