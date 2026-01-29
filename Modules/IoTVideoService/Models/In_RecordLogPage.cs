using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    public class In_RecordLogPage : BaseQueryParam
    {
        public string PlanId { get; set; }
        public string LogType { get; set; }
    }
}
