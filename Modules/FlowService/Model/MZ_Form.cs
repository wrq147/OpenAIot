using MyAccess.DB.Attr;
using System;

namespace FlowService.Model
{
    /// <summary>
    /// 流程对应的表单类型
    /// </summary>
    [TableName("mz_form")]
    public class MZ_Form
    {
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// 表单的字段，json格式存储
        /// </summary>
        public string FormFields { get; set; }
    }
}
