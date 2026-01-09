using GB28181Channel.GB28181.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GB28181Channel.GB28181.Event
{
    /// <summary>
    /// 目录查询事件参数
    /// </summary>
    public class CatalogReceivedEventArgs : EventArgs
    {
        public string DeviceId { get; set; }
        public List<ChannelInfo> Channels { get; set; }
    }
}
