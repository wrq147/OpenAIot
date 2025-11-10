using System;
using System.Collections.Generic;
namespace TemplateAction.Cache
{
    public class CacheItem
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 对应链表节点
        /// </summary>
        public LinkedListNode<string> Node { get; set; }
        public IDependency Dependency { get; set; }
    }
}
