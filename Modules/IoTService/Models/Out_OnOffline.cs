using System;

namespace IoTService.Models
{
    public class Out_OnOffline
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// true为在线，false为离线
        /// </summary>
        public bool IsOnline { get; set; }
    }
}
