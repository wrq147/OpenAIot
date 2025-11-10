using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    /// <summary>
    /// 字段权限项
    /// </summary>
    public class FieldPermsItem
    {
        /// <summary>
        /// 自定义字段Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// R只读.E可编辑,H隐藏
        /// </summary>
        public string perm { get; set; }
    }
}
