using ElBruno.LocalEmbeddings;
using ElBruno.LocalEmbeddings.Options;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace LLMService
{
    public class AiClientRegistry : IAiClientRegistry
    {
        private readonly Dictionary<string, IChatClient> _chatMap = new();
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embedding;
        private readonly string _defChatKey;

        public AiClientRegistry(IOptions<LLMOption> cfg)
        {
            _defChatKey = cfg.Value.DefaultChatModel;

            //循环实例化各个模型（DeepSeek/GPT）
            foreach (var (key, opt) in cfg.Value.Models)
            {
                var apiKey = new ApiKeyCredential(opt.ApiKey);
                var openopt = new OpenAIClientOptions() { Endpoint = new Uri(opt.Endpoint) };
                var openAiCli = new OpenAIClient(apiKey, openopt);
                //对话客户端
                string chatModel = key == "DeepSeek" ? "deepseek-chat" : "gpt-4o";
                IChatClient chatCli = openAiCli.GetChatClient(chatModel).AsIChatClient();
                _chatMap[key] = chatCli;

                //向量客户端
                string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"EmbedModel";
                _embedding = new LocalEmbeddingGenerator(new LocalEmbeddingsOptions
                {
                    ModelPath = modelPath
                });
            }
        }

        public IChatClient GetChatClient(string modelKey) => _chatMap[modelKey];
        public IChatClient GetDefaultChat() => _chatMap[_defChatKey];
        public IEmbeddingGenerator<string, Embedding<float>> GetDefaultEmbed() => _embedding;
    }
}
