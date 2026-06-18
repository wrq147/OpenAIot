using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Model
{
    public class T_ToolResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 工具输出结果
        /// </summary>
        public string Output { get; set; }
        /// <summary>
        /// 工具的调试信息
        /// </summary>
        public string LogInfo { get; set; }
    }
}
