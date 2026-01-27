using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    /// <summary>
    /// 录像播放文件信息
    /// </summary>
    [TableName("mz_iot_record_file")]
    public class MZ_IotRecordFile
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 记录的日期
        /// </summary>
        public DateTime? FileDate { get; set; }
        /// <summary>
        /// 关联录像计划ID
        /// </summary>
        public string PlanId { get; set; }
        /// <summary>
        /// 视频源Id
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// ZLMediaKit的视频Key
        /// </summary>
        public string VideoKey { get; set; }
        /// <summary>
        /// 节点Id
        /// </summary>
        public string NodeId { get; set; }
        /// <summary>
        /// 0为文件存储，1为minio
        /// </summary>
        public byte? StorageWay { get; set; }
        /// <summary>
        /// 状态：0-结束录像，1-录像中
        /// </summary>
        public byte? Status { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
