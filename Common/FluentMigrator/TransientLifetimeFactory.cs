using System;
using TemplateAction.Core;

namespace Common.FluentMigrator
{
    public class TransientLifetimeFactory : ILifetimeFactory
    {
        public object GetValue(IInstanceFactory instanceFactory, Type serviceType, ServiceDescriptor sd)
        {
            return instanceFactory.CreateServiceInstance(serviceType, sd?.Factory, this);
        }
    }
}
