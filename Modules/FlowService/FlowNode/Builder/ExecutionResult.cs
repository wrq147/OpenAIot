using System;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder
{
    public class ExecutionResult
    {
        public string OutcomeValue { get; set; }

        public ExecutionDirective Directive { get; set; }

        public bool ActiveChildren { get; set; }
        public bool Parallel { get; set; }
        public ExecutionResult(string outcome = null)
        {
            Directive = ExecutionDirective.Next;
            OutcomeValue = outcome;
            Parallel = false;
        }

        public static Task<ExecutionResult> Next()
        {
            return Task.FromResult(new ExecutionResult
            {
                Directive = ExecutionDirective.Next,
                OutcomeValue = null
            });
        }
        public static Task<ExecutionResult> End()
        {
            return Task.FromResult(new ExecutionResult
            {
                Directive = ExecutionDirective.EndWorkflow,
                OutcomeValue = null
            });
        }
        public static Task<ExecutionResult> WaitForEvent(string eventKey)
        {
            return Task.FromResult(new ExecutionResult
            {
                Directive = ExecutionDirective.Defer,
                OutcomeValue = eventKey
            });
        }

    }
    public enum ExecutionDirective
    {
        Next = 0,
        Defer = 1,
        EndWorkflow = 2
    }
}
