using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class ChannelInfo
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
        /// 通道显示图片
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
        /// <summary>
        /// 通道备注
        /// </summary>
        public string Remark { get; set; } = string.Empty;
        /// <summary>
        /// 是否可以自定义脚本解释
        /// </summary>
        public bool CanScript { get; set; } = false;
        /// <summary>
        /// 是否启用Modbus规则匹配
        /// </summary>
        public bool CanModbus { get; set; } = false;
        /// <summary>
        /// 发布时是否发送绑定数据包
        /// </summary>
        public bool CanBind { get; set; } = false;
        /// <summary>
        /// 是否启用在线调试
        /// </summary>
        public bool CanDebug { get; set; } = false;
        /// <summary>
        /// 报文间隔
        /// </summary>
        public int SendInterval { get; set; } = 300;
    }

    public class ChangeChannelReq
    {
        public string code { get; set; }
        public ChannelInfo data { get; set; }
    }
}
