using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    /// <summary>
    /// 设备区域数量信息
    /// </summary>
    public class Out_DevAreaData
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        [ColumnBy("AC")]
        public string AreaCode { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int? Count { get; set; }

    }
}
