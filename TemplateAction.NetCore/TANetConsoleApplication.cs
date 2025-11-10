using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetConsoleApplication: TAApplication
    {
        protected Microsoft.Extensions.DependencyInjection.IServiceCollection _services;
        public TANetConsoleApplication(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            _services = services;
        }
        protected override void BeforeInit()
        {
            Services.CopyServicesFrom(_services);
            base.BeforeInit();
        }
    }
}
