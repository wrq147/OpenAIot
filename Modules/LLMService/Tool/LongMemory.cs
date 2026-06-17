using LLMService.Business;
using Microsoft.Extensions.AI;
using MySqlX.XDevAPI;
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
        public static AITool CreateSearchRelatedMemoriesTool(ITAServiceProvider provider)
        {
            return AIFunctionFactory.Create(
              async ([Description("用户的问题")] string query, [Description("相关度阈值，默认0.6")] float score = 0.6f) =>
              {
                  var context = FunctionInvokingChatClient.CurrentContext;
                  return await provider.GetService<MemoryRagBLL>().SearchRelatedMemoriesAsync(context.Options.ConversationId, query, score);
              },
              name: "搜索相关的长期记忆",
              description: "根据用户问题搜索语义相关的蒸馏总结后的历史聊天记忆"
            );
        }
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
