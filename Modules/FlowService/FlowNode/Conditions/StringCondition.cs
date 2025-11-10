using AuthService;
using FlowService.FlowNode.Builder;
using FlowService.Model;
using System;
using System.Linq;

namespace FlowService.FlowNode.Conditions
{
    public class StringCondition : BaseCondition
    {
        public string[] value { get; set; }
        public string compare { get; set; }
        public override string ToExpressionString(BuilderContext context)
        {
            if (compare == "=")
            {
                string val = context.GetFormValue(this.id);
                if (val == null)
                {
                    string tmp = MyAccess.Core.Crypter.EncodeBase64(value[0], System.Text.Encoding.UTF8);
                    return "data.FormEq(\"" + this.id + "\",\"" + tmp + "\")";
                }
                else
                {
                    return val == value[0] ? "true" : "false";
                }
            }
            else if (compare == "!=")
            {
                string val = context.GetFormValue(this.id);
                if (val == null)
                {
                    return "!data.FormEq(\"" + this.id + "\",\"" + value[0].Replace("\"", "\\\"") + "\")";
                }
                else
                {
                    return val != value[0] ? "true" : "false";
                }
            }
            else if (compare == "IN")
            {
                string val = context.GetFormValue(this.id);
                if (val == null)
                {
                    string str = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                    return "data.FormIn(\"" + this.id + "\",\"" + str.Replace("\"", "\\\"") + "\")";
                }
                else
                {
                    return value.Contains(val) ? "true" : "false";
                }

            }
            return "false";
        }
    }
}
