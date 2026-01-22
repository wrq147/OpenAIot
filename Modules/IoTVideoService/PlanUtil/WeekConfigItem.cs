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
        public int Week { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
