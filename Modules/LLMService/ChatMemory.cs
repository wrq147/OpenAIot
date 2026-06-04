using System;
using Microsoft.Extensions.VectorData;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService
{
    /// <summary>
    /// 聊天长期记忆
    /// </summary>
    public class ChatMemory
    {
        [VectorStoreKey]
        public Guid Id { get; set; }

        /// <summary>
        /// 用户ID / 会话ID（用于隔离用户）
        /// </summary>
        [VectorStoreData]
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// 原始文本（用户问题 + AI回答 + 技能执行结果）
        /// </summary>
        [VectorStoreData]
        public string Content { get; set; } = string.Empty;


        /// <summary>
        /// 向量（根据你的 embedding 模型修改维度）
        /// </summary>
        [VectorStoreVector(dimensions: 768, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
        public ReadOnlyMemory<float> Embedding { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [VectorStoreData]
        public DateTime CreateTime { get; set; }
    }
}
