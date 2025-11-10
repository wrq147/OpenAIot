using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Model
{
    /// <summary>
    /// 用户站内消息处理表
    /// </summary>
    [TableName("mz_message_log")]
    public class MZ_Message_Log
    {
        [ID(true)]
        public long? id { get; set; }
        /// <summary>
        /// 接收者id
        /// </summary>
        public long? receiver_id { get; set; }
        /// <summary>
        /// 消息id
        /// </summary>
        public long? messsage_id { get; set; }
        /// <summary>
        /// 0 未读，1 已读，2 删除
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 读取时间
        /// </summary>
        public DateTime? read_time { get; set; }
    }
}
