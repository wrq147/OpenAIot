using Common.Share;
using System;

namespace MonitorService.Model
{
    public class In_JobLogList : BaseQueryParam
    {
        public string jobName { get; set; }
        public string jobGroup { get; set; }
        public string status { get; set; }
        public string invoke_target { get; set; }
    }
}
