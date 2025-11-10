using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_JVList
    {
        /// <summary>
        /// 设备的Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 物模型属性标识（不传则为全部属性）
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 分页号
        /// </summary>
        public int? pageNum { get; set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int? pageSize { get; set; }
    }
}
