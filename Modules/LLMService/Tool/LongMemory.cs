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
        public static AITool CreateSearchRelatedMemoriesTool(ITAServiceProvider provider)
        {
            return AIFunctionFactory.Create(
              async ([Description("用户的问题")] string query, [Description("相关度阈值，默认0.6")] float score = 0.6f) =>
              {
                  var context = FunctionInvokingChatClient.CurrentContext;
                  return await provider.GetService<MemoryRagBLL>().SearchRelatedMemoriesAsync(context.Options.ConversationId, query, score);
              },
              name: "长期记忆检索工具",
              description: "根据用户问题搜索语义相关的历史聊天记忆，用户最近有重复提问、询问往期业务、提及之前对话内容时使用"
            );
        }
        public static AITool CreateGetHistoryByDateTool(ITAServiceProvider provider)
        {
            return AIFunctionFactory.Create(
                async ([Description("开始时间（格式：yyyy-MM-dd HH:mm:ss）")] string start, [Description("结束时间（格式：yyyy-MM-dd HH:mm:ss）")] string end) =>
                {
                    var context = FunctionInvokingChatClient.CurrentContext;
                    if (!DateTime.TryParse(start, out DateTime searchStartDT))
                    {
                        return new T_ToolResult()
                        {
                            IsSuccess = false,
                            Output = $"{start} 开始时间参数格式错误",
                            LogInfo = string.Empty
                        };
                    }
                    if (!DateTime.TryParse(end, out DateTime searchEndDT))
                    {
                        return new T_ToolResult()
                        {
                            IsSuccess = false,
                            Output = $"{end} 结束时间参数格式错误",
                            LogInfo = string.Empty
                        };
                    }
                    return await provider.GetService<MemoryRagBLL>().GetHistoryByDate(context.Options.ConversationId, searchStartDT, searchEndDT);
                },
              name: "搜索聊天历史",
              description: "查询指定日期范围的蒸馏总结后的聊天历史，时间范围不能超过2天。"
            );
        }

    }
}
