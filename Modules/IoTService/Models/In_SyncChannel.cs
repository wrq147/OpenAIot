using ChannelUtility.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_SyncChannel
    {
        public string code { get; set; }
        public ChannelConfig data { get; set; }
    }
}
