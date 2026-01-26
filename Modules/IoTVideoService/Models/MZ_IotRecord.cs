using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    /// <summary>
    /// 录像计划
    /// </summary>
    [TableName("mz_iot_record")]
    public class MZ_IotRecord : BaseEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        public long? OrgId { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }
        /// <summary>
        /// 视频源Id
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// 视频源位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 录像保存周期（天），默认7天
        /// </summary>
        public int? SaveCycle { get; set; }
        /// <summary>
        /// 时段类型：week（按周）、time（按时段）
        /// </summary>
        public string RecordTimeType { get; set; }
        /// <summary>
        /// 录像时段描述（如：周一 08:00-18:00）
        /// </summary>
        public string RecordTimeDesc { get; set; }
        /// <summary>
        /// 按周配置（JSON格式）：[{"week":1,"Time":8,"Op":"Start"},...]
        /// </summary>
        public string WeekConfig { get; set; }
        /// <summary>
        /// 按时段配置（JSON格式）：[{"Time":8,"Op":"Start"},{"Time":18,"Op":"End"},...]
        /// </summary>
        public string TimeConfig { get; set; }
        /// <summary>
        /// 状态：0-禁用，1-启用
        /// </summary>
        public int? Status { get; set; }
    }
}
