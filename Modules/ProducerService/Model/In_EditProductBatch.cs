using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_EditProductBatch
    {
        public string Id { get; set; }
        /// <summary>
        /// 所属产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 通讯编码
        /// </summary>
        public string DtuId { get; set; }
    }
}
