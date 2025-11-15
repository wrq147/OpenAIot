using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class In_FacilityBind
    {
        /// <summary>
        /// 设施编号
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string EquipmentId { get; set; }
    }
}
