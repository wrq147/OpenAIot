using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Model
{
    public class In_KnowledgeQuery : BaseQueryParam
    {
        public string Name { get; set; }
        public int? Status { get; set; }
        /// <summary>
        /// 是否包含公开
        /// </summary>
        public bool? WithPublic { get; set; }
    }
}
