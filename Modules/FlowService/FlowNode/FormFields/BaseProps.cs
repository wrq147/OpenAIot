using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.FormFields
{
    public class BaseProps
    {
        /// <summary>
        /// 判断是否必需
        /// </summary>
        public bool required { get; set; }
        /// <summary>
        /// 是否允许打印
        /// </summary>
        public bool enablePrint { get; set; }
        public virtual void FindTo(FormField field, Func<FormField, bool> func, List<FormField> target)
        {
            if (func(field))
            {
                target.Add(field);
            }
        }
        public virtual bool Check(Dictionary<string, string> commitOperates, FormField field, Dictionary<string, object> model, Dictionary<string, string> inputParams, out string msg)
        {
            object val = null;
            model.TryGetValue(field.id, out val);
            msg = string.Empty;
            string opway;
            if (commitOperates.TryGetValue(field.id, out opway))
            {
                if (opway == "E")
                {
                    if (required && val == null)
                    {
                        msg = string.Format("{0}为必填项", field.title);
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 关联值处理
        /// </summary>
        /// <param name="field"></param>
        /// <param name="model"></param>
        public virtual void FieldRelated(FormField field, Dictionary<string, object> model) { }
    }
}
