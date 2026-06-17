using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Tool
{
    public class SystemTime
    {
        public static AITool CreateSystemTimeTool(ITAServiceProvider provider)
        {
            return AIFunctionFactory.Create(
                () =>
                {
                    var currentTime = DateTime.Now;
                    // 格式化时间字符串，清晰易读
                    return currentTime.ToString("yyyy-MM-dd HH:mm:ss");
                },
                name: "查询当前时间",
                description: "根据服务器的时间，获取当前的系统时间"
            );
        }
    }
}
