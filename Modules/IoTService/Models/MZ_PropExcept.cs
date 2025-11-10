using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace IoTService.Models
{
    [TableName("mz_prop_except")]
    public class MZ_PropExcept
    {
        [ID(true)]
        public long? Id { get; set; }
        /// <summary>
        /// DtuId
        /// </summary>
        public string DtuId { get; set; }
        /// <summary>
        /// 属性的标识符
        /// </summary>
        public string PropCode { get; set; }
        /// <summary>
        /// 异常类型：峰值spike、更改change
        /// </summary>
        public string ExceptType { get; set; }
        /// <summary>
        /// 异常值
        /// </summary>
        public string ExceptValue { get; set; }
        /// <summary>
        /// 发生时间
        /// </summary>
        public long? CreatedOn { get; set; }
    }
}
