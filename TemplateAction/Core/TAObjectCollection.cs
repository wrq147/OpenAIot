
using System;
using System.Collections;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public class TAObjectCollection : ITAObjectCollection
    {
        private Dictionary<string, object> _values;

        public int Count
        {
            get { return _values.Count; }
        }

        public TAObjectCollection()
        {
            _values = new Dictionary<string, object>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return _values.GetEnumerator();
        }
        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }
        public void Add(string key, object value)
        {
            _values[key] = value;
        }
        public object this[string key]
        {
            get
            {
                object rt;
                if (_values.TryGetValue(key, out rt))
                {
                    return rt;
                }
                return null;
            }
        }

        public bool TryGet(string key, out object result)
        {
            return _values.TryGetValue(key, out result);
        }

    }
}
