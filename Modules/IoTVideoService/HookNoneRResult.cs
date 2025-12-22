using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTVideoService
{
    public class HookNoneRResult : AjaxResult
    {
        public int code { get; set; }
        public bool close { get; set; }
        public HookNoneRResult() { }
        public HookNoneRResult(int code, bool close)
        {
            this.code = code;
            this.close = close;
        }
        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, MyDefaultTextJsonConfig.DefaultOptions);
        }
    }
}
