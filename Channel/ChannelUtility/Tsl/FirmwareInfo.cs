using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Tsl
{
    public class FirmwareInfo
    {
        /// <summary>
        /// 固件标识，可以用版本号标识
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 固件标签，表示固件类型
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// 固件下载路径
        /// </summary>
        public string url { get; set; }
    }
}
