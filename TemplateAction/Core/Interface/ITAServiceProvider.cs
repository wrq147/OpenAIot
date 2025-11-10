using System;

namespace TemplateAction.Core
{
    public interface ITAServiceProvider
    {
        object CreateScopeService(ILifetimeFactory scopeFactory, Type serviceType);
        object GetService(string key, ILifetimeFactory scopeFactory = null);
        object GetService(Type tp, ILifetimeFactory scopeFactory = null);
        bool ExistService(string key);
        void ClearService(string key);
    }
}
