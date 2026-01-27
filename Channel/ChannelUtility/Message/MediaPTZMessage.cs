using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaPTZMessage : BaseDeviceMessage
    {
        public string UserName { get; set; }
        public string VideoKey { get; set; }
        public PTZCommandType CommandType { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// 预置位ID(仅预置位指令有效)
        /// </summary>
        public byte? PresetId { get; set; }
        public MediaPTZMessage()
        {
            MsgType = "MediaPTZ";
        }
    }

    /// <summary>
    /// PTZ控制命令类型
    /// </summary>
    public enum PTZCommandType
    {
        Halt = 0,          // 停止
        Right = 1,         // 右
        RightUp = 2,       // 右上
        Up = 3,            // 上
        LeftUp = 4,        // 左上
        Left = 5,          // 左
        LeftDown = 6,      // 左下
        Down = 7,          // 下
        RightDown = 8,     // 右下
        Zoom = 9,          // 变焦
        Iris = 10,         // 光圈
        Focus = 11,        // 聚焦
        PresetSet = 12,    // 设置预置位
        PresetGoto = 13,   // 调用预置位
        PresetClear = 14   // 清除预置位
    }
}
