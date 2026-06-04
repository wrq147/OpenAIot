using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Skill
{
    /// <summary>
    /// Agent Skill 标准结构
    /// </summary>
    public class SkillMeta
    {
        #region YAML FrontMatter 必填/可选字段(SKILL.md头部---之间)
        /// <summary>必填，小写+短横线，和文件夹同名</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>必填：使用场景描述，AI用来判断何时调用</summary>
        public string Description { get; set; } = string.Empty;
        public string[]? AllowedTools { get; set; }
        public string[]? DisallowedTools { get; set; }
        public string? Effort { get; set; }
        public int? MaxTurns { get; set; }
        public string? Model { get; set; }
        public string? Version { get; set; }
        #endregion

        /// <summary>SKILL.md正文：完整Markdown任务SOP、步骤、示例</summary>
        public string SkillInstruction { get; set; } = string.Empty;
        /// <summary>技能文件夹名称（规范要求和Name一致）</summary>
        public string SkillDirName { get; set; } = string.Empty;

        #region 附属资源路径（scripts/references/examples）
        /// <summary>scripts目录下全部可执行脚本全路径</summary>
        public List<string> ScriptFiles { get; set; } = new();
        /// <summary>参考文档目录</summary>
        public List<string> ReferenceFiles { get; set; } = new();
        /// <summary>示例文件目录</summary>
        public List<string> ExampleFiles { get; set; } = new();
        #endregion
    }
}
