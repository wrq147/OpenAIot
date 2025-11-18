using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_DeviceListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤物联协议分类
        /// </summary>
        public string ClassId { get; set; }
        /// <summary>
        /// 0为离线，1为在线，2为未知
        /// </summary>
        public byte? Online { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        public string DState { get; set; }
        /// <summary>
        /// 过滤协议Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 过滤多个协议Id
        /// </summary>
        public string[] ProductList { get; set; }
        /// <summary>
        /// 过滤多个设备Id
        /// </summary>
        public string[] Ids { get; set; }
        /// <summary>
        /// 过滤多个设备的通讯Id
        /// </summary>
        public string[] DtuIds { get; set; }
        /// <summary>
        /// 过滤多个批次编号
        /// </summary>
        public string[] Numbers { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备唯一编码（批次编号）
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 过滤是否绑定通讯Id
        /// </summary>
        public bool? HasDeviceId { get; set; }
        /// <summary>
        /// 同时搜索设备名称、设备编码、批次编号
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 关键词搜索
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 标签条件
        /// </summary>
        public List<TagCondition> TagConditions { get; set; }
        /// <summary>
        /// 是否显示标签
        /// </summary>
        public bool? ShowTags { get; set; }
    }

    public class TagCondition
    {
        /// <summary>
        /// 标签标识
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 标签类型
        /// </summary>
        public string optionType { get; set; }
        /// <summary>
        /// 比较方式：ne不等于、eq等于、gt大于、lt小于、ge大于等于、le小于等于、like（字符串用）
        /// </summary>
        public string compare { get; set; }
        /// <summary>
        /// 比较值
        /// </summary>
        public object val { get; set; }
    }

}
