using Common.Share;
using System;

namespace MonitorService.Model
{
    public class In_OperLogList : BaseQueryParam
    {
        public string title { get; set; }
        public int? status { get; set; }
        public string operName { get; set; }
    }
}
