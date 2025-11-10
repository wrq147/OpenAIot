using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    /// <summary>
    /// 物联属性标识符分组
    /// </summary>
    [TableName("mz_iot_code_group")]
    public class MZ_IotCodeGroup
    {
        [ID(false)]
        public int? Id { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 子分组
        /// </summary>
        [DataIgnore]
        public List<MZ_IotCodeGroup> Children { get; set; }
        /// <summary>
        /// 标识符列表
        /// </summary>
        [DataIgnore]
        public List<MZ_IotCode> CodeList { get; set; }
    }
}
