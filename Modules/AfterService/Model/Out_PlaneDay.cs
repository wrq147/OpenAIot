using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class Out_PlaneDay
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 任务Id（逗号分隔）
        /// </summary>
        public string TaskIds { get; set; }
        /// <summary>
        /// 任务Id列表
        /// </summary>
        public string[] TaskIdList { get; set; }
        /// <summary>
        /// 任务列表
        /// </summary>
        public List<MZ_PlaneTask> TaskList { get; set; }
        /// <summary>
        /// 设备所属房间
        /// </summary>
        [DataIgnore]
        public List<string> RoomNames { get; set; }
    }
}
