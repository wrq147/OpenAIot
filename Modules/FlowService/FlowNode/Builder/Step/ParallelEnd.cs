using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 并行结束
    /// </summary>
    public class ParallelEnd : WorkflowStep
    {
        public ParallelEnd()
        {
            this.PersistenceNode = false;
        }
        public const string Finsh_Key = "FBranchCount";
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            var parentStep = context.StepNodes[context.StepNodes[this.ParentIndex].ParentIndex];
            string parentId = parentStep.Id;
            var parentNode = context.Workflow.ExecutionNodes.Where(x => x.StepId == parentId).FirstOrDefault();
            var attr = parentNode.FindAttribute(Finsh_Key);
            int res = 0;
            if (attr == null)
            {
                res = 1;
                parentNode.UpdateAttribute(Finsh_Key, "1");
            }
            else
            {
                res = Convert.ToInt32(attr.AttributeValue) + 1;
                parentNode.UpdateAttribute(Finsh_Key, res.ToString());
            }
            if (res == parentStep.Children.Count)
            {
                for (int i = this.Index; i < context.StepNodes.Count; i++)
                {
                    if (context.StepNodes[i].Level <= context.StepNodes[this.ParentIndex].Level)
                    {
                        context.ExcuteIndex = i - 1;
                        break;
                    }
                }
                return await ExecutionResult.Next();
            }
            else
            {
                var tmpres = new ExecutionResult();
                tmpres.Directive = ExecutionDirective.Defer;
                return await Task.FromResult(tmpres);
            }
        }
    }
}
