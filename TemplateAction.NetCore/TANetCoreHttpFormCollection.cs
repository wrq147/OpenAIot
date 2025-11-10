using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Reflection;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpFormCollection : ITAFormCollection
    {
        private IFormCollection _form;
        private IRequestFile[] _files;
        public TANetCoreHttpFormCollection(IFormCollection collection, IRequestFile[] files)
        {
            _form = collection;
            _files = files;
        }

        public object this[string key]
        {
            get
            {
                StringValues sv;
                if (_form.TryGetValue(key, out sv))
                {
                    return sv;
                }
                else
                {
                    return null;
                }
            }
        }

        public int Count
        {
            get { return _form.Count; }
        }

        public IRequestFile[] Files
        {
            get { return _files; }
        }

        public IEnumerator GetEnumerator()
        {
            yield return _form.GetEnumerator();
        }

        public bool TryGet(string key, out object result)
        {
            StringValues val;
            if (_form.TryGetValue(key, out val))
            {
                if (val.Count > 1)
                {
                    result = val.ToArray();
                }
                else
                {
                    result = val.ToString();
                }
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
