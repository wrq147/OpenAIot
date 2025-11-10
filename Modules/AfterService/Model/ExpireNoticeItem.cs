using System;
using System.Collections.Generic;
namespace AfterService.Model
{
    public class ExpireNoticeItem
    {
        public int day { get; set; }
        public int way { get; set; }
        /// <summary>
        /// 0为单独提醒、1为合并提醒
        /// </summary>
        public int ccway { get; set; }
        public List<NoticeUserItem> target { get; set; }
    }
    public class NoticeUserItem
    {
        public long userid { get; set; }
        public string img { get; set; }
        public string name { get; set; }
    }
}
