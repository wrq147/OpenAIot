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
        public async Task ChatAsync(Data_ServerTokenInfo user, string userInput, string thinkMode)
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

            _ = StartChart(user, userInput, thinkMode);
        }

        private async Task StartChart(Data_ServerTokenInfo user, string userInput, string thinkMode)
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
2. 如果工具所需参数未知，应该先调用其它工具获取参数数据。
3. 请结合之前的工具执行结果回答问题。
4. 不知道答案不要猜测，优先调用工具查询；如最终还是不知道答案，则直接告诉用户无法回答。
5. 工具执行结果应该整理后，由助手用自然语言回答。
6. 如果提问与近期有重复或近似时，必须主动调用长期记忆检索工具追溯历史上下文。
7. 禁止输出真实的数据库表名。
8. 调用工具时，只能输出标准FunctionCall结构，禁止纯文本描述要调用什么工具；");
            sysbuilder.AppendLine(useridentity);

            var msgList = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System,sysbuilder.ToString()),
            };

            var startTime = DateTime.Now.AddHours(-24);
            var endTime = DateTime.Now;
            var ragResult = await _provider.GetService<MemoryRagBLL>().GetHistoryByDate(sessionId, startTime, endTime);
            if (ragResult.IsSuccess)
            {
                msgList.Add(new ChatMessage(ChatRole.User, $"查询 {startTime.ToString("yyyy-MM-dd HH:mm:ss")}到{endTime.ToString("yyyy-MM-dd HH:mm:ss")} 的历史对话信息"));
                msgList.Add(new ChatMessage(ChatRole.Assistant, ragResult.Output));
            }

            var shortMemorys = await _provider.GetService<ShortMemoryBLL>().GetShortMemoryList(sessionId);
            if (shortMemorys.Count > 0)
            {
                foreach (var chatItem in shortMemorys)
                {
                    msgList.Add(new ChatMessage(ChatRole.User, $"[{chatItem.Time}] {chatItem.User}"));
                    if (!string.IsNullOrEmpty(chatItem.Tool))
                    {
                        msgList.Add(new ChatMessage(ChatRole.Assistant, $"[{chatItem.Time}] {chatItem.Tool}"));
                    }
                    msgList.Add(new ChatMessage(ChatRole.Assistant, $"[{chatItem.Time}] {chatItem.Assistant}"));
                }
            }


            msgList.Add(new ChatMessage(ChatRole.User, $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {userInput}"));

            var extInfo = new AdditionalPropertiesDictionary();
            extInfo.Add("UserInfo", user);
            extInfo.Add("UserStr", useridentity);
            ReasoningOptions reasoning;
            if (thinkMode == "deep")
            {
                reasoning = new ReasoningOptions()
                {
                    Effort = ReasoningEffort.Medium,
                    Output = ReasoningOutput.Full
                };
            }
            else
            {
                reasoning = new ReasoningOptions()
                {
                    Effort = ReasoningEffort.None,
                    Output = ReasoningOutput.None
                };
            }
            var opt = new ChatOptions
            {
                ToolMode = ChatToolMode.Auto,
                Tools = tools,
                ConversationId = sessionId,
                AdditionalProperties = extInfo,
                Reasoning = reasoning
            };
            StringBuilder toolResponse = new StringBuilder();
            StringBuilder aiFullResponse = new StringBuilder();
            var bus = _provider.GetService<NatsScope>().Bus;
            int maxTryCount = 10;
            bool needRetry;
            do
            {
                needRetry = true;
                var resp = chatClient.GetStreamingResponseAsync(msgList, opt);
                try
                {
                    await foreach (var update in resp)
                    {
                        if (await GetSessionStatus(sessionId) == T_ChatStatus.Stoped)
                        {
                            needRetry = false;
                            break;
                        }
                        if (update == null)
                        {
                            break;
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
                                            "#llma" + textContent.Text
                                        });
                                    aiFullResponse.Append(textContent.Text);
                                    needRetry = false;
                                }
                                else if (content is TextReasoningContent reasonInfo)
                                {
                                    //模型思考、推理过程文本
                                    if (!string.IsNullOrEmpty(reasonInfo.Text))
                                    {
                                        await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>()
                                        {
                                            sessionId,
                                            "#llmt" + reasonInfo.Text
                                        });
                                        toolResponse.Append(reasonInfo.Text);
                                    }
                                }
                                else if (content is UsageContent useInfo)
                                {
                                    //获取Token使用信息
                                }
                                else if (content is FunctionCallContent toolCall)
                                {
                                    string calltext = "正在调用工具 " + toolCall.Name + "\r\n";
                                    await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>()
                                    {
                                        sessionId,
                                        "#llmt" + calltext
                                    });
                                    string newtoolcall = $"[ToolCallId:{toolCall.CallId}]" + calltext;
                                    toolResponse.Append(newtoolcall);
                                    msgList.Add(new ChatMessage(ChatRole.Assistant, newtoolcall));
                                }
                                else if (content is FunctionResultContent toolRes)
                                {
                                    string outputText = string.Empty;
                                    string logText = string.Empty;
                                    bool isSuccess = true;
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
                                        if (jsonEle.TryGetProperty("isSuccess", out var ifsuccess))
                                        {
                                            isSuccess = ifsuccess.GetBoolean();
                                        }
                                    }
                                    else
                                    {
                                        outputText = toolRes.Result?.ToString() ?? "";
                                    }

                                    string tmptoolstr = $"[ToolCallId:{toolRes.CallId}的执行结果] {outputText}\r\n{logText}\r\n";
                                    toolResponse.AppendLine(tmptoolstr);
                                    msgList.Add(new ChatMessage(ChatRole.Assistant, tmptoolstr));
                                    if (isSuccess)
                                    {
                                        await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>()
                                        {
                                            sessionId,
                                            "#llmt正在分析工具执行结果\r\n"
                                        });
                                    }
                                    else
                                    {
                                        await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>()
                                        {
                                            sessionId,
                                            "#llmt正在分析工具执行异常原因，并重新尝试\r\n"
                                        });
                                        msgList.Add(new ChatMessage(ChatRole.User, $"需要分析异常原因，重新尝试回答 {userInput}"));
                                    }
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
                    --maxTryCount;
                }
            } while (needRetry && maxTryCount > 0);
            if (needRetry && maxTryCount <= 0)
            {
                await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>(){
                                        sessionId,
                                        "#llma\r\n任务中断，超过了最大执行轮数\r\n"
                                    });
                aiFullResponse.Clear();
            }
            //将本轮对话保存到短期记忆
            if (aiFullResponse.Length > 0)
            {
                await _provider.GetService<ShortMemoryBLL>().SaveChat(sessionId, userInput, toolResponse.ToString(), aiFullResponse.ToString());
            }
            await SetSessionStatus(sessionId, T_ChatStatus.Active);
            await Task.Delay(100);
            await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>() { sessionId, "#llma" });
        }
    }
}
