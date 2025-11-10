using ChannelUtility.Tsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class Out_DeviceFunc
    {
        /// <summary>
        /// 功能名称（必填项）
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 标识符（必填项）
        /// 支持大小写字母、数字和下划线
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool disabled { get; set; }
        /// <summary>
        /// 输入参数
        /// </summary>
        public List<BaseInputValue> inputs { get; set; }
    }
}
