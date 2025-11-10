using System;

namespace IoTService.Models
{
    public class In_TagItem
    {
        /// <summary>
        /// 标签标识
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标签值
        /// </summary>
        public object Value { get; set; }
    }
}
