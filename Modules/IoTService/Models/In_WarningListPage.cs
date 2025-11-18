using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_WarningListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤设备Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 过滤设备的批次编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 过滤协议Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 过滤报警名称、设备名称、协议名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 过滤事件标识
        /// </summary>
        public string WarnCode { get; set; }
        /// <summary>
        /// 过滤状态
        /// </summary>
        public byte? Status { get; set; }
    }
}
