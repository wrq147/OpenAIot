using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class TagItem
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 标签标识
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 对应物模型选项
        /// </summary>
        public object Option { get; set; }
        /// <summary>
        /// 真值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 显示值
        /// </summary>
        public string DisplayValue { get; set; }
    }
}
