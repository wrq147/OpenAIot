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
        // 计算框的中心
        public static Vector2 GetCenter(this BoxItem item) => new Vector2((item.x1 + item.x2) / 2, (item.y1 + item.y2) / 2);
        // 计算框的面积
        public static float GetArea(this BoxItem item) => (item.x2 - item.x1) * (item.y2 - item.y1);
    }
}
