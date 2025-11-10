using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    public class Out_FlowDevice
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 所属产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备通讯编码
        /// </summary>
        public string DeviceId { get; set; }
        public string NetworkWay { get; set; }
        public string ModelTSL { get; set; }
    }
}
