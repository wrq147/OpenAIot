using Microsoft.Extensions.AI;
using Mysqlx.Expr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LLMService.Skill
{
    /// <summary>
    /// Skill转换器：把Skill转MEAI标准AIFunction
    /// </summary>
    public static class SkillConverter
    {
        /// <summary>批量转为AI可调用Function工具</summary>
        public static List<AITool> BuildAIFunctions(this List<SkillMeta> skills, Func<string, string, Task<string>> skillExecuteHandler)
        {
            var funcs = new List<AITool>();
            foreach (var sk in skills)
            {
                //构建绑定执行器的AIFunction，Name=skill.name、Description取自yaml描述
                var func = AIFunctionFactory.Create(
                    async ([Description("用户完整原始输入语句，完整原样传入用户的提问内容，不做截取、提取，整段原文填入")] string userRawInput) =>
                    {
                        return await skillExecuteHandler(sk.Name, userRawInput);
                    },
                    name: sk.Name,
                    description: sk.Description
                );
                funcs.Add(func);
            }
            return funcs;
        }

    }
}
