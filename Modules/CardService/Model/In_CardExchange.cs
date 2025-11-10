using Common.Share;
using System;

namespace CardService.Model
{
    public class In_CardExchange : BaseQueryParam
    {
        /// <summary>
        /// 接收人
        /// </summary>
        public long? ReceiveUserId { get; set; }
    }
}
