using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    public class In_RecordFilePage : BaseQueryParam
    {
        public string VideoId { get; set; }
        public string PlanId { get; set; }
        public string VideoKey { get; set; }
        /// <summary>
        /// 过滤日期
        /// </summary>
        public DateTime? FileDate { get; set; }
    }
}
