using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_SavePropRules
    {
        /// <summary>
        /// 所属协议Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 关联属性
        /// </summary>
        public string PropCode { get; set; }
        /// <summary>
        /// 规则数组
        /// </summary>
        public MZ_IotWinRule[] Rules { get; set; }
    }
}
