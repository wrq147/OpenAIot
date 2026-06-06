using AuthService;
using Common;
using Common.EventBus;
using Grpc.Core;
using LLMService.Model;
using Microsoft.Extensions.AI;
using MySqlX.XDevAPI;
using NATS.Client.Core;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Business
{
    public class ChatBLL
    {
        private IAiClientRegistry _registry;
        private ITAServiceProvider _provider;
        public ChatBLL(IAiClientRegistry registry, ITAServiceProvider provider)
        {
            _registry = registry;
            _provider = provider;
        }
        private async Task<T_ChatStatus> GetSessionStatus(string sessionId)
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            return await redis.StringGetAsync<T_ChatStatus>($"ChatStatus:{sessionId}");
        }
        private async Task SetSessionStatus(string sessionId, T_ChatStatus status)
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            await redis.StringSetAsync<T_ChatStatus>($"ChatStatus:{sessionId}", status, TimeSpan.FromMinutes(10));
        }
        public async Task ChatAsync(Data_ServerTokenInfo user, string userInput)
        {
            string sessionId = user.UserId.ToString();
            var status = await GetSessionStatus(sessionId);
            if (status == T_ChatStatus.Pending)
            {
                await SetSessionStatus(sessionId, T_ChatStatus.Stoped);
                await Task.Delay(500);
                int tccc = 0;
                while (await GetSessionStatus(sessionId) != T_ChatStatus.Stoped)
                {
                    await Task.Delay(500);
                    ++tccc;
                    if (tccc > 10)
                    {
                        return;
                    }
                }
            }
            await SetSessionStatus(sessionId, T_ChatStatus.Pending);

            StartChart(user, userInput);
        }

        private async void StartChart(Data_ServerTokenInfo user, string userInput)
        {
            var sessionId = user.UserId.ToString();
            var tools = _registry.GetSkillTools();
            var chatClient = _registry.GetDefaultChat();

            var msgList = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, @"你是一个专业的智能助手，请严格遵守以下规则：

1. 优先使用提供的工具（Tools）回答用户问题，禁止编造答案。
2. 只有在工具能解决问题时才调用工具；简单闲聊、问候不需要调用工具。
3. 调用工具时必须严格按照工具要求传入正确、完整的参数。
4. 如果参数不足，必须礼貌地向用户询问缺失信息，不要编造参数。
5. 工具返回结果后，用自然语言整理回答，不要暴露工具调用细节。
6. 不知道答案时不要猜测，直接告诉用户无法回答。
7. 保持回答简洁、专业、有礼貌。"),

            };
            var shortMemorys = await _provider.GetService<ShortMemoryBLL>().GetShortMemoryList(sessionId);
            if (shortMemorys.Count > 0)
            {
                foreach (var chatItem in shortMemorys)
                {
                    msgList.Add(new ChatMessage(ChatRole.User, $"[{chatItem.Time}] {chatItem.User}"));
                    msgList.Add(new ChatMessage(ChatRole.Assistant, $"[{chatItem.Time}] {chatItem.Assistant}"));
                }
            }
            msgList.Add(new ChatMessage(ChatRole.User, userInput));

            var extInfo = new AdditionalPropertiesDictionary();
            extInfo.Add("UserInfo", user);
            var opt = new ChatOptions
            {
                Tools = tools,
                ConversationId = user.UserId.ToString(),
                AdditionalProperties = extInfo
            };
            StringBuilder aiFullResponse = new StringBuilder();
            var bus = _provider.GetService<NatsScope>().Bus;
            var resp = chatClient.GetStreamingResponseAsync(msgList, opt);
            await foreach (var update in resp)
            {
                if (await GetSessionStatus(sessionId) == T_ChatStatus.Stoped)
                {
                    await SetSessionStatus(sessionId, T_ChatStatus.Active);
                    return;
                }
                if (!string.IsNullOrEmpty(update.Text))
                {
                    List<string> data = new List<string>();
                    data.Add("llmchat/" + sessionId);
                    data.Add(update.Text);
                    await bus.PublishAsync(new NatsMsg<List<string>>()
                    {
                        Subject = "MqttNotice.Msg",
                        Data = data
                    }, DefalutNatsJsonSerializer<List<string>>.Default);

                    aiFullResponse.Append(update.Text);
                }
            }
            //将本轮对话保存到短期记忆
            await _provider.GetService<ShortMemoryBLL>().SaveChat(opt.ConversationId, userInput, aiFullResponse.ToString());
        }
    }
}
