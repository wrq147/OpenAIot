using Common.Share;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Milvus.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Business
{
    /// <summary>
    /// 长期记忆
    /// </summary>
    public class MemoryRagBLL
    {
        private IAiClientRegistry _registry;
        private readonly MilvusClient _client;
        public MemoryRagBLL(IOptions<LLMOption> option, IAiClientRegistry registry)
        {
            _registry = registry;
            _client = new MilvusClient(new Uri(option.Value.MilvusUrl), database: option.Value.MilvusDatabase);
        }
        public async Task<BusResponse<string>> CreateRagCollection()
        {
            // 检查集合是否已存在
            var exists = await _client.HasCollectionAsync("ChatMemory");
            if (exists)
            {
                return BusResponse<string>.Error(111, "AI记忆向量表已存在");
            }

            // 定义集合结构
            var schema = new CollectionSchema();
            schema.Fields.Add(FieldSchema.Create<long>("Id", isPrimaryKey: true, autoId: true));
            schema.Fields.Add(FieldSchema.CreateVarchar("SessionId", maxLength: 256));
            schema.Fields.Add(FieldSchema.CreateVarchar("Content", 8192));
            schema.Fields.Add(FieldSchema.CreateFloatVector("Embedding", dimension: 768));
            schema.Fields.Add(FieldSchema.Create<long>("CreateTime"));

            var collection = await _client.CreateCollectionAsync("ChatMemory", schema);
            // 创建索引以提高搜索性能
            await collection.CreateIndexAsync("Embedding", IndexType.Hnsw, SimilarityMetricType.Cosine);
            await collection.CreateIndexAsync(fieldName: "SessionId", indexType: IndexType.AutoIndex);
            await collection.CreateIndexAsync(fieldName: "CreateTime", indexType: IndexType.AutoIndex);

            return BusResponse<string>.Success();
        }

        public async Task SaveMemoryAsync(string sessionId, string userInput, string aiResp)
        {
            var content = $"用户输入: {userInput}\nAI回答: {aiResp}";
            var vec = await _registry.GetDefaultEmbed().GenerateVectorAsync(content);
            MilvusCollection collection = _client.GetCollection("ChatMemory");
            var curtime = MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now);
            List<ReadOnlyMemory<float>> embedVector = new();
            embedVector.Add(vec);
            await collection.InsertAsync(
                new FieldData[]
                {
                            FieldData.CreateVarChar("SessionId", new[] { sessionId }),
                            FieldData.CreateVarChar("Content",  new[] { content }),
                            FieldData.CreateFloatVector("Embedding", embedVector),
                            FieldData.Create<long>("CreateTime",new[] {curtime})
                });

            await collection.FlushAsync();
        }

        public async Task<string> SearchRelatedMemoriesAsync(string sessionId, string query, int limit = 3)
        {
            var vec = await _embeddingGenerator.GenerateEmbeddingAsync(query);
            var coll = _milvusClient.GetCollection(CollectionName);

            var searchResult = await coll.SearchAsync(
                "Embedding",
                new[] { vec.Vector.ToArray() },
                new SearchParameters
                {
                    Top = limit,
                    Filter = $"SessionId == '{sessionId.Replace("'", "\\'")}'",
                    MetricType = DistanceType.Cosine
                },
                outputFields: new[] { "Content", "SkillResult" });

            if (!searchResult.Any() || !searchResult[0].Any()) return "";

            var sb = new StringBuilder();
            sb.AppendLine("\n【长期记忆】");
            foreach (var hit in searchResult[0])
            {
                sb.AppendLine(hit.GetValue<string>("Content"));
                var skill = hit.GetValue<string>("SkillResult");
                if (!string.IsNullOrEmpty(skill))
                    sb.AppendLine($"[技能结果] {skill}");
            }
            return sb.ToString();
        }
    }
}
