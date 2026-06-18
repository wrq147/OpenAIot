using LLMService.Business;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Tool
{
    /// <summary>
    /// 长期记忆工具
    /// </summary>
    public static class LongMemory
    {
        public static AITool CreateGetHistoryByDateTool(ITAServiceProvider provider)
        {
            return AIFunctionFactory.Create(
                async ([Description("查询日期（格式：yyyy-MM-dd）")] string day) =>
                {
                    var context = FunctionInvokingChatClient.CurrentContext;
                    return await provider.GetService<MemoryRagBLL>().GetHistoryByDate(context.Options.ConversationId, day);
                },
              name: "搜索聊天历史",
              description: "查询指定日期的蒸馏总结后的聊天历史"
            );
        }

    }
}
