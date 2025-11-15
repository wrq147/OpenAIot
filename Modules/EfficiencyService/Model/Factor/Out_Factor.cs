using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_Factor: T_ENG_Factor
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 排放因子单位
        /// </summary>
        public string FactorUnit { get; set; }

        /// <summary>
        /// 活动数据单位
        /// </summary>
        public string ActivityUnit { get; set; }
    }
}
