using AuthService;
using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Models
{
    /// <summary>
    /// 人员建模信息表
    /// </summary>
    [TableName("mz_ai_mem")]
    public class MZ_AIMem
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 人员Id
        /// </summary>
        public long? MemId { get; set; }
        /// <summary>
        /// 建模状态：0未建模，1为建模成功，2为建模失败
        /// </summary>
        public byte? FStatus { get; set; }
        /// <summary>
        /// 人脸建模头像
        /// </summary>
        [JsonConverter(typeof(ImageUrl), true)]
        public string FaceImg { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 人员信息
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo MemInfo { get; set; }
        /// <summary>
        /// 建模库信息
        /// </summary>
        [DataIgnore]
        public MZ_AIHouse HouseInfo { get; set; }
    }
}
