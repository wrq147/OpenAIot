using System;

namespace AirJointUI.Models
{
    public class AlarmItem
    {
        /// <summary>
        /// Id编号
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 工单唯一编号
        /// </summary>
        public string WarnNumber { get; set; }
        /// <summary>
        /// 告警名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 事件标识
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 告警描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 报警级别：0普通、1告警、2紧急
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string MsgInfo { get; set; }
        /// <summary>
        /// 状态：0待处理，1已处理
        /// </summary>
        public byte? Status { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public string ClearOn { get; set; }
        /// <summary>
        /// 处理人员
        /// </summary>
        public long? ClearId { get; set; }
        /// <summary>
        /// 处理备注
        /// </summary>
        public string ClearRemark { get; set; }
        /// <summary>
        /// 告警时间
        /// </summary>
        public string CreateOn { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 设备图片
        /// </summary>
        public string DevicePhotoUrl { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
    }
}
