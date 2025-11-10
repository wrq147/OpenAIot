using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using TemplateAction.Core;
using ServiceDescriptor = TemplateAction.Core.ServiceDescriptor;

namespace TemplateAction.NetCore
{
    public class TANetCoreScope : IServiceScope, ILifetimeFactory
    {
        private ConcurrentDictionary<string, object> _dict = new ConcurrentDictionary<string, object>();
        public IServiceProvider ServiceProvider => _serviceProvider;
        private IServiceProvider _serviceProvider;
        public TANetCoreScope(ITAServiceProvider provider)
        {
            _serviceProvider = new TANetScopeServiceProvider(provider, this);
        }
        public void Dispose()
        {

        }

        public object GetValue(IInstanceFactory instanceFactory, Type serviceType, ServiceDescriptor sd)
        {
            string tkey = serviceType.FullName;
            if (sd != null)
            {
                if (sd.Factory != null)
                {
                    tkey += sd.GetHashCode().ToString();
                }
                return _dict.GetOrAdd(tkey, key => instanceFactory.CreateServiceInstance(serviceType, sd.Factory, this));
            }
            else
            {
                return _dict.GetOrAdd(tkey, key => instanceFactory.CreateServiceInstance(serviceType, null, this));
            }
        }
    }
}
