using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Models
{
    public class In_FaceList : BaseQueryParam
    {
        /// <summary>
        /// 按姓名搜索
        /// </summary>
        public string Name { get; set; }
    }
}
