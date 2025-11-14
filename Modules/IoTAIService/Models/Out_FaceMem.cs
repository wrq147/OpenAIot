using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Models
{
    public class Out_FaceMem
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
    }
}
