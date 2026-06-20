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
        private AiClientRegistry _registry;
        private ITAServiceProvider _provider;
        public ShortMemoryBLL(AiClientRegistry registry, ITAServiceProvider provider, GeneralRedisHelper redis, ILoggerFactory logFactory)
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
                    if (string.IsNullOrEmpty(historyText))
                    {
                        return;
                    }
                    // LLM 总结
                    var prompt = $@"
请对以下用户对话进行精简总结，字数不能超过2500个字，并且只需要返回总结内容，不要额外说明：

{historyText}

";
                    var chatClient = _registry.GetDefaultChat();
                    var response = await chatClient.GetResponseAsync(new List<ChatMessage>
                    {
                        new(ChatRole.System, "你是记忆处理助手，只输出精简总结，无其他内容"),
                        new(ChatRole.User, prompt)
                    });
                    string summary = response.Text;

                    // 存入长期记忆（带query用于相关度计算）
                    await _provider.GetService<MemoryRagBLL>().SaveMemoryAsync(sessionId, summary);
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
        /// <param name="tool"></param>
        /// <param name="aiReply"></param>
        /// <returns></returns>
        public async Task SaveChat(string sessionId, string userInput, string tool, string aiReply)
        {
            await DoChatLock(sessionId, async () =>
            {
                string dataKey = $"short_memory:{sessionId}";
                var entry = new T_ShortMemory()
                {
                    User = userInput,
                    Tool = tool,
                    Assistant = aiReply,
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                await _redis.ListRightPushAsync(dataKey, entry);
                long expireTimestamp = DateTimeOffset.Now.AddMinutes(30).ToUnixTimeSeconds();
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
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("【最近对话记录】");

            foreach (var obj in entries)
            {
                sb.AppendLine($"时间：{obj.Time}");
                sb.AppendLine($"用户：{obj.User}");
                if (string.IsNullOrEmpty(obj.Tool))
                {
                    sb.AppendLine($"助手：{obj.Assistant}");
                }
                else
                {
                    sb.AppendLine($"助手：{obj.Tool}\r\n\r\n{obj.Assistant}");
                }
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
