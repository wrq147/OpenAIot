using System;

namespace IoTService
{
    /// <summary>
    /// 时序存储配置
    /// </summary>
    public class InfluxOption
    {
        /// <summary>
        /// 1表示存储，0表示不使用
        /// </summary>
        public string enable { get; set; }
        /// <summary>
        /// 连接的URL
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 连接令牌
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 存储的bucket名称,默认值MZIoT
        /// </summary>
        public string bucket { get; set; }
        /// <summary>
        /// 存储使用的组织
        /// </summary>
        public string org { get; set; }
    }
}
