using Common.Share;
using LLMService.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Milvus.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            try
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
            catch
            {
                Console.WriteLine("数据库Milvus未启用，无法初始化ChatMemory");
                return BusResponse<string>.Error(110, "张量数据库Milvus未启用");
            }
        
        }

        public async Task SaveMemoryAsync(string sessionId, string query, string content)
        {
            var vec = await _registry.GetDefaultEmbed().GenerateVectorAsync(query);
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

        public async Task<string> SearchRelatedMemoriesAsync(string sessionId, string query, float score = 0.6f)
        {
            var vec = await _registry.GetDefaultEmbed().GenerateVectorAsync(query);
            MilvusCollection collection = _client.GetCollection("ChatMemory");

            SearchParameters searchParameters = new();
            searchParameters.OutputFields.Add("Content");
            searchParameters.OutputFields.Add("CreateTime");
            searchParameters.Expression = "SessionId==\"" + sessionId + "\"";

            var results = await collection.SearchAsync(
                vectorFieldName: "Embedding",
                vectors: new ReadOnlyMemory<float>[] { vec },
                SimilarityMetricType.Cosine,
                limit: 20, searchParameters);


            bool hasmem = false;
            var sb = new StringBuilder();
            sb.AppendLine("【相关历史会话】");
            // 遍历每一条结果
            for (int i = 0; i < results.Scores.Count; i++)
            {
                float rsscore = results.Scores[i];
                if (rsscore > score)
                {
                    string tcontent = (results.FieldsData[0] as FieldData<string>).Data[i];
                    long ttime = (results.FieldsData[1] as FieldData<long>).Data[i];
                    DateTime dttime = MyAccess.Core.TypeConvert.Unix2Time(ttime);
                    sb.AppendLine(dttime.ToString("yyyy-MM-dd HH:mm:ss") + ";" + tcontent);
                    hasmem = true;
                }
            }

            if (hasmem)
            {
                return sb.ToString();
            }
            else
            {
                return "没有相关的会话记录";
            }
        }
        /// <summary>
        /// 查询指定日期的历史会话
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="day">日期格式：yyyy-MM-dd</param>
        /// <returns></returns>
        public async Task<string> GetHistoryByDate(string sessionId, string day)
        {
            if (!DateTime.TryParse(day, out DateTime searchDT))
            {
                return $"{day} 日期参数格式错误";
            }
            MilvusCollection collection = _client.GetCollection("ChatMemory");
            long startll = MyAccess.Core.TypeConvert.Time2Unix(searchDT);
            long endll = MyAccess.Core.TypeConvert.Time2Unix(searchDT.AddDays(1));
            string exp = "SessionId==\"" + sessionId + "\" AND CreateTime>=" + startll + " AND CreateTime<=" + endll;
            QueryParameters searchParameters = new();
            searchParameters.OutputFields.Add("Content");
            searchParameters.OutputFields.Add("CreateTime");
            IReadOnlyList<FieldData> results = await collection.QueryAsync(exp, searchParameters);
            if (results == null || results.Count == 0)
            {
                return $"{day} 无历史记录";
            }

            var contentField = results.FirstOrDefault(f => f.FieldName == "Content");
            var createTimeField = results.FirstOrDefault(f => f.FieldName == "CreateTime");


            var sb = new StringBuilder();
            sb.AppendLine($"【{day} 历史会话】");
            for (int i = 0; i < contentField.RowCount; i++)
            {
                string tcontent = (contentField as FieldData<string>).Data[i];
                long ttime = (createTimeField as FieldData<long>).Data[i];
                DateTime dttime = MyAccess.Core.TypeConvert.Unix2Time(ttime);
                sb.AppendLine(dttime.ToString("yyyy-MM-dd HH:mm:ss") + ";" + tcontent);
            }

            if (contentField.RowCount > 0)
            {
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }

        }
    }
}
