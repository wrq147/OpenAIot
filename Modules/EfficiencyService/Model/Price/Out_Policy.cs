using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_Policy: T_Price_Policy
    {
        /// <summary>
        /// 能源类型名称
        /// </summary>
        public string TypeName { get; set; }
    }
}
