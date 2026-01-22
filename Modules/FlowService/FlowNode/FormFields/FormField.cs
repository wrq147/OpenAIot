using Common.Json;
using Common.Share;
using FlowService.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TemplateAction.Core;

namespace FlowService.FlowNode.FormFields
{
    public class FormField
    {
        /// <summary>
        /// 表单字段ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 表单字段标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 表单字段类型
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 值类型
        /// </summary>
        public string valueType { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public BaseProps props { get; set; }

        public static Dictionary<string, FormField> ToFieldDict(FormField[] Fields)
        {
            List<FormField> target = new List<FormField>();
            foreach (FormField ff in Fields)
            {
                ff.FindTo(x => true, target);
            }
            Dictionary<string, FormField> formDict = new Dictionary<string, FormField>();
            foreach (FormField ff in target)
            {
                formDict.Add(ff.id, ff);
            }
            return formDict;
        }
        public void FindTo(Func<FormField, bool> func, List<FormField> target)
        {
            props.FindTo(this, func, target);
        }
        public bool Check(Dictionary<string, string> commitOperates, Dictionary<string, object> model, Dictionary<string, string> inputParams, out string msg)
        {
            return props.Check(commitOperates, this, model, inputParams, out msg);
        }
        public void FieldRelated(Dictionary<string, object> model)
        {
            props.FieldRelated(this, model);
        }

        /// <summary>
        /// 转换为存储值
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="val"></param>
        /// <param name="globalServiceProvider"></param>
        /// <returns></returns>
        public virtual MZ_FormDataItem[] ToSaveItems(long flowId, object val, ITAServiceProvider globalServiceProvider)
        {
            switch (this.name)
            {
                case "NumberInput":
                    {
                        MZ_FormDataItem tmpfdi = new MZ_FormDataItem();
                        tmpfdi.FlowId = flowId;
                        tmpfdi.FieldId = this.id;
                        tmpfdi.NumberValue = Convert.ToDouble(val);
                        return new MZ_FormDataItem[] { tmpfdi };
                    }
                case "DevicPicker":
                case "UserPicker":
                case "DeptPicker":
                    {
                        var selectedlist = val as IEnumerable<object>;
                        if (selectedlist == null)
                        {
                            return Array.Empty<MZ_FormDataItem>();
                        }
                        List<MZ_FormDataItem> rt = new List<MZ_FormDataItem>();
                        MZ_FormDataItem detail = new MZ_FormDataItem();
                        detail.FlowId = flowId;
                        detail.FieldId = this.id;
                        detail.Value = System.Text.Json.JsonSerializer.Serialize(val, MyDefaultTextJsonConfig.DefaultOptions);
                        rt.Add(detail);
                        int i = 1;
                        foreach (var node in selectedlist)
                        {
                            var dictobj = node as IDictionary<string, object>;
                            var jk = dictobj["id"];
                            if (jk == null)
                            {
                                continue;
                            }
                            MZ_FormDataItem tmpfdi = new MZ_FormDataItem();
                            tmpfdi.FlowId = flowId;
                            tmpfdi.FieldId = this.id + "@v" + i;
                            if (this.name == "DevicPicker")
                            {
                                tmpfdi.Value = Convert.ToString(jk);
                            }
                            else
                            {
                                tmpfdi.NumberValue = Convert.ToInt64(jk);
                            }
                            rt.Add(tmpfdi);
                            ++i;
                        }
                        return rt.ToArray();
                    }
                case "TableList":
                    {
                        var selectedlist = val as IEnumerable<object>;
                        if (selectedlist == null)
                        {
                            return Array.Empty<MZ_FormDataItem>();
                        }
                        List<MZ_FormDataItem> rt = new List<MZ_FormDataItem>();
                        MZ_FormDataItem detail = new MZ_FormDataItem();
                        detail.FlowId = flowId;
                        detail.FieldId = this.id;
                        detail.LongValue = System.Text.Json.JsonSerializer.Serialize(val, MyDefaultTextJsonConfig.DefaultOptions);
                        rt.Add(detail);
                        int i = 1;
                        string[] idxFields = ((TableListProps)props).IdxColName;
                        if (idxFields != null && idxFields.Length > 0)
                        {
                            FormField[] fields = ((TableListProps)props).columns;
                            Dictionary<string, FormField> fieldDict = new Dictionary<string, FormField>();
                            foreach (var f in fields)
                            {
                                fieldDict.Add(f.id, f);
                            }
                            foreach (var row in selectedlist)
                            {
                                var rowObj = row as IDictionary<string, object>;
                                foreach (string rowfield in idxFields)
                                {

                                    MZ_FormDataItem tmpfdi = new MZ_FormDataItem();
                                    tmpfdi.FlowId = flowId;
                                    tmpfdi.FieldId = this.id + "@" + rowfield + "@v" + i;
                                    if (fieldDict[rowfield].name == "TextInput")
                                    {
                                        tmpfdi.Value = System.Text.Json.JsonSerializer.Serialize(rowObj[rowfield], MyDefaultTextJsonConfig.DefaultOptions);
                                    }
                                    else
                                    {
                                        tmpfdi.NumberValue = Convert.ToDouble(rowObj[rowfield]);
                                    }
                                    rt.Add(tmpfdi);
                                }
                                ++i;
                            }
                        }

                        return rt.ToArray();
                    }
                case "ImageUpload":
                case "FileUpload":
                    {
                        MZ_FormDataItem tmpfdi = new MZ_FormDataItem();
                        tmpfdi.FlowId = flowId;
                        tmpfdi.FieldId = this.id;
                        if (val != null)
                        {
                            var selectedlist = val as IEnumerable<object>;
                            if (selectedlist == null)
                            {
                                return Array.Empty<MZ_FormDataItem>();
                            }
                            foreach (var jval in selectedlist)
                            {
                                var rowObj = jval as IDictionary<string, object>;
                                string tmpval = Convert.ToString(rowObj["url"]);
                                GeneralOption go = globalServiceProvider.GetService<IOptions<GeneralOption>>().Value;

                                if (!string.IsNullOrEmpty(go.minio_url))
                                {
                                    tmpval = tmpval.Replace(go.minio_url, string.Empty);
                                }
                                if (!string.IsNullOrEmpty(go.url))
                                {
                                    tmpval = tmpval.ToLower();
                                    string tmpurl1 = go.url.ToLower();
                                    tmpurl1 = tmpurl1.Replace("https:", "http:");
                                    string tmpurl2 = tmpurl1.Replace("http:", "https:");
                                    tmpval = tmpval.Replace(tmpurl1, string.Empty).Replace(tmpurl2, string.Empty);
                                }
                                rowObj["url"] = tmpval;
                            }

                            tmpfdi.Value = System.Text.Json.JsonSerializer.Serialize(selectedlist, MyDefaultTextJsonConfig.DefaultOptions);
                        }
                        else
                        {
                            tmpfdi.Value = string.Empty;
                        }
                        return new MZ_FormDataItem[] { tmpfdi };
                    }
                default:
                    {
                        MZ_FormDataItem tmpfdi = new MZ_FormDataItem();
                        tmpfdi.FlowId = flowId;
                        tmpfdi.FieldId = this.id;
                        tmpfdi.Value = System.Text.Json.JsonSerializer.Serialize(val, MyDefaultTextJsonConfig.DefaultOptions);
                        return new MZ_FormDataItem[] { tmpfdi };
                    }
            }
        }
        /// <summary>
        /// 转换为显示值
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual object FromSave(MZ_FormDataItem item)
        {
            switch (this.name)
            {
                case "TableList":
                    return System.Text.Json.JsonSerializer.Deserialize<object>(item.LongValue, MyDefaultTextJsonConfig.DefaultOptions);
                case "NumberInput":
                    return item.NumberValue;
                case "ImageUpload":
                case "FileUpload":
                    {
                        TAAction ac = TAAction.Current;
                        if (ac == null)
                        {
                            return item.Value;
                        }
                        string val = item.Value;
                        if (string.IsNullOrEmpty(val))
                        {
                            return null;
                        }

                        ITAServiceProvider globalServiceProvider = ac.Context.Application.ServiceProvider;
                        GeneralOption go = globalServiceProvider.GetService<IOptions<GeneralOption>>().Value;
                        var jarr = System.Text.Json.JsonSerializer.Deserialize<List<object>>(val, MyDefaultTextJsonConfig.DefaultOptions);
                        foreach (var jobj in jarr)
                        {
                            var jdict = jobj as IDictionary<string, object>;
                            string valstr = Convert.ToString(jdict["url"]);
                            if (!string.IsNullOrEmpty(valstr))
                            {
                                if (!string.IsNullOrEmpty(go.minio_bucket) && valstr.StartsWith("/" + go.minio_bucket))
                                {
                                    valstr = go.minio_url + valstr;
                                }
                                else if (valstr.StartsWith("/"))
                                {
                                    valstr = go.url + valstr;
                                }
                            }
                            else
                            {
                                if (this.name == "ImageUpload")
                                {
                                    valstr = go.default_imgurl;
                                }
                            }
                            jdict["url"] = valstr;
                        }
                        return jarr;
                    }
                default:
                    {
                        return System.Text.Json.JsonSerializer.Deserialize<object>(item.Value, MyDefaultTextJsonConfig.DefaultOptions);
                    }
            }
        }
    }

}
