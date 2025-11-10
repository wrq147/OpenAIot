using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public class QueryResult<T> : IDoResult<DbDataReader>
    {
        private List<string> _subMaps;
        private List<Type> _tMaps;
        private Dictionary<string, object> _subObjs;
        private List<T> mList;
        public void SetNavigate(List<string> sub, List<Type> tMaps)
        {
            _subMaps = sub;
            _tMaps = tMaps;
        }
        public int Count
        {
            get
            {
                return mList.Count;
            }
        }
        /// <summary>
        /// 获取返回模型列表的第一个，没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToFirst()
        {
            if (mList.Count <= 0) return default(T);
            return mList[0];
        }
        public T ToFirstOrDefault(T def)
        {
            if (mList.Count <= 0) return def;
            return mList[0];
        }

        public T[] ToArray()
        {
            return mList.ToArray();
        }

        public List<T> ToList()
        {
            return mList;
        }
        public QueryResult()
        {
            mList = new List<T>();
        }

        private Dictionary<string, PropertyInfo> _mapColumn = new Dictionary<string, PropertyInfo>();
        private PropertyInfo GetMapColumn(string classname, string propertyname)
        {
            string tkey = classname + "@" + propertyname;
            if (_mapColumn.TryGetValue(tkey, out PropertyInfo outproperty))
            {
                return outproperty;
            }
            return null;
        }
        private void InitMapProperty(Type t)
        {
            if (_mapColumn.ContainsKey(t.Name))
            {
                return;
            }
            _mapColumn.Add(t.Name, null);
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                ColumnByAttribute tmpcolumn = property.GetCustomAttribute<ColumnByAttribute>();
                if (tmpcolumn == null)
                {
                    _mapColumn.Add(t.Name + "@" + property.Name, property);
                }
                else
                {
                    string tClassName = tmpcolumn.ClassName;
                    string tPropName = tmpcolumn.ColumnName;
                    if (string.IsNullOrEmpty(tClassName))
                    {
                        tClassName = t.Name;
                    }
                    if (string.IsNullOrEmpty(tPropName))
                    {
                        tPropName = property.Name;
                    }
                    _mapColumn.Add(tClassName + "@" + tPropName, property);
                }
            }
        }
        private T GetAssigned(DbDataReader dr)
        {
            Type temptype = typeof(T);
            if (temptype.IsInterface)
            {
                throw new Exception("数据库转实体不能为接口");
            }
            bool isvaltmp = !temptype.IsClass || temptype == typeof(string);
            if (isvaltmp)
            {
                string mapval = DBMapping.TryGetValue(temptype);
                if (mapval != null)
                {
                    switch (mapval)
                    {
                        case "DateTime":
                            return (T)(object)dr.GetDateTime(0);
                        case "decimal":
                            return (T)(object)dr.GetDecimal(0);
                        case "string":
                            return (T)(object)dr.GetString(0);
                        case "Int32":
                            return (T)(object)dr.GetInt32(0);
                        case "Int64":
                            return (T)(object)dr.GetInt64(0);
                        case "bool":
                            return (T)(object)dr.GetBoolean(0);
                        case "byte":
                            return (T)(object)dr.GetByte(0);
                        case "char":
                            return (T)(object)dr.GetChar(0);
                        case "short":
                            return (T)(object)dr.GetInt16(0);
                        case "double":
                            return (T)(object)dr.GetDouble(0);
                        case "float":
                            return (T)(object)dr.GetFloat(0);
                    }
                }
                return dr.GetFieldValue<T>(0);
            }
            else
            {
                T model = TypeBuilder.CreateInstance<T>();
                Type targT = model.GetType();
                InitMapProperty(targT);
                //一对一绑定
                if (_subMaps != null)
                {
                    _subObjs = new Dictionary<string, object>();
                    for (int i = 0; i < _subMaps.Count; i++)
                    {
                        if (_subMaps[i] == SqlBuilder.NullSub)
                        {
                            continue;
                        }
                        string s = _subMaps[i];
                        PropertyInfo propertyInfo = targT.GetProperty(s);
                        InitMapProperty(propertyInfo.PropertyType);
                        var tmpObj = Activator.CreateInstance(propertyInfo.PropertyType);
                        propertyInfo.SetValue(model, tmpObj, null);
                        _subObjs.Add(DBMapping.GetSubPrefix(i), tmpObj);
                    }
                }

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (dr.IsDBNull(i))
                    {
                        continue;
                    }
                    string fieldname = dr.GetName(i);
                    int prefix = -1;
                    if (_subMaps != null)
                    {
                        prefix = fieldname.IndexOf('.');
                    }
                    PropertyInfo propertyInfo = null;
                    object target = model;
                    if (prefix == -1)
                    {
                        propertyInfo = GetMapColumn(targT.Name, fieldname);
                        if (propertyInfo == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        string prefixstr = fieldname.Substring(0, prefix);
                        string propname = fieldname.Substring(prefix + 1);
                        if (!_subObjs.TryGetValue(prefixstr, out target))
                        {
                            int prefixIdx = DBMapping.GetIndexByPrefix(prefixstr);
                            propertyInfo = GetMapColumn(_tMaps[prefixIdx].Name, propname);
                            if (propertyInfo == null)
                            {
                                continue;
                            }
                            target = model;
                        }
                        else
                        {
                            propertyInfo = GetMapColumn(target.GetType().Name, propname);
                            if (propertyInfo == null)
                            {
                                continue;
                            }
                        }
                    }



                    string mapval = DBMapping.TryGetValue(propertyInfo.PropertyType);
                    if (mapval != null)
                    {
                        switch (mapval)
                        {
                            case "DateTime":
                                propertyInfo.SetValue(target, dr.GetDateTime(i), null);
                                break;
                            case "decimal":
                                propertyInfo.SetValue(target, dr.GetDecimal(i), null);
                                break;
                            case "string":
                                propertyInfo.SetValue(target, dr.GetString(i), null);
                                break;
                            case "Int32":
                                propertyInfo.SetValue(target, dr.GetInt32(i), null);
                                break;
                            case "Int64":
                                propertyInfo.SetValue(target, dr.GetInt64(i), null);
                                break;
                            case "bool":
                                propertyInfo.SetValue(target, dr.GetBoolean(i), null);
                                break;
                            case "byte":
                                propertyInfo.SetValue(target, dr.GetByte(i), null);
                                break;
                            case "char":
                                propertyInfo.SetValue(target, dr.GetChar(i), null);
                                break;
                            case "short":
                                propertyInfo.SetValue(target, dr.GetInt16(i), null);
                                break;
                            case "double":
                                propertyInfo.SetValue(target, dr.GetDouble(i), null);
                                break;
                            case "float":
                                propertyInfo.SetValue(target, dr.GetFloat(i), null);
                                break;
                            default:
                                propertyInfo.SetValue(target, dr.GetValue(i), null);
                                break;
                        }
                    }
                    else
                    {
                        //枚举赋值
                        propertyInfo.SetValue(target, dr.GetValue(i), null);
                    }

                }
                return model;
            }
        }

        private async Task<T> GetAssignedAsync(DbDataReader dr)
        {
            Type temptype = typeof(T);
            if (temptype.IsInterface)
            {
                throw new Exception("数据库转实体不能为接口");
            }
            bool isvaltmp = !temptype.IsClass || temptype == typeof(string);
            if (isvaltmp)
            {
                string mapval = DBMapping.TryGetValue(temptype);
                if (mapval != null)
                {
                    switch (mapval)
                    {
                        case "DateTime":
                            return (T)(object)dr.GetDateTime(0);
                        case "decimal":
                            return (T)(object)dr.GetDecimal(0);
                        case "string":
                            return (T)(object)dr.GetString(0);
                        case "Int32":
                            return (T)(object)dr.GetInt32(0);
                        case "Int64":
                            return (T)(object)dr.GetInt64(0);
                        case "bool":
                            return (T)(object)dr.GetBoolean(0);
                        case "byte":
                            return (T)(object)dr.GetByte(0);
                        case "char":
                            return (T)(object)dr.GetChar(0);
                        case "short":
                            return (T)(object)dr.GetInt16(0);
                        case "double":
                            return (T)(object)dr.GetDouble(0);
                        case "float":
                            return (T)(object)dr.GetFloat(0);
                    }
                }
                return dr.GetFieldValue<T>(0);
            }
            else
            {
                T model = TypeBuilder.CreateInstance<T>();
                Type targT = model.GetType();
                InitMapProperty(targT);
                //一对一绑定
                if (_subMaps != null)
                {
                    _subObjs = new Dictionary<string, object>();
                    for (int i = 0; i < _subMaps.Count; i++)
                    {
                        if (_subMaps[i] == SqlBuilder.NullSub)
                        {
                            continue;
                        }
                        string s = _subMaps[i];
                        PropertyInfo propertyInfo = targT.GetProperty(s);
                        InitMapProperty(propertyInfo.PropertyType);
                        var tmpObj = Activator.CreateInstance(propertyInfo.PropertyType);
                        propertyInfo.SetValue(model, tmpObj, null);
                        _subObjs.Add(DBMapping.GetSubPrefix(i), tmpObj);
                    }
                }

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (await dr.IsDBNullAsync(i))
                    {
                        continue;
                    }
                    string fieldname = dr.GetName(i);
                    int prefix = -1;
                    if (_subMaps != null)
                    {
                        prefix = fieldname.IndexOf('.');
                    }
                    PropertyInfo propertyInfo = null;
                    object target = model;
                    if (prefix == -1)
                    {
                        propertyInfo = GetMapColumn(targT.Name, fieldname);
                        if (propertyInfo == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        string prefixstr = fieldname.Substring(0, prefix);
                        string propname = fieldname.Substring(prefix + 1);
                        if (!_subObjs.TryGetValue(prefixstr, out target))
                        {
                            int prefixIdx = DBMapping.GetIndexByPrefix(prefixstr);
                            propertyInfo = GetMapColumn(_tMaps[prefixIdx].Name, propname);
                            if (propertyInfo == null)
                            {
                                continue;
                            }
                            target = model;
                        }
                        else
                        {
                            propertyInfo = GetMapColumn(target.GetType().Name, propname);
                            if (propertyInfo == null)
                            {
                                continue;
                            }
                        }

                    }
                    string mapval = DBMapping.TryGetValue(propertyInfo.PropertyType);
                    if (mapval != null)
                    {
                        switch (mapval)
                        {
                            case "DateTime":
                                propertyInfo.SetValue(target, dr.GetDateTime(i), null);
                                break;
                            case "decimal":
                                propertyInfo.SetValue(target, dr.GetDecimal(i), null);
                                break;
                            case "string":
                                propertyInfo.SetValue(target, dr.GetString(i), null);
                                break;
                            case "Int32":
                                propertyInfo.SetValue(target, dr.GetInt32(i), null);
                                break;
                            case "Int64":
                                propertyInfo.SetValue(target, dr.GetInt64(i), null);
                                break;
                            case "bool":
                                propertyInfo.SetValue(target, dr.GetBoolean(i), null);
                                break;
                            case "byte":
                                propertyInfo.SetValue(target, dr.GetByte(i), null);
                                break;
                            case "char":
                                propertyInfo.SetValue(target, dr.GetChar(i), null);
                                break;
                            case "short":
                                propertyInfo.SetValue(target, dr.GetInt16(i), null);
                                break;
                            case "double":
                                propertyInfo.SetValue(target, dr.GetDouble(i), null);
                                break;
                            case "float":
                                propertyInfo.SetValue(target, dr.GetFloat(i), null);
                                break;
                            default:
                                propertyInfo.SetValue(target, dr.GetValue(i), null);
                                break;
                        }
                    }
                    else
                    {
                        //枚举赋值
                        propertyInfo.SetValue(target, dr.GetValue(i), null);
                    }
                }


                return model;
            }
        }
        public void SetResult(DbDataReader result)
        {
            mList.Add(GetAssigned(result));
        }
        public async Task SetResultAsync(DbDataReader result)
        {
            mList.Add(await GetAssignedAsync(result));
        }
    }
}
