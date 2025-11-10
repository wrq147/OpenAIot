using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    /// <summary>
    /// 计划的设备事件
    /// </summary>
    [TableName("mz_plane_event")]
    public class MZ_PlaneEvent
    {
        [ID(true)]
        public long? Id { get; set; }
        /// <summary>
        /// 计划类型编码
        /// </summary>
        public string PlaneId { get; set; }
        /// <summary>
        /// 事件所属组织
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 设备事件格式:ProductId$事件标识符
        /// </summary>
        public string EventId { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }
    }
}
