using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 表单权限项
    /// </summary>
    public class FormPermsItem
    {
        public string id { get; set; }
        public string title { get; set; }
        public bool required { get; set; }
        /// <summary>
        /// R只读.E可编辑,H隐藏
        /// </summary>
        public string perm { get; set; }
    }
}
