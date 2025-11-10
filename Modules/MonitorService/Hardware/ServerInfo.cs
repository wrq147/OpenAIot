using MonitorService.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonitorService.Hardware
{
    public class ServerInfo
    {

        /// <summary>
        /// CPU相关信息
        /// </summary>
        public Cpu cpu { get; set; }

        /// <summary>
        /// 內存相关信息
        /// </summary>
        public Mem mem { get; set; }

        /// <summary>
        /// .net clr
        /// </summary>
        public CLR clr { get; set; }


        /// <summary>
        /// 服务器相关信息
        /// </summary>
        public Sys sys { get; set; }

        /// <summary>
        /// 磁盘相关信息
        /// </summary>
        public List<SysFile> sysFiles { get; set; }
        public ServerInfo()
        {
            cpu = new Cpu();
            mem = new Mem();
            clr = new CLR();
            sys = new Sys();
            sysFiles = new List<SysFile>();

            var disks = ComputerUtil.GetDiskInfos();
            foreach (var disk in disks)
            {
                var sysFile = new SysFile();
                sysFile.dirName = disk.DiskName;
                sysFile.typeName = disk.TypeName;
                sysFile.free = disk.TotalFree + "GB";
                sysFile.total = disk.TotalSize + "GB";
                sysFile.used = disk.Used + "GB";
                sysFile.usage = Math.Round(disk.Used / (double)disk.TotalSize * 100.0, 2);
                sysFiles.Add(sysFile);
            }
        }
    }
}
