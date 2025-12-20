using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    [TableName("mz_iot_video_source")]
    public class MZ_VideoSource
    {
        /// <summary>
        /// 视频源Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 摄像头类型:0为固定地址,1为GB/T28181
        /// </summary>
        public byte? VideoType { get; set; }
        /// <summary>
        /// ZLMediaKit的视频Key
        /// </summary>
        public string VideoKey { get; set; }
        /// <summary>
        /// 拉流地址
        /// </summary>
        public string PullAddr { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string UserPwd { get; set; }
        /// <summary>
        /// 码流类型：0为主码流，1为子码流
        /// </summary>
        public byte? BitType { get; set; }
        /// <summary>
        /// AI检测帧间隔,默认25帧
        /// </summary>
        public int? FrameInterval { get; set; }
        /// <summary>
        /// AI检测参数
        /// </summary>
        public string AIParams { get; set; }
        /// <summary>
        /// 当前拉流的节点名称，无为空
        /// </summary>
        public string PullNode { get; set; }
    }
}
