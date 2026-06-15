using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Model
{
    [TableName("llm_article")]
    public class Out_ArticleItem
    {
        [ID]
        public string Id { get; set; }

        /// <summary>
        /// 知识库ID
        /// </summary>
        public string KbId { get; set; }
        public string ColumnId { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
    }
}
