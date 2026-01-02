
namespace AuthService.Fields
{
    public class FieldBase
    {
        /// <summary>
        /// 映射的字段：Ext1~100
        /// </summary>
        public string mapid { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 字段类型：文本、数字、时间、附件、图片、超链接、单选框、复选框、关联对象
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 隐藏条件
        /// </summary>
        public ConditionField[] conditions { get; set; }
        /// <summary>
        /// 隐藏条件的组合方式
        /// </summary>
        public string[] groups { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool is_required { get; set; }
    }
    public class ConditionField
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 比较字符
        /// </summary>
        public string compare { get; set; }
        /// <summary>
        /// 值类型
        /// </summary>
        public string valtype { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object val { get; set; }
    }
}
