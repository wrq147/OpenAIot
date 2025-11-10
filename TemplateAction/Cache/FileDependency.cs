using System;

namespace TemplateAction.Cache
{
    public class FileDependency : IDependency
    {
        bool _isNew;
        public FileDependency()
        {
            _isNew = false;
        }

        public bool Validate()
        {
            return _isNew;
        }
        public void NoticeChange()
        {
            _isNew = true;
        }
        public void Dispose(CacheItem item)
        {
        }

        public void Init(CachePool pool, string key)
        {
        }
    }
}
