using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    /// <summary>
    /// 区域类型
    /// </summary>
    public enum RegionType
    {
        Rectangle,  // 矩形区域
        Polygon     // 多边形区域
    }

    /// <summary>
    /// 目标区域状态
    /// </summary>
    public enum RegionStatus
    {
        Outside,    // 在区域外
        Inside,     // 在区域内
        Entered,    // 刚进入区域（触发告警）
        Exited      // 刚离开区域
    }

    /// <summary>
    /// 监控区域实体
    /// </summary>
    public class MonitoringRegion
    {
        public string Id { get; set; } // 区域ID
        public string Name { get; set; } // 区域名称
        public RegionType Type { get; set; } // 区域类型
        public List<Vector2> Vertices { get; set; } // 顶点坐标（矩形：左上、右上、右下、左下；多边形：按顺序）
        public bool IsActive { get; set; } = true; // 是否启用该区域

        /// <summary>
        /// 矩形区域快捷创建
        /// </summary>
        /// <param name="x1">左</param>
        /// <param name="y1">上</param>
        /// <param name="x2">右</param>
        /// <param name="y2">下</param>
        /// <param name="id">区域ID</param>
        /// <param name="name">区域名称</param>
        /// <returns></returns>
        public static MonitoringRegion CreateRectangle(float x1, float y1, float x2, float y2, string id = "", string name = "默认矩形区域")
        {
            return new MonitoringRegion
            {
                Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id,
                Name = name,
                Type = RegionType.Rectangle,
                Vertices = new List<Vector2>
                {
                    new Vector2(x1, y1), // 左上
                    new Vector2(x2, y1), // 右上
                    new Vector2(x2, y2), // 右下
                    new Vector2(x1, y2)  // 左下
                }
            };
        }

        /// <summary>
        /// 多边形区域快捷创建
        /// </summary>
        /// <param name="vertices">顶点坐标列表</param>
        /// <param name="id">区域ID</param>
        /// <param name="name">区域名称</param>
        /// <returns></returns>
        public static MonitoringRegion CreatePolygon(List<Vector2> vertices, string id = "", string name = "默认多边形区域")
        {
            if (vertices.Count < 3)
                throw new ArgumentException("多边形区域至少需要3个顶点");

            return new MonitoringRegion
            {
                Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id,
                Name = name,
                Type = RegionType.Polygon,
                Vertices = vertices
            };
        }

        /// <summary>
        /// 判断点是否在区域内
        /// </summary>
        /// <param name="point">目标点（中心坐标）</param>
        /// <returns></returns>
        public bool ContainsPoint(Vector2 point)
        {
            if (!IsActive) return false;

            return Type switch
            {
                RegionType.Rectangle => IsPointInRectangle(point),
                RegionType.Polygon => IsPointInPolygon(point),
                _ => false
            };
        }

        // 矩形包含判断
        private bool IsPointInRectangle(Vector2 point)
        {
            float x1 = Vertices[0].X;
            float y1 = Vertices[0].Y;
            float x2 = Vertices[2].X;
            float y2 = Vertices[2].Y;

            return point.X >= x1 && point.X <= x2 && point.Y >= y1 && point.Y <= y2;
        }

        // 多边形包含判断（射线法）
        private bool IsPointInPolygon(Vector2 point)
        {
            bool inside = false;
            int count = Vertices.Count;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                Vector2 vi = Vertices[i];
                Vector2 vj = Vertices[j];

                // 检查点是否在边的端点上
                if (Math.Abs(point.X - vi.X) < 0.001f && Math.Abs(point.Y - vi.Y) < 0.001f)
                    return true;

                // 射线法核心逻辑
                if (((vi.Y > point.Y) != (vj.Y > point.Y)) &&
                    (point.X < (vj.X - vi.X) * (point.Y - vi.Y) / (vj.Y - vi.Y) + vi.X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
