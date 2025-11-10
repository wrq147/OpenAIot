using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorService.Model
{
    /// <summary>
    /// 节日类型表(每日根据类型定时生成节日)
    /// </summary>
    [TableName("mz_holiday_type")]
    public class MZ_HolidayType
    {
        /// <summary>
        /// 节日类型ID
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 节日类型名称
        /// </summary>
        public string HolidayName { get; set; }
        /// <summary>
        /// 日历类型：0为公历、1为农历
        /// </summary>
        public int? CalendarType { get; set; }
        /// <summary>
        /// 开始放假
        /// </summary>
        public DateTime? HolidayStart { get; set; }
        /// <summary>
        /// 结束放假
        /// </summary>
        public DateTime? HolidayEnd { get; set; }
        /// <summary>
        /// 下次生成节日时间
        /// </summary>
        public DateTime? NextTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
    }
}
