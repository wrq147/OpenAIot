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
    public static class HttpWebSearch
    {
        public static AITool CreateHttpWebSearchTool(ITAServiceProvider provider)
        {
            return AIFunctionFactory.Create(
                async ([Description("需要全网检索的用户问题关键词")] string searchQuery) =>
                {
                    var bll = provider.GetService<WebSearchBLL>();
                    return await bll.ExecuteWebSearchTool(searchQuery);
                },
                name: "全网网络搜索",
                description: """
                全网互联网搜索工具，仅允许在【长期记忆检索、业务知识库检索均无匹配内容】时调用。
                功能：通过互联网检索公开资料、实时资讯、外部业务常识；禁止优先调用此工具，必须先查询记忆与本地知识库。
                """
            );
        }
    }
}
