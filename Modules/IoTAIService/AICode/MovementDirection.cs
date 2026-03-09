using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    /// <summary>
    /// 运动方向枚举（8个主方向）
    /// </summary>
    public enum MovementDirection
    {
        Unknown,    // 未知
        Up,         // 上
        Down,       // 下
        Left,       // 左
        Right,      // 右
        UpLeft,     // 左上
        UpRight,    // 右上
        DownLeft,   // 左下
        DownRight   // 右下
    }
}
