using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Model
{
    public class T_TableField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColumnName{ get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 所属表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 字段备注
        /// </summary>
        public string ColumnComment { get; set; }
    }
}
