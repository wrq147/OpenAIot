using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Reflection;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpQueryCollection : ITAObjectCollection
    {
        private IQueryCollection _query;
        public TANetCoreHttpQueryCollection(IQueryCollection collection)
        {
            _query = collection;
        }
        public object this[string key]
        {
            get
            {
                StringValues sv;
                if(_query.TryGetValue(key,out sv))
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
            get { return _query.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            yield return _query.GetEnumerator();
        }
 
        public bool TryGet(string key, out object result)
        {
            StringValues val;
            if (_query.TryGetValue(key, out val))
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
