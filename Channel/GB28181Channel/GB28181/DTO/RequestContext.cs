using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.DTO
{

    /// <summary>
    /// 请求上下文，用于关联请求和响应
    /// </summary>
    public class RequestContext
    {
        public string RequestType { get; set; }
        public string DeviceId { get; set; }
        public string ChannelId { get; set; }
        public DateTime RequestTime { get; set; }
        public object ExtraData { get; set; }
    }

}
