using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    /// <summary>
    /// 告警通知方式
    /// </summary>
    public class WarningNoticeItem
    {
        /// <summary>
        /// 通知方式:APP站内通知,EMAIL邮件通知,SMS短信通知,WX微信通知
        /// </summary>
        public string NoticeWay { get; set; }
        /// <summary>
        /// 目标类型：0为设备来源的用户，1为设备来源的角色，2为设备拥有者，3为设备使用者（可能是组织的管理员或设备使用者）,4为固定目标
        /// </summary>
        public byte TargetType { get; set; }
        /// <summary>
        /// TargetType为4时传邮件通知时传固定邮箱，短信通知时传固定手机号，0时传用户Id,1时传角色Id，其它传空
        /// </summary>
        public string TargetValue { get; set; }
    }
}
