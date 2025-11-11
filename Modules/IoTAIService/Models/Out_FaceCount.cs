using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Models
{
    public class Out_FaceCount
    {
        /// <summary>
        /// 所属模型库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 总人脸数
        /// </summary>
        public int? TotalFace { get; set; }
    }
}
