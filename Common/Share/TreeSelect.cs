using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Common.Share
{
    /// <summary>
    /// 选择树
    /// </summary>
    public class TreeSelect<T>
    {

        /// <summary>
        /// 节点ID
        /// </summary>
        public T id { get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        public T parentId { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TreeSelect<T>> children { get; set; }
    }
   
}
