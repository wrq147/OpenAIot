using FastTunnel.Core.Models;
using FastTunnel.Core.Models.Massage;
using System;
using System.Collections.Generic;

namespace ModbusChannel
{
    public class LogInByKeyMassage : TunnelMassage
    {
        public string key { get; set; }
        public string md5devkey { get; set; }
        public int webport { get; set; }
    }
}
