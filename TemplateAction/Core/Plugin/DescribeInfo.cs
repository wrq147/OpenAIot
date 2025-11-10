using System;


namespace TemplateAction.Core
{
    /// <summary>
    /// 描述信息
    /// </summary>
    public class DescribeInfo
    {
        public string Name { get; set; }
        /// <summary>
        /// 接口路径
        /// </summary>
        public string Code { get; set; }
        public int Sort { get; set; }
        public string ParentCode { get; set; }
        /// <summary>
        /// 关联代码
        /// </summary>
        public string AboutCode { get; set; }
    }
}
