using Common.EventBus;
using Common.Share;
using Microsoft.Extensions.Options;
using MyAccess.DB.Builder.WhereToSql;
using NPOI.OpenXmlFormats.Dml.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService.Fields
{
    public static class FieldUtility
    {
        /// <summary>
        /// 获取关联对象的Id
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetObjectFieldId(string val)
        {
            int tidx = val.IndexOf(',');
            if (tidx != -1)
            {
                return val.Substring(0, tidx);
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取关联对象的名称
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetObjectFieldName(string val)
        {
            if (string.IsNullOrEmpty(val)) return string.Empty;
            int tidx = val.IndexOf(',');
            if (tidx != -1)
            {
                return val.Substring(tidx + 1);
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取扩展字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formobj"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static object GetExtVal<T>(T formobj, string field)
        {
            PropertyInfo prop = formobj.GetType().GetProperty(field);
            return prop.GetValue(formobj);
        }
        /// <summary>
        /// 设置扩展字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formobj"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static void SetExtVal<T>(T formobj, string field, object val)
        {
            PropertyInfo prop = formobj.GetType().GetProperty(field);
            prop.SetValue(formobj, val);
        }


        /// <summary>
        /// 检查表单新增的输入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="formobj"></param>
        /// <param name="orgId"></param>
        /// <param name="formName"></param>
        /// <param name="ac"></param>
        /// <param name="filterFields"></param>
        /// <returns></returns>
        public static async Task<BusResponse<string>> CheckAddForm<T>(ITAServiceProvider provider, T formobj, long orgId, string formName, TAAction ac, HashSet<string> filterFields = null)
        {
            var extfields = await provider.GetService<OrgBLL>().GetExtFormFields(orgId, formName);
            foreach (var ext in extfields)
            {
                if (filterFields != null)
                {
                    if (filterFields.Contains(ext.mapid))
                    {
                        continue;
                    }
                }
                var mapval = GetExtVal(formobj, ext.mapid);
                if (ext.is_required)
                {
                    if (mapval == null)
                    {
                        return BusResponse<string>.Error(211, $"{ext.name}为必填项");
                    }
                }
                if (ext.type == "图片")
                {
                    string val = (string)mapval;
                    if (val != null)
                    {
                        GeneralOption go = provider.GetService<IOptions<GeneralOption>>().Value;
                        if (!string.IsNullOrEmpty(go.minio_url))
                        {
                            val = val.Replace(go.minio_url, "");
                        }
                        if (!string.IsNullOrEmpty(go.url))
                        {
                            val = val.ToLower();
                            string tmpurl1 = go.url.ToLower();
                            tmpurl1 = tmpurl1.Replace("https:", "http:");
                            string tmpurl2 = tmpurl1.Replace("http:", "https:");
                            val = val.Replace(tmpurl1, string.Empty).Replace(tmpurl2, string.Empty);
                        }
                        SetExtVal(formobj, ext.mapid, val);
                    }
                }
                else if (ext.type == "时间")
                {
                    double? val = (double?)mapval;
                    if (val != null && val > 0)
                    {
                        string clientTZ = ac.Context.Request.Header["TZ"];
                        if (!string.IsNullOrEmpty(clientTZ))
                        {
                            if (int.TryParse(clientTZ, out int tz))
                            {
                                DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds((long)val).LocalDateTime;
                                dt = TimeZoneInfo.ConvertTimeFromUtc(dt.AddMinutes(tz), TimeZoneInfo.Local);
                                SetExtVal(formobj, ext.mapid, (double)new DateTimeOffset(dt).ToUnixTimeMilliseconds());
                            }
                        }
                    }
                }
            }
            return BusResponse<string>.Success();
        }


        /// <summary>
        /// 检查表单编辑的输入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="formobj"></param>
        /// <param name="orgId"></param>
        /// <param name="formName"></param>
        /// <param name="ac"></param>
        /// <param name="filterFields"></param>
        /// <returns></returns>
        public static async Task<BusResponse<int>> CheckEditForm<T>(ITAServiceProvider provider, T formobj, long orgId, string formName, TAAction ac, HashSet<string> filterFields = null)
        {
            var extfields = await provider.GetService<OrgBLL>().GetExtFormFields(orgId, formName);
            foreach (var ext in extfields)
            {
                if (filterFields != null)
                {
                    if (filterFields.Contains(ext.mapid))
                    {
                        continue;
                    }
                }
                var mapval = GetExtVal(formobj, ext.mapid);
                if (ext.type == "图片")
                {
                    string val = (string)mapval;
                    if (val != null)
                    {
                        GeneralOption go = provider.GetService<IOptions<GeneralOption>>().Value;
                        if (!string.IsNullOrEmpty(go.minio_url))
                        {
                            val = val.Replace(go.minio_url, "");
                        }
                        if (!string.IsNullOrEmpty(go.url))
                        {
                            val = val.ToLower();
                            string tmpurl1 = go.url.ToLower();
                            tmpurl1 = tmpurl1.Replace("https:", "http:");
                            string tmpurl2 = tmpurl1.Replace("http:", "https:");
                            val = val.Replace(tmpurl1, string.Empty).Replace(tmpurl2, string.Empty);
                        }
                        SetExtVal(formobj, ext.mapid, val);
                    }
                }
                else if (ext.type == "时间")
                {
                    double? val = (double?)mapval;
                    if (val != null && val > 0)
                    {
                        string clientTZ = ac.Context.Request.Header["TZ"];
                        if (!string.IsNullOrEmpty(clientTZ))
                        {
                            if (int.TryParse(clientTZ, out int tz))
                            {
                                DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds((long)val).LocalDateTime;
                                dt = TimeZoneInfo.ConvertTimeFromUtc(dt.AddMinutes(tz), TimeZoneInfo.Local);
                                SetExtVal(formobj, ext.mapid, (double)new DateTimeOffset(dt).ToUnixTimeMilliseconds());
                            }
                        }
                    }
                }
            }
            return BusResponse<int>.Success();
        }
        /// <summary>
        /// 生成表单的扩展信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="formobj"></param>
        /// <param name="orgId"></param>
        /// <param name="formName"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, object>> GenerateExtForm<T>(ITAServiceProvider provider, T formobj, long orgId, string formName, TAAction ac)
        {
            var extObjects = new Dictionary<string, object>();
            var extfields = await provider.GetService<OrgBLL>().GetExtFormFields(orgId, formName);
            foreach (var ext in extfields)
            {
                if (ext.type == "关联对象")
                {
                    var obj = (ObjectField)ext;
                    switch (obj.object_type)
                    {
                        case "用户":
                            {
                                var tmpid = (string)GetExtVal(formobj, obj.mapid);
                                if (!string.IsNullOrEmpty(tmpid))
                                {
                                    tmpid = GetObjectFieldId(tmpid);
                                    var tmp = await provider.GetService<UserDAL>().GetAdminById(Convert.ToInt64(tmpid));
                                    if (tmp != null)
                                    {
                                        extObjects.Add(obj.mapid, tmp);
                                    }
                                }
                            }
                            break;
                        case "部门":
                            {
                                var tmpid = (string)GetExtVal(formobj, obj.mapid);
                                if (!string.IsNullOrEmpty(tmpid))
                                {
                                    tmpid = GetObjectFieldId(tmpid);
                                    var tmp = await provider.GetService<DeptDAL>().SelectById(Convert.ToInt64(tmpid));
                                    if (tmp != null)
                                    {
                                        extObjects.Add(obj.mapid, tmp);
                                    }
                                }
                            }
                            break;
                        default:
                            {
                                var tmpid = (string)GetExtVal(formobj, obj.mapid);
                                if (!string.IsNullOrEmpty(tmpid))
                                {
                                    tmpid = GetObjectFieldId(tmpid);
                                    var rs = await BusUtility.Call("GetObject", new
                                    {
                                        id = tmpid,
                                        objtype = obj.object_type
                                    });
                                    if (rs != null)
                                    {
                                        var tmp = rs.GetResult<object>();
                                        if (tmp != null)
                                        {
                                            extObjects.Add(obj.mapid, tmp);
                                        }
                                    }
                                }

                            }
                            break;
                    }
                }
                else if (ext.type == "图片")
                {
                    string val = (string)GetExtVal(formobj, ext.mapid);
                    GeneralOption go = provider.GetService<IOptions<GeneralOption>>().Value;
                    if (string.IsNullOrEmpty(val))
                    {
                        var defaulturl = go.default_imgurl;
                        if (!string.IsNullOrEmpty(go.minio_bucket) && defaulturl.StartsWith("/" + go.minio_bucket))
                        {
                            val = go.minio_url + defaulturl;
                        }
                        else if (defaulturl.StartsWith("/"))
                        {
                            val = go.url + defaulturl;
                        }
                        else
                        {
                            val = defaulturl;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(go.minio_bucket) && val.StartsWith("/" + go.minio_bucket))
                        {
                            val = go.minio_url + val;
                        }
                        else if (val.StartsWith("/"))
                        {
                            val = go.url + val;
                        }
                    }

                    SetExtVal(formobj, ext.mapid, val);
                }
                else if (ext.type == "时间")
                {
                    double? val = (double?)GetExtVal(formobj, ext.mapid);
                    if (val != null && val > 0)
                    {
                        string clientTZ = ac.Context.Request.Header["TZ"];
                        if (!string.IsNullOrEmpty(clientTZ))
                        {
                            if (int.TryParse(clientTZ, out int tz))
                            {
                                DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds((long)val).LocalDateTime;
                                dt = TimeZoneInfo.ConvertTimeFromUtc(dt.AddMinutes(tz), TimeZoneInfo.Local);
                                SetExtVal(formobj, ext.mapid, (double)new DateTimeOffset(dt).ToUnixTimeMilliseconds());
                            }
                        }

                    }
                }
            }
            return extObjects;
        }
        public static async Task<BusResponse<PageObject<ObjectListItem>>> SearchObject(ITAServiceProvider provider, SearchPageParam query, Data_ServerTokenInfo user)
        {
            switch (query.objtype)
            {
                case "用户":
                    {
                        var scope = await user.GetScope(provider);
                        var userDAL = provider.GetService<UserDAL>();
                        var pageObj = await userDAL.SearchByKey(query.key, user.OrgId, query.pageNum, query.pageSize, scope);
                        PageObject<ObjectListItem> list = new PageObject<ObjectListItem>();
                        list.Total = pageObj.Total;
                        list.List = new List<ObjectListItem>();
                        foreach (var item in pageObj.List)
                        {
                            list.List.Add(new ObjectListItem()
                            {
                                Name = item.RealName + " - " + item.dept_name,
                                ValueName = item.RealName,
                                Value = item.Id.ToString(),
                                Obj = item
                            });
                        }
                        return BusResponse<PageObject<ObjectListItem>>.Success(list);
                    }
                case "部门":
                    {
                        var scope = await user.GetScope(provider);
                        var deptDAL = provider.GetService<DeptDAL>();
                        Expression<Func<MZ_Dept, bool>> expression = x => x.OrgId == user.OrgId;
                        if (!string.IsNullOrEmpty(query.key))
                        {
                            expression = expression.And(x => x.dept_name.Contains(query.key));
                        }

                        BaseQueryParam bqp = new BaseQueryParam();
                        bqp.pageNum = query.pageNum;
                        bqp.pageSize = query.pageSize;
                        bqp.showAll = true;
                        var pageObj = await deptDAL.SelectPage(expression, bqp, string.Empty);
                        PageObject<ObjectListItem> list = new PageObject<ObjectListItem>();
                        list.Total = pageObj.Total;
                        list.List = new List<ObjectListItem>();
                        foreach (var item in pageObj.List)
                        {
                            list.List.Add(new ObjectListItem()
                            {
                                Name = item.dept_name,
                                ValueName = item.dept_name,
                                Value = item.dept_id.ToString(),
                                Obj = item
                            });
                        }
                        return BusResponse<PageObject<ObjectListItem>>.Success(list);
                    }
                default:
                    var rs = await BusUtility.Call("SearchObject", new SearchObjectParam()
                    {
                        Query = query,
                        User = user
                    });
                    return rs.GetResult<BusResponse<PageObject<ObjectListItem>>>();
            }
        }
    }

}
