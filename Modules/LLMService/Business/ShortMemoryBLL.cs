using Common;
using LLMService.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Business
{
    public class ShortMemoryBLL
    {
        private ILogger<ShortMemoryBLL> _log;
        private GeneralRedisHelper _redis;
        private IAiClientRegistry _registry;
        private ITAServiceProvider _provider;
        public ShortMemoryBLL(IAiClientRegistry registry, ITAServiceProvider provider, GeneralRedisHelper redis, ILoggerFactory logFactory)
        {
            _registry = registry;
            _provider = provider;
            _redis = redis;
            _log = logFactory.CreateLogger<ShortMemoryBLL>();
        }
        /// <summary>
        /// 将短期记录存储到长期记忆
        /// </summary>
        /// <returns></returns>
        public async Task ProcessExpiredSessions()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            // 获取所有过期 session
            var expiredSessionIds = await _redis.SortedSetRangeByScoreAsync<string>("short_memory_expires", 0, now);

            foreach (var sessionId in expiredSessionIds)
            {
                await DoChatLock(sessionId, async () =>
                {
                    var historyText = await GetShortMemoryStr(sessionId);
                    // LLM 总结
                    var prompt = $@"
请对以下用户对话进行两项处理，并**严格按JSON格式返回**，不要额外说明：

1. summary：精简总结对话内容（长期记忆）
2. query：提取用户问题的核心主题、关键词，用于未来检索相关记忆

{historyText}

输出格式（必须合法JSON）：
{{
    ""summary"": ""这里写总结"",
    ""query"": ""这里写关键词/主题""
}}";
                    var chatClient = _registry.GetDefaultChat();
                    var response = await chatClient.GetResponseAsync(new List<ChatMessage>
                    {
                        new(ChatRole.System, "你是记忆处理助手，只输出标准JSON，无其他内容"),
                        new(ChatRole.User, prompt)
                    }, new ChatOptions()
                    {
                        ToolMode = ChatToolMode.None
                    });
                    var json = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Text);
                    string summary = json["summary"];
                    string query = json["query"];

                    // 存入长期记忆（带query用于相关度计算）
                    await _provider.GetService<MemoryRagBLL>().SaveMemoryAsync(sessionId, query, summary);
                    // 清理
                    await Clear(sessionId);
                });

            }
        }
        public async Task DoChatLock(string sessionId, Func<Task> func)
        {
            string tkey = $"ChatLock:{sessionId}";
            await _redis.WaitLockTakeAsync(tkey);
            try
            {
                await func.Invoke();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
            finally
            {
                await _redis.LockReleaseAsync(tkey);
            }
        }
        /// <summary>
        /// 保存单轮对话
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userInput"></param>
        /// <param name="aiReply"></param>
        public async Task SaveChat(string sessionId, string userInput, string aiReply)
        {
            await DoChatLock(sessionId, async () =>
            {
                string dataKey = $"short_memory:{sessionId}";
                var entry = new T_ShortMemory()
                {
                    User = userInput,
                    Assistant = aiReply,
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                await _redis.ListRightPushAsync(dataKey, entry);
                long expireTimestamp = DateTimeOffset.Now.AddHours(1).ToUnixTimeSeconds();
                await _redis.SortedSetAddAsync("short_memory_expires", sessionId, expireTimestamp);
            });
        }

        /// <summary>
        /// 获取最近对话记录
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<string> GetShortMemoryStr(string sessionId)
        {
            string key = $"short_memory:{sessionId}";

            var entries = await _redis.ListRangeAsync<T_ShortMemory>(key);

            if (entries.Count == 0)
                return "无短期记忆";

            var sb = new StringBuilder();
            sb.AppendLine("【最近对话记录】");

            foreach (var obj in entries)
            {
                sb.AppendLine($"时间：{obj.Time}");
                sb.AppendLine($"用户：{obj.User}");
                sb.AppendLine($"助手：{obj.Assistant}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public async Task<List<T_ShortMemory>> GetShortMemoryList(string sessionId)
        {
            string key = $"short_memory:{sessionId}";

            var entries = await _redis.ListRangeAsync<T_ShortMemory>(key);
            return entries;
        }

        /// <summary>
        /// 清空短期记忆
        /// </summary>
        /// <param name="sessionId"></param>
        public async Task Clear(string sessionId)
        {
            await _redis.KeyDeleteAsync($"short_memory:{sessionId}");
            await _redis.SortedSetRemoveAsync("short_memory_expires", sessionId);
        }

    }
}
