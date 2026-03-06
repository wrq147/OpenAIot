using System;
using System.Collections.Generic;
using MessagePack;
namespace ChannelUtility.Message
{
    [MessagePackObject]
    public class AIDetectRequestMeesage
    {
        /// <summary>
        /// 设备dtuId
        /// </summary>
        [Key(0)]
        public string DeviceId { get; set; }

        [Key(1)]
        public string VideoKey { get; set; }

        [Key(2)]
        public string NodeId { get; set; }

        /// <summary>
        /// 运动区块占比
        /// </summary>
        [Key(3)]
        public float MRatio { get; set; }

        /// <summary>
        /// 图像帧（JPEG压缩后的字节数组，无额外开销）
        /// </summary>
        [Key(4)]
        public byte[] Frame { get; set; }

        [Key(5)]
        public int Width { get; set; }

        [Key(6)]
        public int Height { get; set; }

        /// <summary>
        /// AI配置JSON字符串（需解析为List<AIConfigData>）
        /// </summary>
        [Key(7)]
        public string Configs { get; set; }
    }

    public class AIConfigData
    {
        public string DetType { get; set; }
        public long OrgId { get; set; }
        public Dictionary<string, object> DetParams { get; set; }
    }
}
