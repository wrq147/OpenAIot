using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    /// <summary>
    /// 关联对象字段
    /// </summary>
    public class ObjectField : FieldBase
    {
        /// <summary>
        /// 对象类型：用户、部门
        /// </summary>
        public string object_type { get; set; }
        /// <summary>
        /// 数据填充规则
        /// </summary>
        public Fill_Item[] items { get; set; }
    }
    public class Fill_Item
    {
        /// <summary>
        /// 对象的字段，填充的数据源
        /// </summary>
        public string source_obj { get; set; }
        /// <summary>
        /// 要填充的字段
        /// </summary>
        public string field { get; set; }
    }
}
