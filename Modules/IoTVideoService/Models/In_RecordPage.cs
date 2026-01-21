using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    public class In_RecordPage : BaseQueryParam
    {
        public string VideoId { get; set; }
        /// <summary>
        /// 时段类型：week（按周）、time（按时段）
        /// </summary>
        public string RecordTimeType { get; set; }
        /// <summary>
        /// 状态：0-禁用，1-启用
        /// </summary>
        public int? Status { get; set; }
    }
}
