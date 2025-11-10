using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    /// <summary>
    /// 流程表单数据
    /// </summary>
    [TableName("mz_flow_value")]
    public class MZ_FormDataItem
    {
        public MZ_FormDataItem() { }
        [ID(false)]
        public long FlowId { get; set; }
        /// <summary>
        /// 表单项唯一标识
        /// </summary>
        [ID(false)]
        public string FieldId { get; set; }
        /// <summary>
        /// json类型（该表单项的值）
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 保存长数据
        /// </summary>
        public string LongValue { get; set; }
        /// <summary>
        /// 数值类型（该表单项的值）
        /// </summary>
        public double? NumberValue { get; set; }

    }
}
