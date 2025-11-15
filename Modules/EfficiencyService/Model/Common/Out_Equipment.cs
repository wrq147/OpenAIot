using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_Equipment : T_Com_Equipment
    {
        /// <summary>
        /// 政策名称
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 排放因子名称
        /// </summary>
        public string FactorName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 联网状态：0为离线，1为在线，2为未初始化
        /// </summary>
        public string Online { get; set; }
    }
}
