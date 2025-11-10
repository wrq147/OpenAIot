using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_RechargeData
    {
        /// <summary>
        /// 需要续费的卡Id数组
        /// </summary>
        public string[] Ids { get; set; }
        /// <summary>
        /// 需要续费几个月
        /// </summary>
        public int Month { get; set; }
    }
}
