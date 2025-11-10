using IoTRulesService.Flow.Builder.Step;
using System;
using System.Linq.Expressions;
using static IoTRulesService.Flow.Builder.Step.IfStep;

namespace IoTRulesService.Flow.Builder
{
    public class StepBuilder<T> where T : RuleflowStep
    {
        private RuleExecutorBuilder _builder;
        public RuleExecutorBuilder Builder { get { return _builder; } }
        private T _step;
        public T Step { get { return _step; } }
        public StepBuilder(RuleExecutorBuilder builder, T step)
        {
            _builder = builder;
            _step = step;
        }
        public StepBuilder<TStep> Start<TStep>(Action<StepBuilder<TStep>> stepSetup = null) where TStep : RuleflowStep, new()
        {
            TStep step = new TStep();
            step.Level = _step.Level + 1;
            step.ParentIndex = _step.Index;
            step.Name = typeof(TStep).Name;
            StepBuilder<TStep> newStepBuilder = new StepBuilder<TStep>(_builder, step);
            if (stepSetup != null)
            {
                stepSetup.Invoke(newStepBuilder);
            }
            _builder.AddStep(step);
            _step.Children.Add(step.Index);
            return newStepBuilder;
        }
        public StepBuilder<TStep> Then<TStep>(Action<StepBuilder<TStep>> stepSetup = null) where TStep : RuleflowStep, new()
        {
            TStep step = new TStep();
            step.Level = _step.Level;
            step.ParentIndex = _step.ParentIndex;
            step.Name = typeof(TStep).Name;
            StepBuilder<TStep> newStepBuilder = new StepBuilder<TStep>(_builder, step);
            if (stepSetup != null)
            {
                stepSetup.Invoke(newStepBuilder);
            }
            _builder.AddStep(step);
            return newStepBuilder;
        }
        public StepBuilder<IfStep> If(Action<StepBuilder<IfStep>> stepSetup = null)
        {
            IfStep step = new IfStep();
            step.Level = _step.Level;
            step.ParentIndex = _step.ParentIndex;
            step.Name = typeof(IfStep).Name;
            step.IsLastCondition = false;
            StepBuilder<IfStep> newStepBuilder = new StepBuilder<IfStep>(_builder, step);
            if (stepSetup != null)
            {
                stepSetup.Invoke(newStepBuilder);
            }
            _builder.AddStep(step);
            if (_step is IfStep oldStep)
            {
                step.FirstIndex = oldStep.FirstIndex;
            }
            else
            {
                step.FirstIndex = step.Index;
            }
            return newStepBuilder;
        }
  
        public virtual StepBuilder<T> Do(Action<StepBuilder<T>> builder)
        {
            builder.Invoke(this);
            return this;
        }

        public virtual ParallelBuilder Parallel(Action<StepBuilder<ParallelStep>> stepSetup = null)
        {
            ParallelStep step = new ParallelStep();
            step.Level = _step.Level;
            step.ParentIndex = _step.ParentIndex;
            step.Name = typeof(ParallelStep).Name;
            StepBuilder<ParallelStep> newStepBuilder = new StepBuilder<ParallelStep>(_builder, step);
            if (stepSetup != null)
            {
                stepSetup.Invoke(newStepBuilder);
            }
            _builder.AddStep(step);
            return new ParallelBuilder(_builder, newStepBuilder);
        }
    }
}
