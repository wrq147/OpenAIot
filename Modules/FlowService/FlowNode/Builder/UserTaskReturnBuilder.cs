using FlowService.FlowNode.Builder.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder
{
    public class UserTaskReturnBuilder<T> where T : UserTask
    {
        private ExecutorBuilder _builder;
        public ExecutorBuilder Builder { get { return _builder; } }
        private StepBuilder<T> _referenceBuilder;

        private StepBuilder<WhenStep> _step;

        public UserTaskReturnBuilder(ExecutorBuilder builder, StepBuilder<WhenStep> step, StepBuilder<T> referenceBuilder)
        {
            _builder = builder;
            _step = step;
            _referenceBuilder = referenceBuilder;
        }

        public StepBuilder<T> Do(Action<StepBuilder<WhenStep>> builder)
        {
            builder.Invoke(_step);
            return _referenceBuilder;
        }
    }
}
