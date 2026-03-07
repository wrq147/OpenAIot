using System;
using System.Collections.Generic;

namespace ChannelUtility.Tsl
{
    public class BaseInputValue
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
        /// 数据类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object defval { get; set; }
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool readOnly { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// url,base64
        /// </summary>
        public string bodyType { get; set; }
        /// <summary>
        /// 枚举类型使用的枚举元素
        /// </summary>
        public Dictionary<string, string> elements { get; set; }
        /// <summary>
        /// 判断枚举是否多选
        /// </summary>
        public bool multi { get; set; }
    }
}
