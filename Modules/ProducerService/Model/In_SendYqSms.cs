using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_SendYqSms
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 邀请链接（无需参数）
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string code { get; set; }
    }
}
