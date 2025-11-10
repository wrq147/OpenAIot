using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Model
{
    /// <summary>
    /// 站内消息表
    /// </summary>
    [TableName("mz_message")]
    public class MZ_Message
    {
        [ID(false)]
        public long id { get; set; }
        /// <summary>
        /// 关联组织ID
        /// </summary>
        public long OrgId { get; set; }
        /// <summary>
        /// 发送者id
        /// </summary>
        public long sender_id { get; set; }
        /// <summary>
        /// 接收者id
        /// </summary>
        public long receiver_id { get; set; }
        /// <summary>
        /// 消息标签
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 消息点击类型
        /// </summary>
        public string click_type { get; set; }
        /// <summary>
        /// 消息点击跳转目标
        /// </summary>
        public string click_url { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataIgnore]
        public int? status { get; set; }
    }
}
