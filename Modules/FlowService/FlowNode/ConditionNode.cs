using DynamicExpresso;
using FlowService.FlowNode.Builder;
using FlowService.FlowNode.Conditions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FlowService.FlowNode
{
    public class ConditionNode : FlowBaseNode
    {
        public ConditionsProps props { get; set; }
    }

    public class ConditionsProps
    {
        /// <summary>
        /// 条件组逻辑关系 OR、AND
        /// </summary>
        public string groupsType { get; set; }
        public ConditionGroup[] groups { get; set; }
        /// <summary>
        /// 自定义表达式，灵活构建逻辑关系
        /// </summary>
        public string expression { get; set; }
        public string ToExpressionString(BuilderContext context)
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
                bool allcal = true;
                foreach (char c in expression)
                {
                    if (char.IsLetter(c))
                    {
                        var res = dic[c.ToString()].ToExpressionString(context);
                        if (res != "true" && res != "false")
                        {
                            allcal = false;
                        }
                        sb.Append(res);
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
                string tmprt = sb.ToString();
                if (allcal)
                {
                    var interpreter = new Interpreter();
                    return interpreter.Eval<bool>(tmprt) ? "true" : "false";
                }
                return tmprt;
            }
            else
            {
                string expression = string.Empty;
                for (int i = 0; i < groups.Length; i++)
                {
                    if (i == 0)
                    {
                        expression = groups[i].ToExpressionString(context);
                    }
                    else
                    {
                        expression = ConditionGroup.ExpressionCompare(expression, groups[i].ToExpressionString(context), groupsType);
                    }
                }
                return expression;
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
        public static string ExpressionCompare(string pre, string curent, string t)
        {
            if (pre == "true" && curent == "true")
            {
                return "true";
            }
            else if (pre == "true" && curent == "false")
            {
                return t == "OR" ? "true" : "false";
            }
            else if (pre == "false" && curent == "true")
            {
                return t == "OR" ? "true" : "false";
            }
            else if (pre == "false" && curent == "false")
            {
                return "false";
            }
            else
            {
                return pre + (t == "OR" ? "||" : "&&") + curent;
            }
        }
        public string ToExpressionString(BuilderContext context)
        {
            string expression = string.Empty;

            foreach (var condi in conditions)
            {
                if (expression == string.Empty)
                {
                    expression = condi.ToExpressionString(context);
                }
                else
                {
                    expression = ExpressionCompare(expression, condi.ToExpressionString(context), groupType);
                }
            }
            if (expression == string.Empty)
            {
                return "false";
            }
            if (expression == "true" || expression == "false")
            {
                return expression;
            }
            return "(" + expression + ")";
        }

    }
}
