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
        /// 视频源位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 摄像头类型:0为固定地址,1为GB28181设备，2为GB28181通道，3为Onvif设备
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
        /// 注册用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 注册密码
        /// </summary>
        public string UserPwd { get; set; }
        /// <summary>
        /// AI检测任务
        /// </summary>
        public string AITasks { get; set; }
        /// <summary>
        /// 服务器节点Id
        /// </summary>
        public string NodeId { get; set; }
    }
}
