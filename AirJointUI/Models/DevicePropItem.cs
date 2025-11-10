using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class DevicePropItem
    {
        /// <summary>
        /// 属性中文名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 属性代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 显示单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 属性值类型
        /// </summary>
        public string OptionType { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdatedOn { get; set; }
        /// <summary>
        /// 属性备注
        /// </summary>
        public string Description { get; set; }
    }
}
