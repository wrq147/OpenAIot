using Common.Share;
using MyAccess.DB.Attr;
using System;

namespace LLMService.Model
{
    [TableName("llm_article")]
    public class MZ_Article : BaseEntity
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
        /// 栏目ID
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// 栏目
        /// </summary>
        [DataIgnore]
        public MZ_KbColumn KbColumn { get; set; }


        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 文章关键词
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// 封面URL
        /// </summary>
        public string Cover { get; set; }


        /// <summary>
        /// 阅读次数
        /// </summary>
        public int? ViewCount { get; set; }
    }

}