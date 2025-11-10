using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Model
{
    [TableName("mz_push_client")]
    public class MZ_PushClient
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [ID(false)]
        public long? UserId { get; set; }
        /// <summary>
        /// 用户最新使用的客户端的CID
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}
