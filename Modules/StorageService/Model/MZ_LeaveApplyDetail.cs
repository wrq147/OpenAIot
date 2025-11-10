using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    [TableName("mz_leave_apply_detail")]
    public class MZ_LeaveApplyDetail
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 申请单编码
        /// </summary>
        public string ApplyId { get; set; }
        /// <summary>
        /// 所在仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 存储类型：0半成品、1成品
        /// </summary>
        public int? TargetType { get; set; }
        /// <summary>
        /// 产品批次Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 目标编号
        /// </summary>
        [DataIgnore]
        public string TargetNumber { get; set; }
        /// <summary>
        /// 目标名称
        /// </summary>
        [DataIgnore]
        public string TargetName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        [DataIgnore]
        public string PhotoUrl { get; set; }
    }
}
