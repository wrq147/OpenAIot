using DynamicExpresso;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    public class IfStep : WorkflowStep
    {
        public IfStep()
        {
            this.PersistenceNode = false;
        }
        public delegate bool ConditionDelegate(StepExecutionContext context);
        /// <summary>
        /// 是否为最后的条件
        /// </summary>
        public bool IsLastCondition { get; set; }
        public int FirstIndex { get; set; }
        /// <summary>
        /// 参数名
        /// </summary>
        public string ConditionName { get; set; }
        /// <summary>
        /// 表达式
        /// </summary>
        public string ConditionBody { get; set; }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            string tkey = "if-" + FirstIndex;
            if (this.Index == FirstIndex)
            {
                //首个条件先清标记
                context.ExecutionPointer.RemoveAttribute(tkey);
            }
            var attr = context.ExecutionPointer.FindAttribute(tkey);
            if (attr == null || attr.AttributeValue != "true")
            {
                var interpreter = new Interpreter();
                var func = interpreter.ParseAsDelegate<ConditionDelegate>(ConditionBody, ConditionName);
                if (func.Invoke(context))
                {
                    context.ExecutionPointer.UpdateAttribute(tkey, "true");
                    var res = new ExecutionResult();
                    res.ActiveChildren = true;
                    return res;
                }
            }
            else
            {
                return await ExecutionResult.Next();
            }
            if (IsLastCondition)
            {
                var tmpres = new ExecutionResult();
                tmpres.Directive = ExecutionDirective.Defer;
                return await Task.FromResult(tmpres);
            }
            else
            {
                return await ExecutionResult.Next();
            }
        }
    }
}
