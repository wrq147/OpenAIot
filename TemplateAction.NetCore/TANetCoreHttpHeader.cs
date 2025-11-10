using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpHeader: NameValueCollection
    {
        private IHeaderDictionary _header;
        public TANetCoreHttpHeader(IHeaderDictionary header)
        {
            _header = header;
        }
        public override void Add(string name, string value)
        {
            _header.Add(name, new StringValues(value));
        }

        public override string[] AllKeys
        {
            get
            {
                string[] allKeys = new string[_header.Count];
                int i = 0;
                foreach(StringValues sv in _header.Values)
                {
                    allKeys[i] = sv.ToString();
                    i++;
                }
                return allKeys;
            }
        }
        public override void Clear()
        {
            _header.Clear();
        }
        public override int Count
        {
            get { return _header.Count; }
        }
        public override string Get(int index)
        {
            int i = 0;
            foreach(StringValues s in _header.Values)
            {
                if (i == index)
                {
                    return s.ToString();
                }
                i++;
            }
            return null;
        }
        public override string GetKey(int index)
        {
            int i = 0;
            foreach (string s in _header.Keys)
            {
                if (i == index)
                {
                    return s;
                }
                i++;
            }
            return null;
        }
        public override string Get(string name)
        {
            StringValues sv;
            if (_header.TryGetValue(name, out sv))
            {
                return sv.ToString();
            }
            else
            {
                return null;
            }
        }
        public override IEnumerator GetEnumerator()
        {
            foreach (StringValues val in _header.Values)
            {
                yield return val.ToString();
            }
        }
        public override void Set(string name, string value)
        {
            _header[name] = new StringValues(value);
        }
        public override string[] GetValues(int index)
        {
            int i = 0;
            foreach (StringValues s in _header.Values)
            {
                if (i == index)
                {
                    return s.ToArray();
                }
                i++;
            }
            return null;
        }
        public override string[] GetValues(string name)
        {
            StringValues sv;
            if (_header.TryGetValue(name, out sv))
            {
                return sv.ToArray();
            }
            else
            {
                return null;
            }
        }
        public override void Remove(string name)
        {
            _header.Remove(name);
        }
    }
}
