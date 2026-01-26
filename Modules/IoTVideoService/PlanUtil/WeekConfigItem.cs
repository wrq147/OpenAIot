using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.PlanUtil
{
    /// <summary>
    /// 按周配置项模型
    /// </summary>
    public class WeekConfigItem
    {
        public int week { get; set; }    // 星期值：0-周日，1-周一...6-周六
        public int Time { get; set; }   // 小时数（0-24）
        public string Op { get; set; }  // 操作类型：Start/End/Both
    }
}
