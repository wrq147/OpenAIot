using MyAccess.DB.Attr;
using System;


namespace AfterService.Model
{
    [TableName("mz_room_device")]
    public class MZ_RoomDevice
    {
        /// <summary>
        /// 房间编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        [ID(false)]
        public string TargetId { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 房间名称
        /// </summary>
        [DataIgnore]
        public string Name { get; set; }
    }
}
