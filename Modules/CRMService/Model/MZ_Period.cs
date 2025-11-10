using Common.Attr;
using FluentMigrator.Infrastructure;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    /// <summary>
    /// 销售阶段表
    /// </summary>
    [TableName("mz_period")]
    public class MZ_Period
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 阶段名称（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入阶段名称")]
        public string PeriodName { get; set; }
        /// <summary>
        /// 阶段类型：进行中ing,赢单win,输单lose,无效invalid（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入阶段类型")]
        public string PeriodType { get; set; }
        /// <summary>
        /// 赢率，单位%（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入赢率")]
        public float? Probability { get; set; }
        /// <summary>
        /// 排序（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入排序")]
        public int? Sort { get; set; }
    }
}
