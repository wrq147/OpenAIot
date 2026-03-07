using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    /// <summary>
    /// 物联标识模板
    /// </summary>
    [TableName("mz_iot_code")]
    public class MZ_IotCode
    {
        [ID(false)]
        public int? Id { get; set; }
        /// <summary>
        /// 标识符名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标识符
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标识符分组
        /// </summary>
        public int? CodeGroup { get; set; }
        /// <summary>
        /// 默认选项内容
        /// </summary>
        public string OptionData { get; set; }
        /// <summary>
        /// 0为属性、1为功能、2为事件
        /// </summary>
        public int? CodeType { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary>
        public int? Sort { get; set; }
    }
}
