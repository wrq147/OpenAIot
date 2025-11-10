using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetScopeServiceProvider : IServiceProvider
    {
        private ITAServiceProvider _services;
        private TANetCoreScope _scope;
        public TANetScopeServiceProvider(ITAServiceProvider services, TANetCoreScope scope)
        {
            _services = services;
            _scope = scope;
        }
        public object GetService(Type serviceType)
        {
            return _services.GetService(serviceType, _scope);
        }
    }
}
