using ChannelUtility.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    public static class BoxItemExtensions
    {
        /// <summary>
        /// 检测框宽度
        /// </summary>
        public static float Width(this BoxItem item)
        {
            return Math.Max(0, item.x2 - item.x1);
        }

        /// <summary>
        /// 检测框高度
        /// </summary>
        public static float Height(this BoxItem item)
        {
            return Math.Max(0, item.y2 - item.y1);
        }

        /// <summary>
        /// 宽高比（Aspect Ratio
        /// </summary>
        public static float AspectRatio(this BoxItem item)
        {
            float h = item.Height();
            if (h < 1e-6f) return 1.0f; // 高度接近0时默认宽高比为1
            return item.Width() / h;
        }
        // 计算框的中心
        public static Vector2 GetCenter(this BoxItem item) => new Vector2((item.x1 + item.x2) / 2, (item.y1 + item.y2) / 2);
        // 计算框的面积
        public static float GetArea(this BoxItem item) => (item.x2 - item.x1) * (item.y2 - item.y1);
    }
}
