using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Label;
using TemplateAction.Label.Expression;

namespace TemplateAction.Core
{
    public class DefatulParamMapping : IParamMapping
    {
        /// <summary>
        /// 默认参数映射拦截
        /// </summary>
        public static Func<TAAction, Type, object, object> DefaultMappingResolver { get; set; }
        private static object GetArrayValueByKey(Type t, ITAObjectCollection collection, string key)
        {
            string[] sarr = null;
            string tlists = collection.Cast<string>(key + "[]", null);
            if (tlists == null)
            {
                object outstrs;
                if (collection.TryGet(key, out outstrs))
                {
                    sarr = outstrs as string[];
                    if (sarr == null)
                    {
                        tlists = TAConverter.Cast<string>(outstrs, null);
                    }
                }

            }
            if (sarr == null)
            {
                if (tlists != null)
                {
                    sarr = tlists.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    sarr = new string[0];
                }
            }


            if (t == typeof(int[]))
            {
                int[] arr = new int[sarr.Length];
                for (int i = 0; i < sarr.Length; i++)
                {
                    string s = sarr[i];
                    int ti = 0;
                    if (int.TryParse(s, out ti))
                    {
                        arr[i] = ti;
                    }
                    else
                    {
                        continue;
                    }
                }
                return arr;
            }
            else if (t == typeof(long[]))
            {
                long[] arr = new long[sarr.Length];
                for (int i = 0; i < sarr.Length; i++)
                {
                    string s = sarr[i];
                    long ti = 0;
                    if (long.TryParse(s, out ti))
                    {
                        arr[i] = ti;
                    }
                    else
                    {
                        continue;
                    }
                }

                return arr;
            }
            else if (t == typeof(decimal[]))
            {
                decimal[] arr = new decimal[sarr.Length];
                for (int i = 0; i < sarr.Length; i++)
                {
                    string s = sarr[i];
                    decimal ti = 0;
                    if (decimal.TryParse(s, out ti))
                    {
                        arr[i] = ti;
                    }
                    else
                    {
                        continue;
                    }
                }
                return arr;
            }
            else if (t == typeof(string[]))
            {
                return sarr;
            }
            return sarr;
        }
        internal static object TAObjectMapping(ITAObjectCollection collection, TAAction ac, string key, Type t)
        {
            if (collection.Count == 0)
            {
                if (ac.Context.Request.HasFormContentType == true || !string.IsNullOrEmpty(ac.Context.Request.ContentType))
                {
                    return DBNull.Value;
                }
            }
            if (t.IsArray)
            {
                object raars = GetArrayValueByKey(t, collection, key);
                if (DefaultMappingResolver != null)
                {
                    return DefaultMappingResolver(ac, t, raars);
                }
                else
                {
                    return raars;
                }
            }
            else if (t.IsValueType || t == typeof(string))
            {
                object result;
                if (collection.TryGet(key, t, out result))
                {
                    if (DefaultMappingResolver != null)
                    {
                        return DefaultMappingResolver(ac, t, result);
                    }
                    else
                    {
                        return result;
                    }

                }
            }
            else if (t.IsClass)
            {
                object result = null;
                PropertyInfo[] myallProinfos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
                foreach (PropertyInfo myProInfo in myallProinfos)
                {
                    if (myProInfo.GetIndexParameters().Length != 0) continue;
                    if (myProInfo.PropertyType.IsArray)
                    {
                        if (result == null)
                        {
                            result = Activator.CreateInstance(t);
                        }
                        var tmpeleType = myProInfo.PropertyType.GetElementType();
                        if (tmpeleType.IsClass && tmpeleType != typeof(string))
                        {
                            continue;
                        }
                        object raars = GetArrayValueByKey(myProInfo.PropertyType, collection, myProInfo.Name);
                        if (DefaultMappingResolver != null)
                        {
                            myProInfo.SetValue(result, DefaultMappingResolver(ac, myProInfo.PropertyType, raars), null);
                        }
                        else
                        {
                            myProInfo.SetValue(result, raars, null);
                        }
                    }
                    else
                    {
                        object tvalobj;
                        if (collection.TryGet(myProInfo.Name, myProInfo.PropertyType, out tvalobj))
                        {
                            if (result == null)
                            {
                                result = Activator.CreateInstance(t);
                            }
                            if (DefaultMappingResolver != null)
                            {
                                myProInfo.SetValue(result, DefaultMappingResolver(ac, myProInfo.PropertyType, tvalobj), null);
                            }
                            else
                            {
                                myProInfo.SetValue(result, tvalobj, null);
                            }
                        }
                    }

                }

                if (result == null)
                {
                    if (ac.Context.Request.HasFormContentType == true || !string.IsNullOrEmpty(ac.Context.Request.ContentType))
                    {
                        return DBNull.Value;
                    }
                    else
                    {
                        result = Activator.CreateInstance(t);
                    }
                }
                if (DefaultMappingResolver != null)
                {
                    return DefaultMappingResolver(ac, t, result);
                }
                else
                {
                    return result;
                }
            }

            return DBNull.Value;
        }
        /// <summary>
        /// 默认参数绑定
        /// </summary>
        /// <param name="next"></param>
        /// <param name="ac"></param>
        /// <param name="pi"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task<object> Mapping(LinkedListNode<IParamMapping> next, TAAction ac, string key, Type t)
        {
            ITARequest req = ac.Context.Request;
            object rt = DBNull.Value;
            if (req.Query != null)
            {
                rt = TAObjectMapping(req.Query, ac, key, t);
                if (rt != DBNull.Value)
                {
                    return rt;
                }
            }
            ITAFormCollection form = await req.ReadFormAsync();
            if (form != null)
            {
                rt = TAObjectMapping(form, ac, key, t);
                if (rt != DBNull.Value)
                {
                    return rt;
                }
            }
            if (ac.ExtParams != null)
            {
                rt = TAObjectMapping(ac.ExtParams, ac, key, t);
                if (rt != DBNull.Value)
                {
                    return rt;
                }
            }

            if (next == null)
            {
                return DBNull.Value;
            }
            return await next.Value.Mapping(next.Next, ac, key, t);
        }
    }
}
