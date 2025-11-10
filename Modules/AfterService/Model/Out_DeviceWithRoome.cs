

using MyAccess.DB.Attr;
using System.Collections.Generic;

namespace AfterService.Model
{
    public class Out_DeviceWithRoome
    {
        public string RoomId { get; set; }
        /// <summary>
        /// 房间名称
        /// </summary>
        public string RoomName { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 分类Id
        /// </summary>
        public string CategoryId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public long LeaderId { get; set; }
        /// <summary>
        /// 协作者
        /// </summary>
        public string Helper { get; set; }
    }
}
