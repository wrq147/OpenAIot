using Common.Share;
using LLMService.Controller;
using LLMService.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Milvus.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace LLMService.Business
{
    public class KnowledgeRagBLL
    {
        private AiClientRegistry _registry;
        private readonly MilvusClient _client;
        public KnowledgeRagBLL(IOptions<LLMOption> option, AiClientRegistry registry)
        {
            _registry = registry;
            _client = new MilvusClient(new Uri(option.Value.MilvusUrl), database: option.Value.MilvusDatabase);
        }
        public async Task<BusResponse<string>> CreateRagCollection()
        {
            try
            {
                // 检查集合是否已存在
                var exists = await _client.HasCollectionAsync("Knowledges");
                if (exists)
                {
                    MilvusCollection tmpcollection = _client.GetCollection("Knowledges");
                    await tmpcollection.LoadAsync();
                    await tmpcollection.WaitForCollectionLoadAsync();
                    return BusResponse<string>.Error(111, "AI知识库已存在");
                }

                // 定义集合结构
                var schema = new CollectionSchema();
                schema.Fields.Add(FieldSchema.Create<long>("Id", isPrimaryKey: true, autoId: true));
                schema.Fields.Add(FieldSchema.Create<long>("OrgId"));
                schema.Fields.Add(FieldSchema.CreateVarchar("KbId", maxLength: 128));
                schema.Fields.Add(FieldSchema.CreateVarchar("Content", 8192));
                schema.Fields.Add(FieldSchema.CreateFloatVector("Embedding", dimension: 1024));

                var collection = await _client.CreateCollectionAsync("Knowledges", schema);
                // 创建索引以提高搜索性能
                await collection.CreateIndexAsync("Embedding", IndexType.Hnsw, SimilarityMetricType.Cosine);
                await collection.CreateIndexAsync(fieldName: "OrgId", indexType: IndexType.AutoIndex);
                await collection.LoadAsync();
                await collection.WaitForCollectionLoadAsync();
                return BusResponse<string>.Success();
            }
            catch
            {
                Console.WriteLine("数据库Milvus未启用，无法初始化Knowledges");
                return BusResponse<string>.Error(110, "张量数据库Milvus未启用");
            }
        }
        public async Task<BusResponse<int>> DelKnowledgeFromMilvus(string kbId)
        {
            try
            {
                MilvusCollection collection = _client.GetCollection("Knowledges");
                await collection.DeleteAsync("KbId=='" + kbId + "'");
                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(122, ex.Message);
            }
        }
        public async Task<BusResponse<int>> SyncPublicToMilvus(string rawQuery, string summaryText)
        {
            try
            {
                MilvusCollection collection = _client.GetCollection("Knowledges");
                var chunks = SplitMarkdownChunk(summaryText);
                var embedGen = _registry.GetDefaultEmbed();
                long targetOrgId = 0;
                string kbId = $"web_{Guid.NewGuid():N}";
                foreach (var item in chunks)
                {
                    StringBuilder chunkSb = new StringBuilder();
                    chunkSb.AppendLine($"【知识库数据来自全网网络搜索】");
                    chunkSb.AppendLine($"检索关键词：{rawQuery}");
                    chunkSb.AppendLine($"内容片段：{item}");
                    string chunkText = chunkSb.ToString();
                    var vector = await embedGen.GenerateVectorAsync(chunkText);

                    // 插入Milvus
                    List<ReadOnlyMemory<float>> embedVector = new();
                    embedVector.Add(vector);
                    await collection.InsertAsync(
                        new FieldData[]
                        {
                            FieldData.Create<long>("OrgId", new[] { targetOrgId }),
                            FieldData.CreateVarChar("KbId",  new[] { kbId }),
                            FieldData.CreateVarChar("Content",  new[] { chunkText }),
                            FieldData.CreateFloatVector("Embedding", embedVector)
                        });
                }

                await collection.FlushAsync();
                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(122, ex.Message);
            }
        }
        public async Task<BusResponse<int>> SyncArticleToMilvus(MZ_Knowledge knowledge, List<MZ_KbColumn> columns, List<MZ_Article> articles)
        {
            try
            {
                //清除旧的同步数据
                MilvusCollection collection = _client.GetCollection("Knowledges");

                await collection.DeleteAsync("KbId=='" + knowledge.Id + "'");
                long targetOrgId = knowledge.IsPublic == true ? 0 : knowledge.OrgId.Value;
                //同步新的数据
                var coldict = columns.ToDictionary(x => x.Id);
                var embedGen = _registry.GetDefaultEmbed();
                foreach (var art in articles)
                {
                    var tlist = GenerateChunks(knowledge.Name, art.Title, art.Title, art.Content);
                    foreach (var tstr in tlist)
                    {
                        var vec = await embedGen.GenerateVectorAsync(tstr);
                        List<ReadOnlyMemory<float>> embedVector = new();
                        embedVector.Add(vec);
                        await collection.InsertAsync(
                            new FieldData[]
                            {
                            FieldData.Create<long>("OrgId", new[] { targetOrgId }),
                            FieldData.CreateVarChar("KbId",  new[] { knowledge.Id }),
                            FieldData.CreateVarChar("Content",  new[] { tstr }),
                            FieldData.CreateFloatVector("Embedding", embedVector)
                            });


                    }
                }
                await collection.FlushAsync();
                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(122, ex.Message);
            }
        }
        private List<string> GenerateChunks(string kbName, string columnName, string title, string mdContent)
        {
            var rtlist = SplitMarkdownChunk(mdContent);
            List<string> chunks = new List<string>();
            foreach (string rt in rtlist)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"知识库：{kbName}");
                sb.AppendLine($"栏目：{columnName}");
                sb.AppendLine($"文章标题：{title}");
                sb.AppendLine($"内容片段：{rt}");
                chunks.Add(sb.ToString());
            }
            return chunks;
        }
        /// <summary>
        /// Markdown文本分块，保留完整md格式，无需清洗
        /// </summary>
        /// <param name="mdContent">原始Markdown内容</param>
        /// <param name="chunkSize">单块最大字符，推荐2000</param>
        /// <param name="overlapRate">文本重叠比例 0.1~0.15</param>
        /// <returns>分块列表（每一块都是完整可渲染的Markdown片段）</returns>
        private List<string> SplitMarkdownChunk(string mdContent, int chunkSize = 2000, double overlapRate = 0.12)
        {
            var chunks = new List<string>();
            if (string.IsNullOrWhiteSpace(mdContent))
                return chunks;

            // 1. 分割优先级：一级/二级标题 > 换行段落 > 中文句号/问号/感叹号
            var splitPattern = new Regex(@"(^#{1,2}\s.*$)|(\r\n\r\n)|([。！？])", RegexOptions.Multiline);
            var splitPoints = splitPattern.Matches(mdContent);

            int overlapLength = (int)(chunkSize * overlapRate);
            StringBuilder currentChunk = new StringBuilder();
            int lastSplitIndex = 0;

            foreach (Match match in splitPoints)
            {
                // 截取当前段落片段
                string seg = mdContent.Substring(lastSplitIndex, match.Index - lastSplitIndex + match.Length);
                lastSplitIndex = match.Index + match.Length;

                // 加入缓冲区后超出块阈值，切分
                if (currentChunk.Length + seg.Length > chunkSize)
                {
                    chunks.Add(currentChunk.ToString().Trim());
                    // 保留末尾重叠文本，避免语义断裂
                    if (currentChunk.Length > overlapLength)
                    {
                        string overlapText = currentChunk.ToString()
                            .Substring(currentChunk.Length - overlapLength);
                        currentChunk.Clear();
                        currentChunk.Append(overlapText);
                    }
                    else
                    {
                        currentChunk.Clear();
                    }
                }
                currentChunk.Append(seg);
            }

            // 处理剩余末尾文本
            if (lastSplitIndex < mdContent.Length)
            {
                string tailSeg = mdContent.Substring(lastSplitIndex);
                currentChunk.Append(tailSeg);
            }

            if (currentChunk.Length > 0)
                chunks.Add(currentChunk.ToString().Trim());

            return chunks;
        }

        public async Task<T_ToolResult> SearchRelatedAsync(long orgId, string query, float score = 0.6f)
        {
            try
            {
                var vec = await _registry.GetDefaultEmbed().GenerateVectorAsync(query);
                MilvusCollection collection = _client.GetCollection("Knowledges");

                SearchParameters searchParameters = new();
                searchParameters.OutputFields.Add("Content");
                searchParameters.Expression = "OrgId==" + orgId + " or OrgId==0";

                var results = await collection.SearchAsync(
                    vectorFieldName: "Embedding",
                    vectors: new ReadOnlyMemory<float>[] { vec },
                    SimilarityMetricType.Cosine,
                    limit: 20, searchParameters);


                bool hasmem = false;
                var sb = new StringBuilder();
                sb.AppendLine("【相关知识】");
                // 遍历每一条结果
                for (int i = 0; i < results.Scores.Count; i++)
                {
                    float rsscore = results.Scores[i];
                    if (rsscore > score)
                    {
                        string tcontent = (results.FieldsData.FirstOrDefault(f => f.FieldName == "Content") as FieldData<string>).Data[i];
                        sb.AppendLine(tcontent);
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
                        Output = "没有相关知识",
                        LogInfo = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                return new T_ToolResult()
                {
                    IsSuccess = false,
                    Output = "查询异常",
                    LogInfo = $"异常原因：{ex.Message}"
                };
            }

        }
    }
}
