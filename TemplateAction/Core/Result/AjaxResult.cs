using TemplateAction.Label;
using System;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    /// <summary>
    /// ajax结果，进行ajax操作时使用
    /// </summary>
    public class AjaxResult : IResult
    {
        public async Task Output(TAAction ac)
        {
            ac.Context.Response.ContentType = "application/json";
            await ac.Context.Response.WriteAsync(ToString());
        }
    }
}
