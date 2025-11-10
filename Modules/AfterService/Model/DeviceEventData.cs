using System;
using System.Collections.Generic;

namespace AfterService.Model
{
    public class DeviceEventData
    {
        public string DtuId { get; set; }
        public string DevId { get; set; }
        public string DevNumber { get; set; }
        public long OrgId { get; set; }
        public long OwnerOrgId { get; set; }
        public long UseOrgId { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
        public string EventInfo { get; set; }
        public Dictionary<string,object> EventOutput { get; set; }
    }
}
