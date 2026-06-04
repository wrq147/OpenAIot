using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace LLMService.Skill
{
    public class SkillLoader
    {
        private readonly string _rootSkillPath;
        private readonly IDeserializer _yamlDeserializer;

        public SkillLoader(string rootSkillDirectory)
        {
            _rootSkillPath = rootSkillDirectory;
            _yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties() //兼容扩展yaml字段不报错
                .Build();
        }

        /// <summary>批量加载全部Claude标准Skill</summary>
        public List<SkillMeta> LoadAllSkills()
        {
            var skillList = new List<SkillMeta>();
            if (!Directory.Exists(_rootSkillPath)) return skillList;

            //遍历一级子文件夹=单个Skill
            foreach (var skillFolder in Directory.GetDirectories(_rootSkillPath))
            {
                string skillMdPath = Path.Combine(skillFolder, "SKILL.md");
                if (!File.Exists(skillMdPath)) continue;

                var skill = ParseSingleSkill(skillFolder, skillMdPath);
                skillList.Add(skill);
            }
            return skillList;
        }

        private SkillMeta ParseSingleSkill(string skillDir, string mdPath)
        {
            var content = File.ReadAllText(mdPath);
            //正则拆分FrontMatter(---包裹yaml)和正文Markdown，官方固定格式
            var match = Regex.Match(content, @"^\-\-\-\r?\n(.*?)\r?\n\-\-\-\r?\n(.*)", RegexOptions.Singleline);

            string yamlText = match.Groups[1].Value.Trim();
            string instructionText = match.Groups[2].Value.Trim();

            var meta = _yamlDeserializer.Deserialize<SkillMeta>(yamlText);
            meta.SkillInstruction = instructionText;
            meta.SkillDirName = Path.GetFileName(skillDir);

            //扫描三类附属目录
            meta.ScriptFiles = ScanDirFiles(Path.Combine(skillDir, "scripts"));
            meta.ReferenceFiles = ScanDirFiles(Path.Combine(skillDir, "references"));
            meta.ExampleFiles = ScanDirFiles(Path.Combine(skillDir, "examples"));

            return meta;
        }

        private List<string> ScanDirFiles(string dir)
        {
            var res = new List<string>();
            if (!Directory.Exists(dir)) return res;
            res.AddRange(Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories));
            return res;
        }
    }
}
