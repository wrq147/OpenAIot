using Microsoft.Extensions.DependencyInjection;
using System;
using TemplateAction.Core;
namespace TemplateAction.NetCore
{
    public class TANetServiceProvider : IServiceProvider, IServiceProviderIsService
    {
        private ITAServiceProvider _services;
        public TANetServiceProvider(ITAServiceProvider services)
        {
            _services = services;
        }
        public object GetService(Type serviceType)
        {
            return _services.GetService(serviceType);
        }

        public bool IsService(Type serviceType)
        {
            return _services.ExistService(serviceType.FullName);
        }
    }
}
