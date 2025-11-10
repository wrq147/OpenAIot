using Castle.Components.DictionaryAdapter;
using IoTRulesService.Flow.Builder.Step;
using IoTRulesService.Flow.Node;
using Jint.Native;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder
{
    /// <summary>
    /// 构建规则
    /// </summary>
    public class RuleExecutorBuilder
    {
        private ITAServiceProvider _provider;
        private List<RuleflowStep> _steps = new List<RuleflowStep>();
        public RuleExecutorBuilder(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public void AddStep(RuleflowStep step)
        {
            step.Index = _steps.Count;
            _steps.Add(step);
        }
        public async Task<RuleExecutor> Build(string json)
        {
            RootNode root = JsonConvert.DeserializeObject<RootNode>(json, new JsonNodeConverter(), new JsonConditionConverter());
            RuleExecutor exe = _provider.GetService<RuleExecutor>();
            BuildRuleflowNode<StartStep>(root, null);
            //添加终点
            var step = new EndStep();
            step.Id = "EndStep";
            step.Level = 0;
            step.ParentIndex = -1;
            step.Name = "终点";
            AddStep(step);

            exe.Init(_steps);
            return await Task.FromResult(exe);
        }

        private void BuildRuleflowNode<T>(RuleBaseNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            if (node == null) return;
            switch (node.type)
            {
                case "ROOT":
                    {
                        var step = new StartStep();
                        step.Id = "root";
                        step.Level = 0;
                        step.ParentIndex = -1;
                        step.Name = node.name;
                        AddStep(step);
                        BuildRuleflowNode(node.children, new StepBuilder<StartStep>(this, step));
                    }
                    break;
                case "EMPTY":
                    BuildRuleflowNode(node.children, stepBuilder);
                    break;
                case "CONDITIONS":
                    var nodes = (ConditionGroupNode)node;
                    BuildCondition(nodes.branchs, 0, stepBuilder);
                    BuildRuleflowNode(node.children, stepBuilder);
                    break;
                case "CONCURRENTS":
                    BuildParallel((ConcurrentGroupNode)node, stepBuilder);
                    BuildRuleflowNode(node.children, stepBuilder);
                    break;
                case "DELAY":
                    BuildDelay((DelayNode)node, stepBuilder);
                    break;
                case "DATAWRITE":
                    BuildDataWrite((DataWriteNode)node, stepBuilder);
                    break;
                case "CONVERSION":
                    BuildConvert((ConvertNode)node, stepBuilder);
                    break;
                case "TRIGGER":
                    BuildHttp((HttpNode)node, stepBuilder);
                    break;
                case "FUNC":
                    BuildFunc((FuncNode)node, stepBuilder);
                    break;
                case "WARN":
                    BuildWarn((WarnNode)node, stepBuilder);
                    break;
                case "METRONOME":
                    BuildCount((CountNode)node, stepBuilder);
                    break;
                case "EXCEPT":
                    BuildExcept((ExceptNode)node, stepBuilder);
                    break;
                case "CLEARDELTA":
                    BuildClearDelta((ClearDeltaNode)node, stepBuilder);
                    break;
                case "TIMESCHEDULER":
                    BuildTimeScheduler((TimeSchedulerNode)node, stepBuilder);
                    break;
                case "TAG":
                    BuildTag((TagNode)node, stepBuilder);
                    break;
                case "REDIRECT":
                    BuildRedirect((RedirectNode)node, stepBuilder);
                    break;
                case "PID":
                    BuildPID((PIDNode)node, stepBuilder);
                    break;
                case "SETPROP":
                    BuildSetProp((SetPropNode)node, stepBuilder);
                    break;
            }
        }


        private void BuildCondition<T>(ConditionNode[] nodes, int i, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            if (i >= nodes.Length)
            {
                return;
            }
            ConditionNode node = nodes[i];
            var nextBuilder = stepBuilder.If(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
                if (i == nodes.Length - 1)
                {
                    x.Step.IsLastCondition = true;
                }
            }).Do(then =>
             {
                 BuildRuleflowNode(node.children, then.Start<StartStep>());
             });

            BuildCondition(nodes, i + 1, nextBuilder);
        }

        private void BuildParallel<T>(ConcurrentGroupNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Parallel();
            foreach (var branchNode in node.branchs)
            {
                builder.Do(then =>
                {
                    BuildRuleflowNode(branchNode.children, then.Start<NextStep>());
                });
            }
        }
        private void BuildDelay<T>(DelayNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<DelayStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildDataWrite<T>(DataWriteNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<DataWriteStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildConvert<T>(ConvertNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<DataMapStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.ScriptBody = node.props.func;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildHttp<T>(HttpNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<HttpStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildFunc<T>(FuncNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<FuncStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildWarn<T>(WarnNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<WarnStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildCount<T>(CountNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<CountStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.Way = node.props.Way;
                x.Step.FieldName = node.props.FieldName;
                x.Step.Count = node.props.Count;
                x.Step.CountLen = node.props.CountLen;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildExcept<T>(ExceptNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<ExceptStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildClearDelta<T>(ClearDeltaNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<ClearDeltaStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildTimeScheduler<T>(TimeSchedulerNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<TimeSchedulerStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildTag<T>(TagNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<TagStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }

        private void BuildRedirect<T>(RedirectNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<RedirectStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }
        private void BuildPID<T>(PIDNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<PIDStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }


        private void BuildSetProp<T>(SetPropNode node, StepBuilder<T> stepBuilder) where T : RuleflowStep
        {
            var builder = stepBuilder.Then<SetPropStep>(x =>
            {
                x.Step.Id = node.id;
                x.Step.Name = node.name;
                x.Step.props = node.props;
            });
            BuildRuleflowNode(node.children, builder);
        }

    }
}
