using ChannelUtility.Config;
using System;

namespace ChannelUtility
{
    public class ChannelOption
    {
        /// <summary>
        /// rabbitmq连接字符串
        /// </summary>
        public string EventConn { get; set; }
        public string EventUser { get; set; }
        public string EventPass { get; set; }
        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public string RedisConn { get; set; }
        /// <summary>
        /// 通道物模型的扩展配置
        /// </summary>
        public ChannelConfig config { get; set; }
    }
}
