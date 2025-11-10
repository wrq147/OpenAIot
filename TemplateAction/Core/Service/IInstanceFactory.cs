using System;

namespace TemplateAction.Core
{
    public interface IInstanceFactory
    {
        object CreateServiceInstance(Type serviceType, ProxyFactory factory, ILifetimeFactory scopeFactory);
    }
}
