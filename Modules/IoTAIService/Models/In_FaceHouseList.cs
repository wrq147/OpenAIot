using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Models
{
    public class In_FaceHouseList
    {
        public string Key { get; set; }
        /// <summary>
        /// 状态（0禁用、1启用）
        /// </summary>
        public string Status { get; set; }
    }
}
