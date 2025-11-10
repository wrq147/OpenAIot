using FlowService.FlowNode.Builder.Step;
using System;
using System.Linq.Expressions;
using TemplateAction.Core;

namespace FlowService.FlowNode.Builder
{
    public class StepBuilder<T> where T : WorkflowStep
    {
        private ExecutorBuilder _builder;
        public ExecutorBuilder Builder { get { return _builder; } }
        private T _step;
        public T Step { get { return _step; } }
        public StepBuilder(ExecutorBuilder builder, T step)
        {
            _builder = builder;
            _step = step;
        }
        public StepBuilder<TStep> Start<TStep>(Action<StepBuilder<TStep>> stepSetup = null) where TStep : WorkflowStep, new()
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
        public StepBuilder<TStep> Then<TStep>(Action<StepBuilder<TStep>> stepSetup = null) where TStep : WorkflowStep, new()
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
        public StepBuilder<WhenStep> When(string expectedOutcome)
        {
            WhenStep step = new WhenStep();
            if (_step.GetType() == typeof(WhenStep))
            {
                step.Level = _step.Level;
                step.ParentIndex = _step.ParentIndex;
            }
            else
            {
                step.Level = _step.Level + 1;
                step.ParentIndex = _step.Index;
            }

            step.Name = typeof(WhenStep).Name;
            step.ExpectedOutcome = expectedOutcome;
            StepBuilder<WhenStep> newStepBuilder = new StepBuilder<WhenStep>(_builder, step);
            _builder.AddStep(step);
            return newStepBuilder;
        }
        public StepBuilder<IfStep> If(Expression<Func<StepExecutionContext, bool>> condition, Action<StepBuilder<IfStep>> stepSetup = null)
        {
            IfStep step = new IfStep();
            step.Level = _step.Level;
            step.ParentIndex = _step.ParentIndex;
            step.Name = typeof(IfStep).Name;
            step.IsLastCondition = false;
            if (condition != null)
            {
                step.ConditionName = condition.Parameters[0].Name;
                step.ConditionBody = condition.Body.ToString();
            }
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

        public virtual ParallelReturnBuilder Parallel(Action<StepBuilder<ParallelStep>> stepSetup = null)
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
            return new ParallelReturnBuilder(_builder, newStepBuilder);
        }
    }
}
