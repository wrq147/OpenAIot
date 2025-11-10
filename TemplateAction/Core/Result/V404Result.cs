
using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class V404Result : IResult
    {
        public Task Output(TAAction ac)
        {
            ac.Context.Response.StatusCode = 404;
            ac.Context.Response.StatusDescription = "Not Found";
            return Task.CompletedTask;
        }
    }
}
