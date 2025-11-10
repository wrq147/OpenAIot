using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class In_DevPlaneType : BaseQueryParam
    {
        /// <summary>
        /// 过滤发起类型
        /// </summary>
        public int? StartWay { get; set; }
        /// <summary>
        /// 过滤设备Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 只显示公开或拥有设备权限的类型
        /// </summary>
        public bool? CanStart { get; set; }
        /// <summary>
        /// 过滤流程模板Id
        /// </summary>
        public long? FlowTemplateId { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Key { get; set; }
    }
}
