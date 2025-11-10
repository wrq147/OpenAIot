using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class Out_ChannelInfo
    {
        /// <summary>
        /// 通道名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 通道代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public Out_ChannelInfo(string name, string code, string img, string remark)
        {
            this.Name = name;
            this.Code = code;
            this.ImageUrl = img;
            this.Remark = remark;
        }
    }
}
