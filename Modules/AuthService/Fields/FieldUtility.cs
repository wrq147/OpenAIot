using AuthService.DAL;
using AuthService.Model;
using Common.EventBus;
using Common.Share;
using Microsoft.Extensions.Options;
using Minio.DataModel;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService.Fields
{
    public static class FieldUtility
    {
        public static void AppendFilter<A>(ISqlBuilder<A> sql, FieldFilterItem[] items, string rawId) where A : ISqlBuilder<A>
        {
            if (items == null || items.Length == 0)
            {
                return;
            }
            sql.Append($" and EXISTS(select 1 from mz_field_val where Id={rawId} and ");
            int kl = 0;
            foreach (var item in items)
            {
                if (kl == 0)
                {
                    sql.Append("(FieldId=").AppendParam(item.field);
                }
                else
                {
                    sql.Append(" or (FieldId=").AppendParam(item.field);
                }
                if (item.val_num != null)
                {
                    sql.Append(" and NumberValue");
                    //判断数字
                    switch (item.compare)
                    {
                        case "大于":
                            sql.Append(">");
                            break;
                        case "小于":
                            sql.Append("<");
                            break;
                        case "大于等于":
                            sql.Append(">=");
                            break;
                        case "小于等于":
                            sql.Append("<=");
                            break;
                        case "不等于":
                            sql.Append("<>");
                            break;
                        case "等于":
                            sql.Append("=");
                            break;
                    }
                    sql.AppendParam(item.val_num);
                }
                else if (item.val_arr != null && item.val_arr.Length > 0)
                {
                    sql.Append(" and Value");
                    //判断数组
                    switch (item.compare)
                    {
                        case "不等于":
                            sql.Append("<>");
                            break;
                        case "等于":
                            sql.Append("=");
                            break;
                        case "包含":
                            sql.Append(" in (");
                            break;
                        case "不包含":
                            sql.Append(" not in (");
                            break;
                    }
                    int i = 0;
                    foreach (var itemval in item.val_arr)
                    {
                        if (i == 0)
                        {
                            sql.AppendParam(itemval);
                        }
                        else
                        {
                            sql.Append(",").AppendParam(itemval);
                        }
                    }
                    if (item.compare == "包含" || item.compare == "不包含")
                    {
                        sql.Append(")");
                    }
                }
                else if (item.val != null)
                {
                    sql.Append(" and Value");
                    //判断字符串
                    switch (item.compare)
                    {
                        case "不等于":
                            sql.Append("<>").AppendParam(item.val);
                            break;
                        case "等于":
                            sql.Append("=").AppendParam(item.val);
                            break;
                        case "包含":
                            {
                                string tmpval = MyAccess.Core.StringTool.SqlLikeFilter(item.val);
                                sql.Append(" like ").AppendParam("%" + tmpval + "%");
                            }
                            break;
                        case "不包含":
                            {
                                string tmpval = MyAccess.Core.StringTool.SqlLikeFilter(item.val);
                                sql.Append(" not like ").AppendParam("%" + tmpval + "%");
                            }
                            break;
                        case "关联":
                            {
                                if (item.field.StartsWith("StrExt"))
                                {
                                    int didx = item.val.IndexOf(',');
                                    if (didx != -1)
                                    {
                                        sql.Append(" like ").AppendParam(item.val.Substring(0, didx + 1) + "%");
                                    }
                                }
                                else
                                {
                                    int didx = item.val.IndexOf(',');
                                    if (didx != -1)
                                    {
                                        sql.Append("=").AppendParam(item.val.Substring(0, didx + 1));
                                    }
                                    else if (item.val != "")
                                    {
                                        sql.Append("=").AppendParam(item.val);
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    sql.Append(" and 1=2");
                }
                sql.Append(")");
                kl++;
            }

            sql.Append(")");
        }
        public static Dictionary<string, object> FieldValToDict(List<MZ_FieldVal> list)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var item in list)
            {
                if (item.LongValue != null)
                {
                    dict.Add(item.FieldId, item.LongValue);
                }
                if (item.Value != null)
                {
                    dict.Add(item.FieldId, item.Value);
                }
                if (item.NumberValue != null)
                {
                    dict.Add(item.FieldId, item.NumberValue);
                }
            }
            return dict;
        }
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
        public static object GetExtVal<T>(T formobj, string field) where T : IFieldEntity
        {
            if (formobj.ExtVals.TryGetValue(field, out var val))
            {
                return val;
            }

            return null;
        }

        /// <summary>
        /// 设置扩展字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formobj"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        public static void SetExtVal<T>(T formobj, string field, object val) where T : IFieldEntity
        {
            if (formobj.ExtVals == null)
            {
                formobj.ExtVals = new Dictionary<string, object>();
            }
            formobj.ExtVals[field] = val;
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
        public static async Task<BusResponse<string>> CheckAddForm<T>(ITAServiceProvider provider, T formobj, long orgId, string formName, TAAction ac, HashSet<string> filterFields = null) where T : IFieldEntity
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
        public static async Task<BusResponse<int>> CheckEditForm<T>(ITAServiceProvider provider, T formobj, long orgId, string formName, TAAction ac, HashSet<string> filterFields = null) where T : IFieldEntity
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
        public static async Task DeleteFieldEntity<T>(ITAServiceProvider provider, T formobj) where T : IFieldEntity
        {
            var fieldValDAL = provider.GetService<FieldValDAL>();
            string formId = formobj.GetFormId();
            string formName = formobj.GetFormName();
            await fieldValDAL.Delete(x => x.Id == formId && x.TableName == formName);
        }
        public static async Task DeleteFieldEntityList<T>(ITAServiceProvider provider, List<T> formobjList) where T : IFieldEntity
        {
            if (formobjList.Count == 0)
            {
                return;
            }
            var fieldValDAL = provider.GetService<FieldValDAL>();
            List<string> formIds = formobjList.Select(x => x.GetFormId()).ToList();
            string formName = formobjList[0].GetFormName();
            await fieldValDAL.Delete(x => formIds.Contains(x.Id) && x.TableName == formName);
        }
        public static async Task UpdateFieldEntityList<T>(ITAServiceProvider provider, List<T> formobjList, long orgId) where T : IFieldEntity
        {
            if (formobjList.Count == 0)
            {
                return;
            }
            var fieldValDAL = provider.GetService<FieldValDAL>();
            List<string> formIds = formobjList.Select(x => x.GetFormId()).ToList();
            string formName = formobjList[0].GetFormName();
            await fieldValDAL.Delete(x => formIds.Contains(x.Id) && x.TableName == formName);

            var extObjects = new Dictionary<string, object>();
            var extfields = await provider.GetService<OrgBLL>().GetExtFormFields(orgId, formName);
            List<MZ_FieldVal> insertmodels = new List<MZ_FieldVal>();
            foreach (var ext in extfields)
            {
                foreach (var formobj in formobjList)
                {
                    if (formobj.ExtVals.TryGetValue(ext.mapid, out object tmpval))
                    {
                        MZ_FieldVal fieldVal = new MZ_FieldVal();
                        fieldVal.Id = formobj.GetFormId();
                        fieldVal.FieldId = ext.mapid;
                        insertmodels.Add(fieldVal);
                        if (ext.type == "数字" || ext.type == "时间")
                        {
                            fieldVal.NumberValue = Convert.ToDouble(tmpval);
                            continue;
                        }
                        else if (ext.type == "文本")
                        {
                            if (((TextField)ext).is_multiple)
                            {
                                fieldVal.LongValue = tmpval.ToString();
                                continue;
                            }
                        }
                        fieldVal.Value = tmpval.ToString();
                    }
                }

            }
            if (insertmodels.Count > 0)
            {
                await fieldValDAL.Insert(insertmodels);
            }
        }
        public static async Task UpdateFieldEntity<T>(ITAServiceProvider provider, T formobj, long orgId) where T : IFieldEntity
        {
            var fieldValDAL = provider.GetService<FieldValDAL>();
            string formId = formobj.GetFormId();
            string formName = formobj.GetFormName();
            await fieldValDAL.Delete(x => x.Id == formId && x.TableName == formName);

            var extObjects = new Dictionary<string, object>();
            var extfields = await provider.GetService<OrgBLL>().GetExtFormFields(orgId, formName);
            if (formobj.ExtVals == null)
            {
                return;
            }
            List<MZ_FieldVal> insertmodels = new List<MZ_FieldVal>();
            foreach (var ext in extfields)
            {
                if (formobj.ExtVals.TryGetValue(ext.mapid, out object tmpval))
                {
                    MZ_FieldVal fieldVal = new MZ_FieldVal();
                    fieldVal.Id = formobj.GetFormId();
                    fieldVal.FieldId = ext.mapid;
                    insertmodels.Add(fieldVal);
                    if (ext.type == "数字" || ext.type == "时间")
                    {
                        fieldVal.NumberValue = Convert.ToDouble(tmpval);
                        continue;
                    }
                    else if (ext.type == "文本")
                    {
                        if (((TextField)ext).is_multiple)
                        {
                            fieldVal.LongValue = tmpval.ToString();
                            continue;
                        }
                    }
                    fieldVal.Value = tmpval.ToString();

                }
            }
            if (insertmodels.Count > 0)
            {
                await fieldValDAL.Insert(insertmodels);
            }
        }

        /// <summary>
        /// 生成列表的扩展值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="formobjList"></param>
        /// <returns></returns>
        public static async Task GenerateExtValList<T>(ITAServiceProvider provider, List<T> formobjList) where T : IFieldEntity, new()
        {
            List<string> ids = formobjList.Select(x => x.GetFormId()).ToList();
            string formName = new T().GetFormName();
            var fieldvals = await provider.GetService<FieldValDAL>().SelectList(x => ids.Contains(x.Id) && x.TableName == formName);
            foreach (var formobj in formobjList)
            {
                var tmpfflist = fieldvals.Where(x => x.Id == formobj.GetFormId()).ToList();
                formobj.ExtVals = FieldValToDict(tmpfflist);
            }
        }
        /// <summary>
        /// 生成表单的扩展值
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="formobj"></param>
        /// <returns></returns>
        public static async Task GenerateExtVals<T>(ITAServiceProvider provider, T formobj) where T : IFieldEntity
        {
            string formId = formobj.GetFormId();
            string formName = formobj.GetFormName();
            var fieldvals = await provider.GetService<FieldValDAL>().SelectList(x => x.Id == formId && x.TableName == formName);
            formobj.ExtVals = FieldValToDict(fieldvals);
        }
        /// <summary>
        /// 生成表单的扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="formobj"></param>
        /// <param name="orgId"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public static async Task GenerateExtObject<T>(ITAServiceProvider provider, T formobj, long orgId, TAAction ac) where T : IFieldEntity
        {
            string formName = formobj.GetFormName();
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
                                    string tbname = formobj.GetFormName();
                                    var fieldvals = await provider.GetService<FieldValDAL>().SelectList(x => x.Id == tmpid && x.TableName == tbname);
                                    if (fieldvals != null && fieldvals.Count > 0)
                                    {
                                        extObjects.Add(obj.mapid, FieldValToDict(fieldvals));
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
            formobj.ExtObjects = extObjects;

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
