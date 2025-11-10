using System;
using System.Reflection;
using TemplateAction.Common;

namespace TemplateAction.Core
{
    public static class TAObectCollectionExtensions
    {
        public static T Cast<T>(this ITAObjectCollection collection, string key, T def)
        {
            object result;
            if (collection.TryGet(key, out result))
            {
                return TAConverter.Cast(result, def);
            }
            else
            {
                return def;
            }
        }
        public static bool TryGet(this ITAObjectCollection collection, string key, Type t,out object result)
        {
            if (collection.TryGet(key, out result))
            {
                if (TAConverter.Instance.TryConvert(result, t, out result))
                {
                    return true;
                }
            }
            return false;
        }
       
    }
}
