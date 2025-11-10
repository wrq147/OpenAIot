using Microsoft.Win32;
using MonitorService.Util;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;


namespace MonitorService.Hardware
{
    public class Cpu
    {

        /// <summary>
        /// 核心数
        /// </summary>
        public int CpuNum { get; set; }


        /// <summary>
        /// CPU使用率
        /// </summary>
        public string CpuUse { get; set; }
        /// <summary>
        /// CPU线程数
        /// </summary>
        public int ThreadCount { get; set; }
        /// <summary>
        /// cpu架构
        /// </summary>
        public string CpuBit { get; set; }

        public void Refresh()
        {
            this.CpuNum = Environment.ProcessorCount;
            Process[] processes = Process.GetProcesses();
            ThreadCount = 0;
            foreach (var info in processes)
            {
                ThreadCount += info.Threads.Count;
            }
            this.CpuUse = ComputerUtil.GetCPURate();
            this.CpuBit = Environment.Is64BitProcess ? "x64" : "x86";

        }
    }
}
