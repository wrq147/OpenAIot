using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    /// <summary>
    /// 录像播放文件的关键帧
    /// </summary>
    [TableName("mz_iot_record_key")]
    public class MZ_IotRecordKey
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
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
        [JsonConverter(typeof(ImageUrl))]
        public string FilePath { get; set; }
    }
}
