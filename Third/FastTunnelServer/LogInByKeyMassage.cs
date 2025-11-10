using FastTunnel.Core.Models.Massage;
using System;


namespace FastTunnelServer
{
    public class LogInByKeyMassage : TunnelMassage
    {
        public string key { get; set; }
        public string md5devkey { get; set; }
        public int webport { get; set; }
    }
}
