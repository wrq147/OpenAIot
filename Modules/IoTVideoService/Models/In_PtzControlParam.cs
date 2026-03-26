using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    public class In_PtzControlParam
    {
        public string SourceId { get; set; }
        public string VideoKey { get; set; }
        /// <summary>
        /// ptz控制命令
        /// </summary>
        public int Cmd { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// 预置位ID
        /// </summary>
        public string PresetId { get; set; }
    }
}
