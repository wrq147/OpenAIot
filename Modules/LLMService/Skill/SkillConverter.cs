using Microsoft.Extensions.AI;
using Mysqlx.Expr;
using System;
using System.Collections.Generic;
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
        public static List<AITool> BuildAIFunctions(this List<SkillMeta> skills, Func<string, AIFunctionArguments, Task<string>> skillExecuteHandler)
        {
            var funcs = new List<AITool>();
            foreach (var sk in skills)
            {
                //构建绑定执行器的AIFunction，Name=skill.name、Description取自yaml描述
                var func = AIFunctionFactory.Create(
                    async (FunctionInvocationContext context, CancellationToken ct) =>
                    {
                        string skillName = context.Function.Name;
                        return await skillExecuteHandler(skillName, context.Arguments);
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
