using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService
{
    /// <summary>
    /// 全局模型注册表
    /// </summary>
    public interface IAiClientRegistry
    {
        IChatClient GetChatClient(string modelKey);
        IChatClient GetDefaultChat();
        IEmbeddingGenerator<string, Embedding<float>> GetDefaultEmbed();
    }
}
