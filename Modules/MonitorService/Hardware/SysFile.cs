using System;

namespace MonitorService.Hardware
{
    /// <summary>
    /// 系统文件相关信息
    /// </summary>
    public class SysFile
    {

        /// <summary>
        /// 盘符路径
        /// </summary>
        public string dirName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string typeName { get; set; }


        /// <summary>
        /// 总大小
        /// </summary>
        public string total { get; set; }


        /// <summary>
        /// 剩余大小
        /// </summary>
        public string free { get; set; }

        /// <summary>
        /// 已经使用量
        /// </summary>
        public string used { get; set; }


        /// <summary>
        /// 资源的使用率
        /// </summary>
        public double usage { get; set; }

    }
}
