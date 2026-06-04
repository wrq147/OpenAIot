using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService
{
    /// <summary>
    /// LLM生成的脚本信息
    /// </summary>
    public class GeneratedTaskScript
    {
        public string FilePath { get; set; } //临时脚本全路径（放在临时目录）
        public string ScriptContent { get; set; } //生成代码
        public string Lang { get; set; } //py/sh/csharp
    }
}
