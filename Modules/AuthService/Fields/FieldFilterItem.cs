using Minio.DataModel;
using MyAccess.DB;
using MyAccess.DB.Builder;
using System;

namespace AuthService.Fields
{
    public class FieldFilterItem
    {
        /// <summary>
        /// 过滤字段
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 比较符号：大于、小于、大于等于、小于等于、不等于、等于、包含、不包含、关联
        /// </summary>
        public string compare { get; set; } 

        /// <summary>
        /// 比较的值（字符串）
        /// </summary>
        public string val { get; set; }
        /// <summary>
        /// 比较的值（数字）
        /// </summary>
        public double? val_num { get; set; }
        /// <summary>
        /// 比较的值（字符串数组）
        /// </summary>
        public string[] val_arr { get; set; }
    }
}
