using LLMService.Skill;
using LLMService.Tool;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService
{
    public class AiClientRegistry
    {
        private readonly Dictionary<string, IChatClient> _chatMap = new();
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embedding;
        private readonly string _defChatKey;
        private readonly List<SkillMeta> _allSkills;
        private readonly List<AITool> _allTools;
        private ITAServiceProvider _provider;
        public AiClientRegistry(IOptions<LLMOption> cfg, ITAServiceProvider provider)
        {
            _provider = provider;
            //加载标准Skills目录
            string skillRoot = Path.Combine(Directory.GetCurrentDirectory(), "Skills");
            var skillLoader = new SkillLoader(skillRoot);
            _allSkills = skillLoader.LoadAllSkills();
            _allTools = _allSkills.BuildAIFunctions(ExecuteSkill);

            _defChatKey = cfg.Value.DefaultChatModel;
            //循环实例化各个模型（DeepSeek/GPT）
            foreach (var (key, opt) in cfg.Value.Models)
            {
                var apiKey = new ApiKeyCredential(opt.ApiKey);
                var openopt = new OpenAIClientOptions() { Endpoint = new Uri(opt.Endpoint) };
                var openAiCli = new OpenAIClient(apiKey, openopt);
                //对话客户端
                IChatClient chatCli = openAiCli.GetChatClient(key).AsIChatClient().AsBuilder()
                .UseFunctionInvocation()
                .Build();
                _chatMap[key] = chatCli;

                //向量客户端
                _embedding = new BpeLocalEmbeddingGenerator();
            }
        }
        public async Task InitTools()
        {
            _allTools.Add(SearchKnowledge.CreateSearchRelatedKnowledgeTool(_provider));
            _allTools.Add(LongMemory.CreateSearchRelatedMemoriesTool(_provider));
            _allTools.Add(LongMemory.CreateGetHistoryByDateTool(_provider));
            _allTools.Add(SystemTime.CreateSystemTimeTool(_provider));
            _allTools.Add(await SqlTableSearch.CreateSqlTableSearchTool(_provider));
        }

        /// <summary>
        /// 技能执行回调：外部注入业务逻辑（执行scripts脚本/本地API/自定义逻辑）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userRawInput"></param>
        /// <returns></returns>
        private async Task<string> ExecuteSkill(FunctionInvocationContext context, string userRawInput)
        {
            try
            {
                //1.查找技能元数据
                var skill = _allSkills.FirstOrDefault(s => s.Name == context.Function.Name);
                if (skill == null) return $"未找到技能:{context.Function.Name}";

                var promptSb = new StringBuilder();
                promptSb.AppendLine($"# 技能：{skill.Name}");
                promptSb.AppendLine(skill.Description);
                promptSb.AppendLine(skill.SkillInstruction);

                if (skill.ReferenceFiles.Any())
                {
                    promptSb.AppendLine("\n## 参考文档");
                    foreach (var f in skill.ReferenceFiles)
                        promptSb.AppendLine(File.ReadAllText(f));
                }
                if (skill.ScriptFiles.Any())
                {
                    promptSb.AppendLine("\n## 参考脚本模板");
                    foreach (var f in skill.ScriptFiles)
                        promptSb.AppendLine($"```\n{File.ReadAllText(f)}\n```");
                }
                if (skill.ExampleFiles.Any())
                {
                    promptSb.AppendLine("\n## 示例");
                    foreach (var f in skill.ExampleFiles)
                        promptSb.AppendLine(File.ReadAllText(f));
                }

                // 系统信息
                promptSb.AppendLine("\n## 运行环境");
                promptSb.AppendLine(OsPlatform.GetSystemPrompt());

                // 参数
                promptSb.AppendLine("\n## 用户原始输入语句：");
                promptSb.AppendLine(userRawInput);


                promptSb.AppendLine($@"
### 硬性要求：
1. 仅输出可直接在当前系统运行的完整代码，严格参考上面文档、代码模板、示例格式；
2. 代码适配本次传入参数，不要占位符、不要注释说明；
3. 可返回多个代码块，支持混合语言 python/bash/cmd，输出格式：```[lang]\n[code]\n```；
4. 优先沿用参考脚本的编程语言。");

                //调用LLM生成单次任务专属脚本
                var chat = GetDefaultChat();
                var resp = await chat.GetResponseAsync(new List<ChatMessage>
                {
                    new(ChatRole.System,"你是代码生成器，仅输出可运行代码块"),
                    new(ChatRole.User,promptSb.ToString())
                }, new ChatOptions()
                {
                    ToolMode = ChatToolMode.None
                });

                //解析LLM返回的代码块，落地到临时目录
                var scripts = ParseCodeBlocks(resp.Text);
                if (scripts == null)
                {
                    return "脚本格式错误";
                }
                string tempDir = Path.Combine(Path.GetTempPath(), $"skill_{Guid.NewGuid():N}");
                Directory.CreateDirectory(tempDir);
                var resultBuilder = new StringBuilder();
                int index = 1;
                foreach (var script in scripts)
                {
                    script.FilePath = Path.Combine(tempDir, $"task_{index++}.{script.Lang}");
                    await File.WriteAllTextAsync(script.FilePath, script.ScriptContent);

                    var runResult = await RunGeneratedScript(script);
                    resultBuilder.AppendLine($"\n--- 脚本 {index} 执行结果 ---");
                    resultBuilder.AppendLine(runResult);
                }
                //清理临时脚本目录
                Directory.Delete(tempDir, true);

                return $"【技能执行完成】\n{resultBuilder}";
            }
            catch (Exception ex)
            {
                return $"技能执行异常:{ex.Message}";
            }
        }
        private List<GeneratedTaskScript> ParseCodeBlocks(string llmText)
        {
            var scripts = new List<GeneratedTaskScript>();
            var matches = Regex.Matches(llmText, @"```(?<lang>\w+)\s*\n(?<code>.+?)\n```", RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                string lang = match.Groups["lang"].Value.ToLower();
                string code = match.Groups["code"].Value;

                string finalLang = lang switch
                {
                    "py" or "python" => "py",
                    "bash" or "sh" => "sh",
                    "bat" or "cmd" => "cmd",
                    _ => "unknown"
                };

                if (finalLang != "unknown")
                {
                    scripts.Add(new GeneratedTaskScript
                    {
                        Lang = finalLang,
                        ScriptContent = code
                    });
                }
            }

            if (scripts.Count == 0)
                return null;

            return scripts;
        }
        private async Task<string> RunGeneratedScript(GeneratedTaskScript script)
        {
            var psi = new ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(script.FilePath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var langval = script.Lang.ToLower();
            if (langval == "py")
            {
                psi.FileName = "python";
                psi.Arguments = $"\"{script.FilePath}\"";
            }
            else if (langval == "sh")
            {
                psi.FileName = "/bin/bash";
                psi.Arguments = $"\"{script.FilePath}\"";
            }
            else if (langval == "cmd")
            {
                psi.FileName = "cmd.exe";
                psi.Arguments = $"/c \"{script.FilePath}\"";
            }
            else
            {
                return $"不支持脚本类型:{script.Lang}";
            }
            using var proc = Process.Start(psi)!;
            var stdout = await proc.StandardOutput.ReadToEndAsync();
            var stderr = await proc.StandardError.ReadToEndAsync();
            await proc.WaitForExitAsync();
            return $"[标准输出]\n{stdout}\n[错误信息]\n{stderr}";
        }
        public IChatClient GetChatClient(string modelKey) => _chatMap[modelKey];
        public IChatClient GetDefaultChat() => _chatMap[_defChatKey];
        public IEmbeddingGenerator<string, Embedding<float>> GetDefaultEmbed() => _embedding;
        public List<SkillMeta> GetAllSkills() => _allSkills;
        public List<AITool> GetSkillTools() => _allTools;
    }
}
