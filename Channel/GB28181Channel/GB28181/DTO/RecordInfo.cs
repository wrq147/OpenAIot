using GB28181Channel.GB28181.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.DTO
{
    /// <summary>
    /// 录像信息DTO
    /// </summary>
    public class RecordInfo
    {
        public string RecordId { get; set; }
        public string ChannelId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public RecordType RecordType { get; set; }
        public long SizeMB { get; set; }
        public string FilePath { get; set; }
    }
}
