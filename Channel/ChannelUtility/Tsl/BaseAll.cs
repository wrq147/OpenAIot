using System;
using System.Collections.Generic;

namespace ChannelUtility.Tsl
{
    public class BaseAll
    {
        /// <summary>
        /// 名称（必填项）
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 标识符（必填项）
        /// 支持大小写字母、数字和下划线
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 是否为固定模板
        /// </summary>
        public bool isfixed { get; set; } = false;
    }
}
