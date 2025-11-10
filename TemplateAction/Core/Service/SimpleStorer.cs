using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    /// <summary>
    /// 单线程服务实例存储器
    /// </summary>
    public class SimpleStorer
    {
        private Dictionary<string, object> _instances;
        public SimpleStorer()
        {
            _instances = new Dictionary<string, object>();
        }
        /// <summary>
        /// 添加服务实例
        /// </summary>
        /// <param name="t"></param>
        /// <param name="value"></param>
        public void AddInstance(string key, object value)
        {
            _instances[key] = value;
        }
        public bool TryGetValue(string key, out object result)
        {
            return _instances.TryGetValue(key, out result);
        }
        public object GetInstance(string key)
        {
            object rt;
            if (_instances.TryGetValue(key, out rt))
            {
                return rt;
            }
            return null;
        }
        public T GetInstance<T>()
        {
            try
            {
                return (T)GetInstance(typeof(T).FullName);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
