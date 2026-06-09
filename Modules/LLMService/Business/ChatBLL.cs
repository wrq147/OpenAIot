using AuthService;
using AuthService.Business;
using Common;
using Common.EventBus;
using LLMService.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
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
        private ILogger<ChatBLL> _log;
        public ChatBLL(IAiClientRegistry registry, ITAServiceProvider provider, ILoggerFactory logFactory)
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

            StartChart(user, userInput);
        }

        private async void StartChart(Data_ServerTokenInfo user, string userInput)
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
            var msgList = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, $@"你是一个专业的智能助手。
当前用户信息：
- 企业Id：{companyId}
- 所属企业：{companyName}
- 企业地址：{companyAddr}
- 所在部门：{deptStr}
- 所在部门Id：{deptIds}
- 用户职位：{postStr}
- 用户ID：{user.UserId}
- 真实姓名或用户名：{user.UserName}

请严格遵守以下规则：
1. 请根据用户身份提供合适的回答
2. 优先使用提供的工具回答用户问题，工具返回结果后，用自然语言整理回答，不要暴露工具调用细节。
3. 不知道答案时不要猜测，直接告诉用户无法回答。
4. 保持回答简洁、专业、有礼貌。"),

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
                ToolMode = ChatToolMode.Auto,
                Tools = tools,
                ConversationId = user.UserId.ToString(),
                AdditionalProperties = extInfo
            };
            StringBuilder aiFullResponse = new StringBuilder();
            var bus = _provider.GetService<NatsScope>().Bus;
            var resp = chatClient.GetStreamingResponseAsync(msgList, opt);
            try
            {
                await foreach (var update in resp)
                {
                    if (await GetSessionStatus(sessionId) == T_ChatStatus.Stoped)
                    {
                        await SetSessionStatus(sessionId, T_ChatStatus.Active);
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
                    await _provider.GetService<ShortMemoryBLL>().SaveChat(opt.ConversationId, userInput, aiFullResponse.ToString());
                }
                await SetSessionStatus(sessionId, T_ChatStatus.Active);
                await Task.Delay(100);
                await TAEventDispatcher.Instance.Dispatch("Mqtt.User.New", new List<string>() { sessionId, "#llm" });
            }

        }
    }
}
