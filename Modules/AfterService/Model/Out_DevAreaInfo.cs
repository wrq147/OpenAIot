using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    /// <summary>
    /// 设备区域统计信息
    /// </summary>
    public class Out_DevAreaInfo
    {
        /// <summary>
        /// 在线数量
        /// </summary>
        public int onCount { get; set; }
        /// <summary>
        /// 离线数量
        /// </summary>
        public int offCount { get; set; }
        /// <summary>
        /// 未初始化数量
        /// </summary>
        public int unCount { get; set; }
    }
}
