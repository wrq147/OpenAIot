using IoTRulesService.Flow.Builder;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace IoTRulesService.Flow.Node.Conditions
{
    public class EnumCondition : BaseCondition
    {
        public object value { get; set; }
        public string compare { get; set; }
        public bool codeIsArrary { get; set; }
        /// <summary>
        /// 0为值、1为参数
        /// </summary>
        public int valueFrom { get; set; }
        public override async Task<bool> ToExpressionString(RuleExecutionContext context)
        {
            List<string> comparevals = new List<string>();
            if (valueFrom == 1)
            {
                object tmpval = await context.ReadSourceValue((string)value);
                if (tmpval is IList tmplist)
                {
                    foreach (var tmpitem in tmplist)
                    {
                        if (tmpitem != null)
                        {
                            comparevals.Add(tmpitem.ToString());
                        }
                    }
                }
                else if (tmpval != null)
                {
                    comparevals.Add(tmpval.ToString());
                }
            }
            else
            {
                if (codeIsArrary == true)
                {
                    if (value is JArray jvals)
                    {
                        comparevals.AddRange(jvals.Values<string>());
                    }
                    else if (value is IEnumerable<string> jvalstrs)
                    {
                        comparevals.AddRange(jvalstrs);
                    }
                }
                else
                {
                    comparevals.Add((string)value);
                }
            }
            for (int i = 0; i < comparevals.Count; i++)
            {
                if (comparevals[i] != null)
                {
                    comparevals[i] = comparevals[i].Trim();
                }
            }
            var tmpobj = await context.ReadSourceValue(this.code);
            string[] sourcevals = tmpobj as string[];
            if (sourcevals == null)
            {
                string sourceval = tmpobj == null ? string.Empty : tmpobj.ToString();
                sourcevals = sourceval.Split(',');
            }

            if (this.compare == "=")
            {
                foreach (string tmpval in comparevals)
                {
                    if (sourcevals.Contains(tmpval))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                foreach (string tmpval in comparevals)
                {
                    if (sourcevals.Contains(tmpval))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
