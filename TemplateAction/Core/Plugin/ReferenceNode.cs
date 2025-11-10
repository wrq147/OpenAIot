using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateAction.Core
{
    public class ReferenceNode
    {
        public string Name { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<ReferenceNode> Children { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public List<ReferenceNode> Parents { get; set; }
        /// <summary>
        /// 离末端的最远距离
        /// </summary>
        public int Level { get; set; }
    }
}
