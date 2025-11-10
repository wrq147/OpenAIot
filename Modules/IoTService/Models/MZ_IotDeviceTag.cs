using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    [TableName("mz_iot_device_tag")]
    public class MZ_IotDeviceTag
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 标签标识
        /// </summary>
        [ID(false)]
        public string Code { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 扩展信息值（字符串存储）
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 扩展信息值（数值存储）
        /// </summary>
        public double? NumValue { get; set; }
    }
}
