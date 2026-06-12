using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Model
{
    public class In_ArticleQuery : BaseQueryParam
    {
        public string? KbId { get; set; }
        public string Key { get; set; }
        public bool? IsPublic { get; set; }
    }
}
