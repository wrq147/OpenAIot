#if UNITY_5
using UnityEngine;

namespace LibGB28181SipServer.System.Hardware.RAM
{
    internal class UnityRAMInfo : RAMInfo
    {
        public override ulong Free => 0;

        public override ulong Total => (ulong) SystemInfo.systemMemorySize*1024;
    }
}

#endif