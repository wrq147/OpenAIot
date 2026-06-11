using Common.Share;
using MyAccess.DB.Attr;
using System;

namespace LLMService.Model
{
    [TableName("llm_kb_column")]
    public class MZ_KbColumn : BaseEntity
    {
        [ID]
        public string Id { get; set; }
        /// <summary>
        /// 知识库ID
        /// </summary>
        public string KbId { get; set; }
        /// <summary>
        /// 知识库
        /// </summary>
        [DataIgnore]
        public MZ_Knowledge Kb { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父栏目ID，0表示顶级栏目
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int? SortOrder { get; set; }
        /// <summary>
        /// 状态：1-启用，0-禁用
        /// </summary>
        public int? Status { get; set; }
    }

}