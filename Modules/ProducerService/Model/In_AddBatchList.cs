using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_AddBatchList
    {
        /// <summary>
        /// 设备id（数组）
        /// </summary>
        public string[] Ids { get; set; }
        /// <summary>
        /// 批次所属产品Id
        /// </summary>
        public string ProductId { get; set; }
    }
}
