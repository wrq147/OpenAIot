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
    /// 报警事件参数
    /// </summary>
    public class AlarmReceivedEventArgs : EventArgs
    {
        public AlarmInfo Alarm { get; set; }
        public XDocument OriginalXml { get; set; }
    }
}
