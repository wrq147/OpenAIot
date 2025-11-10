using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_SyncSaveTags
    {        
        /// <summary>
        /// 设备第三方编码
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// 标签值列表
        /// </summary>
        public List<In_TagItem> list { get; set; }
    }
}
