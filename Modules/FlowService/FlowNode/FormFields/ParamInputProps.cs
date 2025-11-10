using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.FormFields
{
    public class ParamInputProps : BaseProps
    {
        /// <summary>
        /// 关联的表单类型
        /// </summary>
        public string formType { get; set; }
        /// <summary>
        /// 关联的表单Id值
        /// </summary>
        public string formId { get; set; }
        public override bool Check(Dictionary<string, string> commitOperates, FormField field, Dictionary<string, object> model, Dictionary<string, string> inputParams, out string msg)
        {
            if (!base.Check(commitOperates, field, model, inputParams, out msg))
            {
                return false;
            }
            if (this.formType == "")
            {
                if (!inputParams.ContainsKey("@from"))
                {
                    msg = string.Format("流程表单的{0}类型是‘发起的工单’，所以无法直接发起", field.title);
                    return false;
                }
            }
            return true;
        }
    }
}
