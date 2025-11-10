using System;
namespace TemplateAction.Cache
{
    public class UnionOrDependency : IDependency
    {
        private IDependency _first;
        private IDependency _second;
        public UnionOrDependency(IDependency first, IDependency second)
        {
            _first = first;
            _second = second;
        }
        public void Dispose(CacheItem item)
        {
        }

        public void Init(CachePool pool, string key)
        {
        }

        public bool Validate()
        {
            return _first.Validate() || _second.Validate();
        }
    }
}
