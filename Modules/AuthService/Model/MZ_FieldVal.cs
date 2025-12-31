using MyAccess.DB.Attr;
using System;
namespace AuthService.Model
{
    [TableName("mz_field_val")]
    public class MZ_FieldVal
    {
        /// <summary>
        /// 原表Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 字段Id
        /// </summary>
        [ID(false)]
        public string FieldId { get; set; }
        /// <summary>
        /// 原表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 保存长文本数据
        /// </summary>
        public string LongValue { get; set; }
        /// <summary>
        /// 文本值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 数值
        /// </summary>
        public double? NumberValue { get; set; }
    }
}
