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
    /// 相关知识搜索
    /// </summary>
    public static class SearchKnowledge
    {
        public static AITool CreateSearchRelatedKnowledgeTool(ITAServiceProvider provider)
        {
            return AIFunctionFactory.Create(
              async ([Description("用户待检索的业务问题，用于向量匹配知识库文档片段")] string query, 
              [Description("文档相关度过滤阈值，仅返回相似度大于该值的文档，默认0.6")] float score = 0.6f) =>
              {
                  var context = FunctionInvokingChatClient.CurrentContext;
                  return await provider.GetService<KnowledgeRagBLL>().SearchRelatedAsync(context.Options.ConversationId, query, score);
              },
              name: "SearchRelatedAsync",
              description: """
                业务知识库检索工具，根据用户自然语言问题匹配库内相似文档片段。
                使用场景：
                1. 用户询问业务规则、流程、产品说明、历史配置等知识库内存储的资料；
                2. 回答问题缺少依据时，调用本工具获取参考原文；
                """
            );
        }
    }
}
