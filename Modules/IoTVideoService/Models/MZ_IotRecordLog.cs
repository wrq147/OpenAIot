using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    /// <summary>
    /// 录像计划的执行日志
    /// </summary>
    [TableName("mz_iot_recordlog")]
    public class MZ_IotRecordLog
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 关联录像计划ID
        /// </summary>
        public string PlanId { get; set; }
        /// <summary>
        /// 视频源Id
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// 发生位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 日志类型：start（录像启动）、stop（录像停止）、fail（录像失败）、clean（文件清理）
        /// </summary>
        public string LogType { get; set; }
        /// <summary>
        /// 日志内容（如：录像失败原因、文件清理数量等）
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? ExecTime { get; set; }
    }
}
