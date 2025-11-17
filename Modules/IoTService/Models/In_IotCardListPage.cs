using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_IotCardListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤来源
        /// </summary>
        public string CardFrom { get; set; }
        /// <summary>
        /// 过滤物联卡状态: 测试中:testing、库存:inventory、待激活:pending-activation、已激活:activation、已停卡:deactivation、已销卡:retired
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 按ICCID号、IMSI、MSISDN查询
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 按绑定设备的批次编号、通讯编码查询
        /// </summary>
        public string DeviceKey { get; set; }
        /// <summary>
        /// 过滤是否绑定设备
        /// </summary>
        public bool? IsBindDev { get; set; }
        /// <summary>
        /// 到期开始时间
        /// </summary>
        public DateTime? ExpirBeginTime { get; set; }
        /// <summary>
        /// 到期结束时间
        /// </summary>
        public DateTime? ExpirEndTime { get; set; }
    }
}
