using IoTRulesService.Flow.Builder;
using IoTRulesService.Flow.Node.Conditions;
using Jint;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    public class ConditionNode : RuleBaseNode
    {
        public ConditionsProps props { get; set; }
    }


    public class ConditionsProps
    {
        /// <summary>
        /// 自定义条件解释脚本，灵活构建逻辑关系
        /// </summary>
        public string expression { get; set; }
        /// <summary>
        /// 条件组逻辑关系 OR、AND
        /// </summary>
        public string groupsType { get; set; }
        public ConditionGroup[] groups { get; set; }
        public async Task<bool> ToExpressionString(RuleExecutionContext context)
        {

            if (!string.IsNullOrEmpty(expression))
            {
                string[] groupNames = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
                Dictionary<string, ConditionGroup> dic = new Dictionary<string, ConditionGroup>();
                for (int i = 0; i < groups.Length; i++)
                {
                    dic.Add(groupNames[i], groups[i]);
                }
                StringBuilder sb = new StringBuilder();
                foreach (char c in expression)
                {
                    if (char.IsLetter(c))
                    {
                        bool tmprs = await dic[c.ToString()].ToExpressionString(context);

                        sb.Append(tmprs ? "true" : "false");
                    }
                    else if (c == '&')
                    {
                        sb.Append("&&");
                    }
                    else if (c == '|')
                    {
                        sb.Append("||");
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                var res = new Engine()
                   .Evaluate(sb.ToString());
                return res.AsBoolean();
            }
            else
            {
                bool istrue = false;
                for (int i = 0; i < groups.Length; i++)
                {
                    if (i == 0)
                    {
                        istrue = await groups[i].ToExpressionString(context);
                    }
                    else
                    {
                        if (groupsType == "OR")
                        {
                            istrue = istrue || (await groups[i].ToExpressionString(context));
                        }
                        else
                        {
                            istrue = istrue && (await groups[i].ToExpressionString(context));
                        }
                    }
                }
                return istrue;
            }
        }
    }

    public class ConditionGroup
    {
        /// <summary>
        /// 条件逻辑关系 OR、AND
        /// </summary>
        public string groupType { get; set; }
        public string[] cids { get; set; }
        public BaseCondition[] conditions { get; set; }
        public async Task<bool> ToExpressionString(RuleExecutionContext context)
        {
            bool isrr = false;
            for (int i = 0; i < conditions.Length; i++)
            {
                if (i == 0)
                {
                    isrr = await conditions[i].ToExpressionString(context);
                }
                else
                {
                    if (groupType == "OR")
                    {
                        isrr = isrr || (await conditions[i].ToExpressionString(context));
                    }
                    else
                    {
                        isrr = isrr && (await conditions[i].ToExpressionString(context));
                    }
                }
            }
            return isrr;
        }

    }
}
