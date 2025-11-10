using System;
using System.Collections.Generic;
using System.Linq;

namespace IoTService.Models
{
    public class In_SaveTags
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 标签值列表
        /// </summary>
        public List<In_TagItem> list { get; set; }
        /// <summary>
        /// 标签修改时间（不传为当前时间）
        /// </summary>
        public DateTime? indate { get; set; }
    }

}
