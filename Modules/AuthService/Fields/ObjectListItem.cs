using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    /// <summary>
    /// 通用关联对象的列表项
    /// </summary>
    public class ObjectListItem
    {
        /// <summary>
        /// 列表显示名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值显示名称
        /// </summary>
        public string ValueName { get; set; }
        /// <summary>
        /// 关联对象的Id
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 关联对象
        /// </summary>
        public object Obj { get; set; }
    }
}
