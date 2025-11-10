using System;

namespace TemplateAction.Cache
{
    public interface IDependency
    {
        void Init(CachePool pool, string key);
        bool Validate();
        void Dispose(CacheItem item);
    }
}
