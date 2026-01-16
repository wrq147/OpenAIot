using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    /// <summary>
    /// 物联协议的属性规则
    /// </summary>
    [TableName("mz_iot_win_rule")]
    public class MZ_IotWinRule
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属协议Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 关联属性
        /// </summary>
        public string PropCode { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 统计时间：0每时、1每日、2每月
        /// </summary>
        public byte? WindowWay { get; set; }
        /// <summary>
        /// 统计方式：最大值:max，最小值：min，平均值：mean，合计值：sum，期初值：first，期末值：last，区间值：range，计数：count
        /// </summary>
        public string MergeWay { get; set; }
        /// <summary>
        /// 统计属性
        /// </summary>
        public string MergeCode { get; set; }
        /// <summary>
        /// 优先级：值越小越先统计
        /// </summary>
        public int? Priority { get; set; }
    }
}
