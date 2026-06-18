using AuthService;
using AuthService.Business;
using Common;
using Common.EventBus;
using LLMService.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Business
{
    public class ChatBLL
    {
        private AiClientRegistry _registry;
        private ITAServiceProvider _provider;
        private ILogger<ChatBLL> _log;
        public ChatBLL(AiClientRegistry registry, ITAServiceProvider provider, ILoggerFactory logFactory)
        {
            _registry = registry;
            _provider = provider;
            _log = logFactory.CreateLogger<ChatBLL>();
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

            _ = StartChart(user, userInput);
        }

        private async Task StartChart(Data_ServerTokenInfo user, string userInput)
        {
            var sessionId = user.UserId.ToString();
            var tools = _registry.GetSkillTools();
            var chatClient = _registry.GetDefaultChat();
            var orgDAL = _provider.GetService<OrgDAL>();
            var orgInfo = await orgDAL.SelectById(user.OrgId);
            string companyName = string.Empty;
            string companyId = string.Empty;
            string companyAddr = string.Empty;
            string deptStr = string.Empty;
            string deptIds = string.Empty;
            string postStr = string.Empty;
            if (orgInfo != null)
            {
                companyName = orgInfo.OrgName;
                companyId = orgInfo.Id.ToString();
                var tmpcodeDict = await _provider.GetService<CodeBLL>().SelectAreaDict();
                companyAddr = tmpcodeDict.ToName(orgInfo.AddressCode) + orgInfo.AddressDetail;
                var userdepts = await orgDAL.SelectUserDept(user.UserId, user.OrgId);
                deptStr = string.Join(",", userdepts.Select(x => x.dept_name));
                deptIds = string.Join(",", userdepts.Select(x => x.dept_id));
                postStr = string.Join(",", userdepts.Select(x => x.post_name));
            }
            string useridentity = $@"当前用户信息：
- 企业Id：{companyId}
- 所属企业：{companyName}
- 企业地址：{companyAddr}
- 所在部门：{deptStr}
- 所在部门Id：{deptIds}
- 用户职位：{postStr}
- 用户ID：{user.UserId}
- 真实姓名或用户名：{user.UserName}";
            StringBuilder sysbuilder = new StringBuilder();
            sysbuilder.AppendLine(@"你是一个专业的智能助手。
请严格遵守以下规则：
1. 请根据用户身份提供合适的回答。
2. 优先使用提供的工具回答用户问题，工具返回结果后，用自然语言整理回答，不要暴露工具调用细节。
3. 无法区分信息来源时，直接多工具并行检索，避免信息缺失。
4. 不知道答案不要猜测，直接告诉用户无法回答。");
            sysbuilder.AppendLine(useridentity);
            var msgList = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System,sysbuilder.ToString()),
            };


            var shortMemorys = await _provider.GetService<ShortMemoryBLL>().GetShortMemoryList(sessionId);
            if (shortMemorys.Count > 0)
            {
                foreach (var chatItem in shortMemorys)
                {
                    msgList.Add(new ChatMessage(ChatRole.User, $"[{chatItem.Time}] {chatItem.User}"));
                    if (!string.IsNullOrEmpty(chatItem.Tool))
                    {
                        msgList.Add(new ChatMessage(ChatRole.Tool, $"[{chatItem.Time}] {chatItem.Tool}"));
                    }
                    msgList.Add(new ChatMessage(ChatRole.Assistant, $"[{chatItem.Time}] {chatItem.Assistant}"));
                }
                msgList.Add(new ChatMessage(ChatRole.User, userInput));
            }
            else
            {
                StringBuilder userInputBuilder = new StringBuilder();
                var ragResult = await _provider.GetService<MemoryRagBLL>().SearchRelatedMemoriesAsync(sessionId, userInput);
                if (ragResult.IsSuccess)
                {
                    userInputBuilder.AppendLine($"【历史对话参考信息】：\r\n{ragResult.Output}\r\n请结合以上历史信息回答后续问题");
                    userInputBuilder.AppendLine();
                }
                userInputBuilder.AppendLine(userInput);
                msgList.Add(new ChatMessage(ChatRole.User, userInputBuilder.ToString()));
            }

            var extInfo = new AdditionalPropertiesDictionary();
            extInfo.Add("UserInfo", user);
            extInfo.Add("UserStr", useridentity);
            var opt = new ChatOptions
            {
                ToolMode = ChatToolMode.Auto,
                Tools = tools,
                ConversationId = $"{sessionId}_{Guid.NewGuid():N}",
                AdditionalProperties = extInfo
            };
            StringBuilder toolResponse = new StringBuilder();
            StringBuilder aiFullResponse = new StringBuilder();
            var bus = _provider.GetService<NatsScope>().Bus;
            var resp = chatClient.GetStreamingResponseAsync(msgList, opt);
            try
            {
                await foreach (var update in resp)
                {
                    if (update == null)
                    {
                        return;
                    }
                    if (await GetSessionStatus(sessionId) == T_ChatStatus.Stoped)
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(update.Text))
                    {
                        await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>(){
                                sessionId,
                                "#llm"+update.Text
                            });
                        aiFullResponse.Append(update.Text);
                    }
                    if (update.Contents != null && update.Contents.Any())
                    {
                        foreach (var content in update.Contents)
                        {
                            if (content is TextContent textContent && !string.IsNullOrEmpty(textContent.Text))
                            {
                                await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>()
                                {
                                    sessionId,
                                    "#llm" + textContent.Text
                                });
                                aiFullResponse.Append(textContent.Text);
                            }
                            else if (content is FunctionCallContent toolCall)
                            {
                                string calltext = "正在调用工具 " + toolCall.Name + "\r\n";
                                await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>()
                                {
                                    sessionId,
                                    "#llm" + calltext
                                });
                                toolResponse.Append(calltext);
                            }
                            else if (content is FunctionResultContent toolRes)
                            {
                                string outputText = string.Empty;
                                string logText = string.Empty;
                                if (toolRes.Result is JsonElement jsonEle && jsonEle.ValueKind == JsonValueKind.Object)
                                {
                                    // 安全读取 Output
                                    if (jsonEle.TryGetProperty("output", out var outputEle))
                                    {
                                        outputText = outputEle.GetString() ?? string.Empty;
                                    }
                                    // 安全读取 LogInfo
                                    if (jsonEle.TryGetProperty("logInfo", out var logEle))
                                    {
                                        logText = logEle.GetString() ?? string.Empty;
                                    }
                                }
                                else
                                {
                                    outputText = toolRes.Result?.ToString() ?? "";
                                }
                                await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>()
                                    {
                                        sessionId,
                                        "#llm" + outputText
                                    });
                                aiFullResponse.AppendLine(outputText);
                                toolResponse.AppendLine(logText);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
            finally
            {
                //将本轮对话保存到短期记忆
                if (aiFullResponse.Length > 0)
                {
                    await _provider.GetService<ShortMemoryBLL>().SaveChat(sessionId, userInput, toolResponse.ToString(), aiFullResponse.ToString());
                }
                await SetSessionStatus(sessionId, T_ChatStatus.Active);
                await Task.Delay(100);
                await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>() { sessionId, "#llm" });
            }

        }
    }
}
