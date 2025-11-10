using IoTRulesService.Flow.Builder.Step;
using System;

namespace IoTRulesService.Flow.Builder
{
    public class ParallelBuilder
    {
        private RuleExecutorBuilder _builder;
        public RuleExecutorBuilder Builder { get { return _builder; } }
        private StepBuilder<ParallelStep> _referenceBuilder;

        public ParallelBuilder(RuleExecutorBuilder builder, StepBuilder<ParallelStep> referenceBuilder)
        {
            _builder = builder;
            _referenceBuilder = referenceBuilder;
        }

        public StepBuilder<ParallelStep> Do(Action<StepBuilder<ParallelStart>> builder)
        {
            ParallelStart start = new ParallelStart();
            start.Level = _referenceBuilder.Step.Level + 1;
            start.ParentIndex = _referenceBuilder.Step.Index;
            start.Name = typeof(ParallelStart).Name;
            _builder.AddStep(start);
            _referenceBuilder.Step.Children.Add(start.Index);

            StepBuilder<ParallelStart> newStepBuilder = new StepBuilder<ParallelStart>(_builder, start);
            builder.Invoke(newStepBuilder);


            ParallelEnd step = new ParallelEnd();
            step.Level = start.Level + 1;
            step.ParentIndex = start.Index;
            step.Name = typeof(ParallelEnd).Name;
            _builder.AddStep(step);
            start.Children.Add(step.Index);


            return _referenceBuilder;
        }
    }
}
