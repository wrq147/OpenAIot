using AuthService;
using Common.Share;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Business
{
    public class ChatBLL
    {
        private IAiClientRegistry _registry;
        public ChatBLL(IAiClientRegistry registry)
        {
            _registry = registry;
        }

        public virtual async Task<string> ChatAsync(Data_ServerTokenInfo user, string userInput)
        {
            var chatClient = _registry.GetDefaultChat();
            var tools = _registry.GetSkillTools();

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
                new ChatMessage(ChatRole.User, userInput)
            };

            var opt = new ChatOptions { Tools = tools };
            var resp = await chatClient.GetResponseAsync(msgList, opt);

            //自动处理函数调用（Microsoft.Extensions.AI中间件自动回调ExecuteSkill）
            return resp.Text;
        }
    }
}
