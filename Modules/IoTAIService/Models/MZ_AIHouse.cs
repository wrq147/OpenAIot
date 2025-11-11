using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Models
{
    /// <summary>
    /// 人员建模库表
    /// </summary>
    [TableName("mz_ai_house")]
    public class MZ_AIHouse
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 建模库名称
        /// </summary>
        public string HouseName { get; set; }
        /// <summary>
        /// 建模库备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态（0禁用、1启用）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 人脸数量
        /// </summary>
        [DataIgnore]
        public int FaceCount { get; set; }
    }
}
