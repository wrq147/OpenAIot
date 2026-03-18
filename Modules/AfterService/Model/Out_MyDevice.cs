using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class Out_MyDevice
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 第三方编码
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 通讯Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备协议Id
        /// </summary>
        public string ProtocolId { get; set; }
        /// <summary>
        /// 设备所属产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 0为离线，1为在线，2为未初始化
        /// </summary>
        public byte? Online { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }
        /// <summary>
        /// 所在区域代码
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 设备所在房间
        /// </summary>
        public string RoomName { get; set; }
        /// <summary>
        /// 设备的功能定义
        /// </summary>
        public List<DevFun> Funs { get; set; }
        /// <summary>
        /// 设备的实时属性定义
        /// </summary>
        public List<DevProp> Props { get; set; }

        /// <summary>
        /// 是否为视频监控设备
        /// </summary>
        public bool? IsVideo
        {
            get
            {
                return this.DeviceId != null && this.DeviceId.StartsWith("VI_");
            }
        }
    }

    /// <summary>
    /// 设备功能描述
    /// </summary>
    public class DevFun
    {
        /// <summary>
        /// 功能名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 标识符
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 功能描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 功能输入参数
        /// </summary>
        public List<DevFunParam> inputs { get; set; }
    }
    public class DevFunParam
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 标识符
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 参数数据类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 参数备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 枚举元素
        /// </summary>
        public Dictionary<string, string> elements { get; set; }
    }

    public class DevProp
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 标识符
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 属性描述
        /// </summary>
        public string description { get; set; }
    }
}
