using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.PlanUtil
{
    /// <summary>
    /// 录像时间操作类型枚举
    /// </summary>
    public enum RecordTimeOp
    {
        Start,  // 开始录像
        End,    // 结束录像
        Both    // 同时结束、开始
    }
}
