using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Model
{
    public class In_KbColumnQuery : BaseQueryParam
    {
        public string? KbId { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }
        public string? ParentId { get; set; }
    }
}
