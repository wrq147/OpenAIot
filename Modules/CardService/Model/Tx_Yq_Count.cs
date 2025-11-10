using System;

namespace CardService.Model
{
    /// <summary>
    /// 邀请短信计数
    /// </summary>
    public class Tx_Yq_Count
    {
        /// <summary>
        /// 开始计时的时间
        /// </summary>
        public DateTime first_send { get; set; }
        /// <summary>
        /// 1小时内
        /// </summary>
        public int count { get; set; }
    }
}
