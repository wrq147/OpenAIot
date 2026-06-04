using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService
{
    public class LLMOption
    {
        public Dictionary<string, ModelOption> Models { get; set; } = new();
        public string DefaultChatModel { get; set; } = "DeepSeek";
    }
    public class ModelOption
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
    }
}
