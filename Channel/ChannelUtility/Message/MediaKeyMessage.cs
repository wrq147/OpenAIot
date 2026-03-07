using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaKeyMessage : BaseDeviceMessage
    {
        public MediaKeyMessage()
        {
            MsgType = "MediaKey";
        }
        /// <summary>
        /// ZLMediaKit的视频Key
        /// </summary>
        public string VideoKey { get; set; }
        /// <summary>
        /// 记录的日期
        /// </summary>
        public DateTime? KeyDate { get; set; }
        /// <summary>
        /// 关键帧事件描述
        /// </summary>
        public string EvtDes { get; set; }
        /// <summary>
        /// 关键帧图片路径
        /// </summary>
        public string FilePath { get; set; }
    }
}
