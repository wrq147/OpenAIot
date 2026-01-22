using Common.Json;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace FlowService.FlowNode.Builder
{
    public class BuilderContext
    {
        /// <summary>
        /// 所需部门
        /// </summary>
        public List<MZ_UserDept> UserDeptList { get; set; }
        /// <summary>
        /// 表单值
        /// </summary>
        public Dictionary<string, object> FormItems { get; set; }
        /// <summary>
        /// 输入参数
        /// </summary>
        public Dictionary<string, string> InputParams { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public virtual long Creator { get; set; }
        /// <summary>
        /// 执行者
        /// </summary>
        public virtual long Executor { get; set; }
        public FormField[] Fields { get; set; }
        public void FindTo(Func<FormField, bool> func, List<FormField> target)
        {
            foreach (FormField ff in Fields)
            {
                ff.FindTo(func, target);
            }
        }

        public void SetFormByTitle(string title, string val)
        {
            List<FormField> fields = new List<FormField>();
            FindTo(x => x.title == title, fields);
            if (fields.Count > 0)
            {
                FormItems[fields[0].id] = val;
            }
        }
        public object GetFormObject(string key)
        {
            //计算系统函数
            switch (key)
            {
                case "$now":
                    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                case "$true":
                    return "true";
                case "$executor":
                    return this.Executor;
                case "$initiator":
                    return this.Creator;
            }
            //获取传入参数
            if (key.StartsWith("@"))
            {
                string tmpval;
                if (!this.InputParams.TryGetValue(key, out tmpval))
                {
                    return key;
                }
                return tmpval;
            }
            object val;
            if (!FormItems.TryGetValue(key, out val)) return key;
            return val;
        }

        public string GetFormValue(string key)
        {
            var rt = GetFormObject(key);
            if (rt != null)
            {
                return Convert.ToString(rt);
            }
            return null;
        }
        public double GetFormDouble(string key)
        {
            return Convert.ToDouble(GetFormValue(key));
        }
        public List<long> GetUserFormById(string key)
        {
            List<long> uids = new List<long>();
            List<FormField> userField = new List<FormField>();
            foreach (var u in userField)
            {
                object outval;
                if (this.FormItems.TryGetValue(key, out outval))
                {
                    var selectedlist = outval as List<object>;
                    if (selectedlist != null)
                    {
                        foreach (var node in selectedlist)
                        {
                            var nodeObj = node as IDictionary<string, object>;
                            var jk = nodeObj["id"];
                            if (jk == null)
                            {
                                continue;
                            }
                            uids.Add(Convert.ToInt64(jk));
                        }
                    }

                }
            }
            return uids;
        }
        public bool FormEq(string key, string value)
        {
            var val = GetFormValue(key);
            if (val == null) return false;
            return val == value;
        }
        public bool FormIn(string key, string strlist)
        {
            var val = GetFormValue(key);
            if (val == null) return false;
            string[] strs = System.Text.Json.JsonSerializer.Deserialize<string[]>(strlist, MyDefaultTextJsonConfig.DefaultOptions);
            return strs.Contains(val);
        }

        public int CompareDate(string date1, string date2)
        {
            DateTime dt1 = Convert.ToDateTime(date1);
            DateTime dt2 = Convert.ToDateTime(date2);
            if (dt1 > dt2)
            {
                return 1;
            }
            else
            {
                if (dt1 == dt2)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
