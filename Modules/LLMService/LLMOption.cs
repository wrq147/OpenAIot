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
        /// <summary>
        /// LLM的数据库查询连接字符串
        /// </summary>
        public string DBConnectionString { get; set; }
        /// <summary>
        /// SearXNG的API地址，例子：http://127.0.0.1:8061
        /// </summary>
        public string SearXNGUrl { get; set; }
    }
    public class ModelOption
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
    }

}
