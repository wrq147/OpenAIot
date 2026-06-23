using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TANetCoreHttpApplication _app;
        public TANetCoreHttpMiddleware(RequestDelegate next, TANetCoreHttpApplication app)
        {
            _next = next;
            _app = app;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.RequestServices = _app.GetServiceProvider();
            context.Features.Set<TANetCoreHttpApplication>(_app);
            await _next(context);
        }
    }
}
