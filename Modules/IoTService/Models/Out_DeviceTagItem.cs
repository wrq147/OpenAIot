using ChannelUtility.Tsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    /// <summary>
    /// 设备标签信息
    /// </summary>
    public class Out_DeviceTagItem
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
        /// 绑定的属性标识
        /// </summary>
        public string MapCode { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 对应物模型选项
        /// </summary>
        public BaseValueOption Option { get; set; }
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
