using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_CopyHis
    {
        public string SourceId{ get; set; }
        public string TargetId { get; set; }
        /// <summary>
        /// 源属性标识与目标属性标识的映射
        /// </summary>
        public Dictionary<string, string> Mapping { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
