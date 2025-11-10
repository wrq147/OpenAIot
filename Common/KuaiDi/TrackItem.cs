using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.KuaiDi
{
    public class TrackItem
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 信息内容
        /// </summary>
        public string context { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 坐标信息
        /// </summary>
        public string areaCenter { get; set; }
    }
}
