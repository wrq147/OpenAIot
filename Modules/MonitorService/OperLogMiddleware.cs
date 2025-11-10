using AuthService;
using Common;
using FluentMigrator.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;

namespace MonitorService
{
    public class OperLogMiddleware : IFilterMiddleware
    {
        public async Task<IResult> Excute(TAAction ac, FilterMiddlewareNode next)
        {
            var exeRes = await next.Excute(ac);
            bool hasDesc = ac.ActionNode.Method.HasAttribute<DesAttribute>();
            if (hasDesc)
            {
                //记录日志
                ac.Context.Application.ServiceProvider.GetService<OperLogThread>().PushLog(ac, ac.ActionNode.Descript, exeRes.ToString());
            }
            return exeRes;
        }
    }
}
