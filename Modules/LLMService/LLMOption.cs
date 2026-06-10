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
        /// 助手可查询表元数据
        /// </summary>
        public List<SelectTable> TableSchema { get; set; } = new();
    }
    public class ModelOption
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
    }
    public class SelectTable
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表描述
        /// </summary>
        public string Description { get; set; }
    }
}
