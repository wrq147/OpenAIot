using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorService.Model
{
    /// <summary>
    /// 企业的节日表
    /// </summary>
    [TableName("mz_holiday_org")]
    public class MZ_HolidayOrg
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 节日日期
        /// </summary>
        public DateTime? Holiday { get; set; }
        /// <summary>
        /// 0表示整天，1表示起点，2表示结束
        /// </summary>
        public int? TimeWay { get; set; }
        /// <summary>
        /// 日期表示的字符串
        /// </summary>
        public string DayStr { get; set; }
        /// <summary>
        /// 关联的节日类型Id
        /// </summary>
        public string HolidayTypeId { get; set; }
        /// <summary>
        /// 节日类型名称
        /// </summary>
        [DataIgnore]
        [ColumnBy(typeof(MZ_HolidayType), "HolidayName")]
        public string HolidayTypeName { get; set; }
    }

}
