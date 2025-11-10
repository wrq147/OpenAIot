using System;
namespace IoTService.Models
{
    public class In_IotConfig
    {
        /// <summary>
        /// 是否自动增加物联卡
        /// </summary>
        public bool? EnableAutoAdd { get; set; }
        /// <summary>
        /// 为空不开启，设备上线自动添加卡来源，YiDong、SimBoss
        /// </summary>
        public string AutoICCIDFrom { get; set; }
        /// <summary>
        /// 移动物联卡接口配置
        /// </summary>
        public string YiDongOption { get; set; }
        /// <summary>
        /// SimBoss物联卡接口配置
        /// </summary>
        public string SimBossOption { get; set; }
        /// <summary>
        /// SohanOption物联卡接口配置
        /// </summary>
        public string SohanOption { get; set; }
        /// <summary>
        /// 联通物联卡接口配置
        /// </summary>
        public string UnicomOption { get; set; }
    }
}
