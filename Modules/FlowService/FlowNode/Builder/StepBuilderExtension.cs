using FlowService.FlowNode.Builder.Step;
using System;
using System.Collections.Generic;

namespace FlowService.FlowNode.Builder
{
    public static class StepBuilderExtension
    {
        public static UserTaskReturnBuilder<T> WithOption<T>(this StepBuilder<T> builder, string value, string type) where T : UserTask
        {
            if (builder.Step.Options == null)
            {
                builder.Step.Options = new List<OptionItem>();
            }
            builder.Step.Options.Add(new OptionItem(value, type));
            return new UserTaskReturnBuilder<T>(builder.Builder, builder.When(value), builder);
        }
    }
}
