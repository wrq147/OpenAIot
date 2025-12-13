using GB28181SipGate.Json;
using LibGB28181SipServer.Structs;
using LibGB28181SipServer.System.OperatingSystem;
using System;
using System.Text.Json;
using System.Threading;

namespace LibGB28181SipServer.System
{
    public class SystemInfo : IDisposable
    {
        private static OperatingSystemInfo _operatingSystemInfo = OperatingSystemInfo.GetOperatingSystemInfo();
        private static OperatingSystemType _operatingSystemType = _operatingSystemInfo.OperatingSystemType;
        private static PerformanceInfo _globalSystemInfo = new PerformanceInfo();
        private static object _lockObj = new object();
        private static bool _abort = false;


        public SystemInfo()
        {
            _globalSystemInfo.Architecture = _operatingSystemInfo.Architecture;
            _globalSystemInfo.OsName = _operatingSystemInfo.Name;
            _globalSystemInfo.CpuCores = Environment.ProcessorCount;
            _globalSystemInfo.FrameworkVersion = _operatingSystemInfo.Runtime.ToString();
            _globalSystemInfo.SystemType = _operatingSystemType.ToString();
            Thread thread = new Thread(GetInfo);
            thread.Start();
        }

        public void Dispose()
        {
            _abort = true;
            _operatingSystemInfo = null!;
            _globalSystemInfo = null!;
            _globalSystemInfo = null!;
        }

        ~SystemInfo()
        {
            Dispose(); //释放非托管资源
        }

        private MemoryInfo getMeminfo()
        {
            switch (_operatingSystemType)
            {
                case OperatingSystemType.Windows:
                    return MemoryWinValue.GetMemoryStatus();
                    break;
                case OperatingSystemType.Linux:
                    return MemoryLinuxValue.GetMemoryStatus();
                    break;
                case OperatingSystemType.MacOSX:
                    return MemoryMacValue.GetMemoryStatus();
                    break;
            }

            return null;
        }

        private void GetInfo()
        {
            ushort i = 0;
            ushort j = 0;
            while (true)
            {
                if (_abort)
                {
                    break;
                }

                i++;
                j++;
                if (ushort.MaxValue - i < 100)
                {
                    i = 0;
                }

                if (ushort.MaxValue - j < 100)
                {
                    j = 0;
                }

                lock (_lockObj)
                {
                    if ((j % 10 == 0 || j == 1)) //10秒更新一次内存情况
                    {
                        _operatingSystemInfo = null!;
                        _operatingSystemInfo = OperatingSystemInfo.GetOperatingSystemInfo();
                        _operatingSystemType = _operatingSystemInfo.OperatingSystemType;
                        _globalSystemInfo.MemoryInfo = getMeminfo();
                    }

                    if (i % 120 == 0 || i == 1) //2分钟更新一次硬盘情况
                    {
                        _globalSystemInfo.DriveInfo = DiskInfoValue.GetDrivesInfo();
                    }

                    switch (_operatingSystemType)
                    {
                        case OperatingSystemType.Windows:
                            CPUWinLoadValue.Refresh();
                            _globalSystemInfo.CpuLoad = CPUWinLoadValue.CPULOAD;
                            _globalSystemInfo.NetWorkStat = NetWorkWinValue3.GetNetworkStat();
                            break;
                        case OperatingSystemType.MacOSX:
                            _globalSystemInfo.CpuLoad = CPUMacOSLoadValue.CPULOAD;
                            _globalSystemInfo.NetWorkStat = NetWorkMacValue.GetNetworkStat();
                            break;
                        case OperatingSystemType.Linux:
                            _globalSystemInfo.CpuLoad = CPULinuxLoadValue.CPULOAD;
                            _globalSystemInfo.NetWorkStat = NetWorkLinuxValue.GetNetworkStat();
                            break;
                    }

                    _globalSystemInfo.UpdateTime = DateTime.Now;
                }

                Thread.Sleep(1000);
            }
        }


        public string GetSystemInfoJson()
        {
            lock (_lockObj)
            {
                return JsonSerializer.Serialize(_globalSystemInfo, DefaultTextJsonConfig.DefaultOptions);
            }
        }

        public PerformanceInfo GetSystemInfoObject()
        {
            lock (_lockObj)
            {
                return _globalSystemInfo;
            }
        }
    }
}