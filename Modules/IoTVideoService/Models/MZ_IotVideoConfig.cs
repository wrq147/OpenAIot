using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    /// <summary>
    /// 视频策略
    /// </summary>
    [TableName("mz_iot_video_config")]
    public class MZ_IotVideoConfig
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// AI检测任务
        /// </summary>
        public string AITasks { get; set; }
    }
}
