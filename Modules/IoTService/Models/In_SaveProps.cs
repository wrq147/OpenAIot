using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_SaveProps
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 设备属性键值对
        /// </summary>
        public Dictionary<string, object> NewVals { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public DateTime? Indate { get; set; }
    }
}
