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
              provider.GetService<MemoryRagBLL>().SearchRelatedMemoriesAsync,
              name: "SearchRelatedMemoriesAsync",
              description: "根据用户问题搜索语义相关的历史聊天记忆"
            );
        }
    }
}
