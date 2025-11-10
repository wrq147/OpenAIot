using MonitorService.Util;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MonitorService.Hardware
{
    /// <summary>
    /// 內存相关信息
    /// </summary>
    public class Mem
    {

        /// <summary>
        /// 内存总量  单位GB
        /// </summary>
        public string total { get; set; }


        /// <summary>
        /// 已用内存  单位GB
        /// </summary>
        public string used { get; set; }


        /// <summary>
        /// 剩余内存  单位GB
        /// </summary>
        public string free { get; set; }
        /// <summary>
        /// 使用率
        /// </summary>
        public string usage { get; set; }
        public void Refresh()
        {
            var memoryMetrics = ComputerUtil.GetComputerInfo();
            total = memoryMetrics.TotalRam;
            free = memoryMetrics.FreeRam;
            used = memoryMetrics.UsedRam;
            usage = memoryMetrics.RamRate;
        }
    }
}
