#if UNITY_5
using System;

using LibGB28181SipServer.System.Hardware;

using UnityEngine;

namespace LibGB28181SipServer.System.OperatingSystem
{
    internal class UnityOperatingSystemInfo : OperatingSystemInfo
    {
        public sealed override string Architecture => "Unknown";

        public override string Name => SystemInfo.operatingSystem;

        public override string Runtime => "Unknown";

        private Version _javaVersion;
        public override Version JavaVersion => _javaVersion ?? (_javaVersion = new Version());

        private HardwareInfo _hardware;
        public override HardwareInfo Hardware => _hardware ?? (_hardware = new UnityHardwareInfo());


        public override OperatingSystemInfo Update()
        {
            _hardware = null;

            return this;
        }
    }
}

#endif