using Microsoft.AspNetCore.Hosting;
using MonitorService.Util;
using System;
using System.Runtime.InteropServices;
using TemplateAction.Core;

namespace MonitorService.Hardware
{
    /// <summary>
    /// 系统相关信息
    /// </summary>
    public class Sys
    {

        /// <summary>
        /// 服务器名称
        /// </summary>
        public string computerName { get; set; }

        /// <summary>
        /// 项目路径
        /// </summary>
        public string userDir { get; set; }


        /// <summary>
        /// 操作系统
        /// </summary>
        public string osName { get; set; }


        /// <summary>
        /// 系统架构
        /// </summary>
        public string osArch { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>

        public string RemoteIp { get; set; }

        /// <summary>
        /// 系统运行时间
        /// </summary>
        public string SysRunTime { get; set; }


        public void Refresh(IWebHostEnvironment hostEnvironment)
        {
            computerName = Environment.MachineName;
            osName = RuntimeInformation.OSDescription;
            osArch = Environment.OSVersion.Platform.ToString() + " " + RuntimeInformation.OSArchitecture.ToString(); // 系统架构
            this.userDir = hostEnvironment.ContentRootPath;
            SysRunTime = ComputerUtil.GetRunTime(); // 系统运行时间
            RemoteIp = TAAction.Current?.Context.Request.ServerIP.MapToIPv4().ToString(); // 本地地址
        }
    }
}
