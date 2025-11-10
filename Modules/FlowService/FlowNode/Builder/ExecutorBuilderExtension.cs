using FlowService.FlowNode.Builder.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder
{
    public static class ExecutorBuilderExtension
    {
        public static StepBuilder<StartStep> Start(this ExecutorBuilder builder, RootNode root)
        {
            var step = new StartStep();
            step.Level = 0;
            step.ParentIndex = -1;
            step.Id = root.id;
            step.Name = root.name;
            step.PersistenceNode = true;
            step.FormPerms = root.props.formPerms;
            builder.AddStep(step);
            return new StepBuilder<StartStep>(builder, step);
        }
        public static void End(this ExecutorBuilder builder)
        {
            var step = new EndStep();
            step.Id = "end";
            step.Level = 0;
            step.ParentIndex = -1;
            step.Name = "终点";
            step.PersistenceNode = true;
            builder.AddStep(step);
        }
    }
}
