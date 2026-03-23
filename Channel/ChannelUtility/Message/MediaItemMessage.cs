using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaItemMessage : BaseDeviceMessage
    {
        public MediaItemMessage()
        {
            MsgType = "UpVItem";
        }
        public VideoCaptureItem Item { get; set; }
        public AIConfig Config { get; set; }
    }
    public class VideoCaptureItem
    {
        /// <summary>
        /// 视频Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 拉流地址或摄像头Ip
        /// </summary>
        public string PullAddr { get; set; }
        /// <summary>
        /// 推流的流Id
        /// </summary>
        public string PushKey { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
    public class AIConfig
    {
        public float MotionRatio { get; set; }
        public int CoolDownMs { get; set; }
        public long OrgId { get; set; }
        public List<AIDetectItem> Tasks { get; set; }
    }
    public class AIDetectItem
    {
        /// <summary>
        /// 检测类型
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 是否启用绘制
        /// </summary>
        public bool EnableDraw { get; set; }
        /// <summary>
        /// 检测参数
        /// </summary>
        public Dictionary<string, object> paramValues { get; set; }
    }
}
