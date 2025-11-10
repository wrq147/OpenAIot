using System;

namespace TemplateAction.Core
{
    public class ConcurrentProxy
    {
        private volatile object _target;
        private object _lock = new object();
        internal ConcurrentProxy()
        {
        }
        public object GetValue(IInstanceFactory instanceFactory, Type serviceType, ProxyFactory factory, ILifetimeFactory scopeFactory)
        {
            if (_target == null)
            {
                lock (_lock)
                {
                    if (_target == null)
                    {
                        _target = instanceFactory.CreateServiceInstance(serviceType, factory, scopeFactory);
                    }
                }
            }
            return _target;
        }
    }
}
