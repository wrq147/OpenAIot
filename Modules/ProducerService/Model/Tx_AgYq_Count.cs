using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class Tx_AgYq_Count
    {
        /// <summary>
        /// 开始计时的时间
        /// </summary>
        public DateTime first_send { get; set; }
        /// <summary>
        /// 1小时内
        /// </summary>
        public int count { get; set; }
    }
}
