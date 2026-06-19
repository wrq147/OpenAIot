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
        private AiClientRegistry _registry;
        private readonly MilvusClient _client;
        public MemoryRagBLL(IOptions<LLMOption> option, AiClientRegistry registry)
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
                    MilvusCollection tmpcollection = _client.GetCollection("ChatMemory");
                    await tmpcollection.LoadAsync();
                    await tmpcollection.WaitForCollectionLoadAsync();
                    return BusResponse<string>.Error(111, "AI记忆向量表已存在");
                }

                // 定义集合结构
                var schema = new CollectionSchema();
                schema.Fields.Add(FieldSchema.Create<long>("Id", isPrimaryKey: true, autoId: true));
                schema.Fields.Add(FieldSchema.CreateVarchar("SessionId", maxLength: 256));
                schema.Fields.Add(FieldSchema.CreateVarchar("Content", 8192));
                schema.Fields.Add(FieldSchema.CreateFloatVector("Embedding", dimension: 1024));
                schema.Fields.Add(FieldSchema.Create<long>("CreateTime"));

                var collection = await _client.CreateCollectionAsync("ChatMemory", schema);
                // 创建索引以提高搜索性能
                await collection.CreateIndexAsync("Embedding", IndexType.Hnsw, SimilarityMetricType.Cosine);
                await collection.CreateIndexAsync(fieldName: "SessionId", indexType: IndexType.AutoIndex);
                await collection.CreateIndexAsync(fieldName: "CreateTime", indexType: IndexType.AutoIndex);
                await collection.LoadAsync();
                await collection.WaitForCollectionLoadAsync();
                return BusResponse<string>.Success();
            }
            catch
            {
                Console.WriteLine("数据库Milvus未启用，无法初始化ChatMemory");
                return BusResponse<string>.Error(110, "张量数据库Milvus未启用");
            }

        }

        public async Task SaveMemoryAsync(string sessionId, string content)
        {
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

        public async Task<T_ToolResult> SearchRelatedMemoriesAsync(string sessionId, string query, float score = 0.6f)
        {
            try
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
                // 遍历每一条结果
                for (int i = 0; i < results.Scores.Count; i++)
                {
                    float rsscore = results.Scores[i];
                    if (rsscore > score)
                    {
                        var contentData = results.FieldsData.FirstOrDefault(f => f.FieldName == "Content");
                        var timeData = results.FieldsData.FirstOrDefault(f => f.FieldName == "CreateTime");
                        string tcontent = (contentData as FieldData<string>).Data[i];
                        long ttime = (timeData as FieldData<long>).Data[i];
                        DateTime dttime = MyAccess.Core.TypeConvert.Unix2Time(ttime);
                        sb.AppendLine(dttime.ToString("yyyy-MM-dd HH:mm:ss") + ";" + tcontent);
                        hasmem = true;
                    }
                }

                if (hasmem)
                {
                    return new T_ToolResult()
                    {
                        IsSuccess = true,
                        Output = sb.ToString(),
                        LogInfo = string.Empty
                    };
                }
                else
                {
                    return new T_ToolResult()
                    {
                        IsSuccess = false,
                        Output = "没有相关的历史会话记录",
                        LogInfo = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                return new T_ToolResult()
                {
                    IsSuccess = false,
                    Output = "工具调用异常",
                    LogInfo = $"异常原因：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 查询指定日期的历史会话
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<T_ToolResult> GetHistoryByDate(string sessionId, string start, string end)
        {
            try
            {
                if (!DateTime.TryParse(start, out DateTime searchStartDT))
                {
                    return new T_ToolResult()
                    {
                        IsSuccess = false,
                        Output = $"{start} 开始时间参数格式错误",
                        LogInfo = string.Empty
                    };
                }
                if (!DateTime.TryParse(end, out DateTime searchEndDT))
                {
                    return new T_ToolResult()
                    {
                        IsSuccess = false,
                        Output = $"{end} 结束时间参数格式错误",
                        LogInfo = string.Empty
                    };
                }
                if (end < start)
                {
                    return new T_ToolResult()
                    {
                        IsSuccess = false,
                        Output = $"结束时间不能小于开始时间",
                        LogInfo = string.Empty
                    };
                }
                MilvusCollection collection = _client.GetCollection("ChatMemory");
                long startll = MyAccess.Core.TypeConvert.Time2Unix(start);
                long endll = MyAccess.Core.TypeConvert.Time2Unix(end);
                string exp = "SessionId==\"" + sessionId + "\" AND CreateTime>=" + startll + " AND CreateTime<=" + endll;
                QueryParameters searchParameters = new();
                searchParameters.OutputFields.Add("Content");
                searchParameters.OutputFields.Add("CreateTime");
                IReadOnlyList<FieldData> results = await collection.QueryAsync(exp, searchParameters);
                if (results == null || results.Count == 0)
                {
                    return new T_ToolResult()
                    {
                        IsSuccess = false,
                        Output = $"{day} 无历史记录",
                        LogInfo = string.Empty
                    };
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
                    return new T_ToolResult()
                    {
                        IsSuccess = true,
                        Output = sb.ToString(),
                        LogInfo = string.Empty
                    };
                }
                else
                {
                    return new T_ToolResult()
                    {
                        IsSuccess = false,
                        Output = string.Empty,
                        LogInfo = string.Empty
                    };
                }

            }
            catch (Exception ex)
            {
                return new T_ToolResult()
                {
                    IsSuccess = false,
                    Output = "工具调用异常",
                    LogInfo = $"异常原因：{ex.Message}"
                };
            }


        }
    }
}
