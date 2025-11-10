using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class In_DevRangeList
    {
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 地图绽放级别，最小为1，最大为9，默认为7
        /// </summary>
        public int level { get; set; } = 7;

        /// <summary>
        /// 过滤联网状态：0为离线，1为在线，2为未初始化
        /// </summary>
        public byte? NetStatus { get; set; }
        /// <summary>
        /// 过滤运行状态
        /// </summary>
        public string DState { get; set; }
    }
}
