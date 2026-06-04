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
        /// <summary>
        /// Milvus的连接地址
        /// </summary>
        public string MilvusUrl { get; set; }
        /// <summary>
        /// Milvus数据库名称
        /// </summary>
        public string MilvusDatabase { get; set; }
    }
    public class ModelOption
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
    }
}
