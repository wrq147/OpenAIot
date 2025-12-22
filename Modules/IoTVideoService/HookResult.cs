using Common.Json;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTVideoService
{
    public class HookResult : AjaxResult
    {
        /// <summary>
        /// 返回代码，为0为正确，1~9不提示，大于9提示
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string msg { get; set; }
        public HookResult() { }
        public HookResult(int code, string message)
        {
            this.code = code;
            this.msg = message;
        }
        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, MyDefaultTextJsonConfig.DefaultOptions);
        }
    }
}
