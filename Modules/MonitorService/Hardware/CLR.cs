using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MonitorService.Hardware
{
    public class CLR
    {
        /// <summary>
        /// 运行框架
        /// </summary>
        public string FrameworkDescription { get; set; }

        /// <summary>
        /// 运行时长
        /// </summary>
        public string runTime { get; set; }
        /// <summary>
        /// 启动时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 程序占用内存
        /// </summary>
        public string UseMem { get; set; }
        public void Refresh()
        {
            FrameworkDescription = RuntimeInformation.FrameworkDescription;


            Process thisProcess = Process.GetCurrentProcess();  // 获取当前进程的 ProcessInfo 对象
            long b = thisProcess.PrivateMemorySize64;
            this.UseMem = b / 1024 / 1024 + "MB";

            TimeSpan span = DateTime.Now - thisProcess.StartTime;
            if (span.TotalDays > 1)
            {
                runTime = string.Format("{0}天{1}小时", (int)Math.Floor(span.TotalDays), span.Hours);
            }
            else if (span.TotalHours > 1)
            {
                runTime = string.Format("{0}小时", (int)Math.Floor(span.TotalHours));
            }
            else if (span.TotalMinutes > 1)
            {
                runTime = string.Format("{0}分钟", (int)Math.Floor(span.TotalMinutes));
            }
            else if (span.TotalSeconds >= 1)
            {
                runTime = string.Format("{0}秒", (int)Math.Floor(span.TotalSeconds));
            }
            else
            {
                runTime = "1秒";
            }
            startTime = thisProcess.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
