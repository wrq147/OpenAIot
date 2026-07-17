using Common;
using LLMService.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Business
{
    /// <summary>
    /// 全网搜索业务层：对接SearXNG，搜索结果存入公共Milvus知识库
    /// </summary>
    public class WebSearchBLL
    {
        private LLMOption _llmCfg;
        private AiClientRegistry _registry;
        private ITAServiceProvider _serverProvider;
        /// <summary>
        /// 构造注入服务
        /// </summary>
        public WebSearchBLL(IOptions<LLMOption> option, AiClientRegistry registry, ITAServiceProvider provider)
        {
            _llmCfg = option.Value;
            _registry = registry;
            _serverProvider = provider;
        }

        /// <summary>
        /// 调用SearXNG接口执行全网搜索
        /// </summary>
        /// <param name="searchQuery">用户检索关键词</param>
        /// <returns>结构化搜索条目列表</returns>
        public async Task<List<WebSearchItem>> SearchWebAsync(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return new List<WebSearchItem>();

            // URL编码防止中文乱码
            string encodeQ = Uri.EscapeDataString(searchQuery);
            string reqUrl = $"{_llmCfg.SearXNGUrl}/search?q={encodeQ}&format=json";
            string jsonText = await HttpHelper.Instance.GetAsync(reqUrl); ;
            var jsonOpt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            SearxNGResp searxResult;
            try
            {
                searxResult = JsonSerializer.Deserialize<SearxNGResp>(jsonText, jsonOpt);
            }
            catch
            {
                // JSON解析失败，返回空列表
                return new List<WebSearchItem>();
            }

            if (searxResult?.results == null || searxResult.Count == 0)
                return new List<WebSearchItem>();

            // 转换为业务统一实体
            var itemList = new List<WebSearchItem>();
            foreach (var item in searxResult.results)
            {
                itemList.Add(new WebSearchItem
                {
                    Title = item.title ?? string.Empty,
                    Url = item.url ?? string.Empty,
                    Snippet = item.content ?? string.Empty
                });
            }
            return itemList;
        }

        private async Task<string> SummarizeWebContentAsync(string rawQuery, List<WebSearchItem> items)
        {
            var chatClient = _registry.GetDefaultChat();
            var msgList = new List<ChatMessage>();
            string sysPrompt = $@"你是专业文本摘要助手。要求：
1. 客观提取全部关键信息，无主观评价；
2. 分点结构化输出，逻辑清晰，字数压缩至原文40%以内；
3. 结构化分点，去除广告、重复、无效话术；
4. 每条信息附带来源网页标题；
5. 只输出纯中文文本，禁止markdown、特殊符号。";
            msgList.Add(new ChatMessage(ChatRole.System, sysPrompt));
            StringBuilder userSb = new StringBuilder();
            foreach (var item in items)
            {
                userSb.AppendLine($"【{item.Title}】{item.Snippet}\r\n");
            }
            msgList.Add(new ChatMessage(ChatRole.User, userSb.ToString()));
            var response = await chatClient.GetResponseAsync(msgList);
            return response.Text;
        }

        /// <summary>
        /// 搜索结果异步存入公共知识库（后台执行不阻塞对话）
        /// </summary>
        public async Task SaveSearchToPublicKb(string rawQuery, List<WebSearchItem> items)
        {
            var knowledgeRagBll = _serverProvider.GetService<KnowledgeRagBLL>();
            string summaryText;
            try
            {
                summaryText = await SummarizeWebContentAsync(rawQuery, items);
            }
            catch (Exception ex)
            {
                StringBuilder fallback = new StringBuilder($"检索关键词：{rawQuery}\n");
                foreach (var i in items) fallback.AppendLine($"【{i.Title}】{i.Snippet}\n");
                summaryText = fallback.ToString();
            }
            await knowledgeRagBll.SyncPublicToMilvus(rawQuery, summaryText);

        }

        /// <summary>
        /// 对外统一工具执行入口，返回标准T_ToolResult
        /// </summary>
        public async Task<T_ToolResult> ExecuteWebSearchTool(string searchQuery)
        {
            try
            {
                if (string.IsNullOrEmpty(_llmCfg.SearXNGUrl))
                {
                    return new T_ToolResult
                    {
                        IsSuccess = false,
                        Output = "全网网络搜索工具未启用",
                        LogInfo = string.Empty
                    };
                }
                // 1. 调用搜索接口
                var itemList = await SearchWebAsync(searchQuery);
                if (itemList.Count == 0)
                {
                    return new T_ToolResult
                    {
                        IsSuccess = false,
                        Output = "全网搜索未查询到相关网页资料",
                        LogInfo = $"SearXNG无返回结果，检索词：{searchQuery}"
                    };
                }

                // 2. 拼接给大模型阅读的可读文本
                StringBuilder outputSb = new StringBuilder("【全网检索结果汇总】\n");
                foreach (var item in itemList)
                {
                    outputSb.AppendLine($"标题：{item.Title}");
                    outputSb.AppendLine($"链接：{item.Url}");
                    outputSb.AppendLine($"摘要：{item.Snippet}\n");
                }

                // 3. 异步后台入库，不阻塞当前对话流程
                _ = Task.Run(async () => await SaveSearchToPublicKb(searchQuery, itemList));

                // 4. 返回成功结果
                return new T_ToolResult
                {
                    IsSuccess = true,
                    Output = outputSb.ToString(),
                    LogInfo = $"成功检索到{itemList}条网页，结果将自动存入企业公共知识库"
                };
            }
            catch (Exception ex)
            {
                // 全局异常捕获
                return new T_ToolResult
                {
                    IsSuccess = false,
                    Output = "全网搜索接口调用异常，暂时无法获取互联网信息",
                    LogInfo = $"WebSearch异常：{ex.Message}，检索词：{searchQuery}"
                };
            }
        }
    }

    #region 配套实体类
    /// <summary>
    /// SearXNG标准返回根实体
    /// </summary>
    public class SearxNGResp
    {
        public string query { get; set; }
        public List<SearxItem> results { get; set; } = new List<SearxItem>();
        public List<List<string>> unresponsive_engines { get; set; } = new List<List<string>>();
        public int Count => results.Count;
    }

    /// <summary>
    /// SearXNG单条搜索结果
    /// </summary>
    public class SearxItem
    {
        public string title { get; set; }
        public string url { get; set; }
        public string content { get; set; }
    }

    /// <summary>
    /// 业务层统一网页条目实体
    /// </summary>
    public class WebSearchItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Snippet { get; set; }
    }
    #endregion
}
