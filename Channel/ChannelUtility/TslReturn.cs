using ChannelUtility.Tsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility
{
    public class TslReturn
    {
        public string ProductId { get; set; }
        public TslModel Model { get; set; }
        public string Status { get; set; }
        public string script { get; set; }
        public string NetworkWay { get; set; }
        public TslReturn(string productId, TslModel model, string status, string script, string netway)
        {
            this.ProductId = productId;
            this.Model = model;
            this.Status = status;
            this.script = script;
            this.NetworkWay = netway;
        }
    }
}
