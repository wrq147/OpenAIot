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
        /// 摄像头类型:0为固定地址,1为GB28181
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
        /// 码流类型：0为主码流，1为子码流
        /// </summary>
        public byte? BitType { get; set; }
        /// <summary>
        /// GB28181服务的公网主机
        /// </summary>
        public string GBPublicAddr { get; set; }
        /// <summary>
        /// GB28181服务的公网主机端口
        /// </summary>
        public int? GBPublicPort { get; set; }
        /// <summary>
        /// AI检测任务
        /// </summary>
        public string AITasks { get; set; }
        /// <summary>
        /// 当前拉流的节点名称，无为空
        /// </summary>
        public string PullNode { get; set; }
        /// <summary>
        /// 服务器节点Id
        /// </summary>
        public string NodeId { get; set; }
        /// <summary>
        /// 视频地址
        /// </summary>
        [DataIgnore]
        public string VideoUrl { get; set; }
    }
}
