using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class In_KFDevListPage : BaseQueryParam
    {
        /// <summary>
        /// 0为离线，1为在线，2为未知
        /// </summary>
        public byte? Online { get; set; }
        /// <summary>
        /// 过滤产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 过滤多个产品Id
        /// </summary>
        public string[] ProductList { get; set; }
        /// <summary>
        /// 通讯Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 是否显示告警
        /// </summary>
        public bool? ShowWarn { get; set; }
        /// <summary>
        /// 通过关键词搜索设备名称、编号、通讯Id
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 关键词搜索
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 过滤目标企业Id
        /// </summary>
        public long? TargetOrgId { get; set; }
        /// <summary>
        /// 过滤房间Id
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// 过滤房间分类
        /// </summary>
        public string RoomCategory { get; set; }
        /// <summary>
        /// 是否过滤已分配房间的
        /// </summary>
        public bool? FilterRoom { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        public string DState { get; set; }
        /// <summary>
        /// 是否显示标签
        /// </summary>
        public bool? ShowTags { get; set; }
    }
}
