using FlowService.Model;
using System;
using System.Collections.Generic;
using FlowService.FlowNode.Builder.Step;
using TemplateAction.Core;
using MonitorService.Model;
using Common.EventBus;

namespace FlowService.FlowNode.Builder
{
    public class StepExecutionContext : BuilderContext
    {
        public MZ_Flow_Node ExecutionPointer { get; set; }

        public WorkflowStep Step
        {
            get { return StepNodes[this.ExcuteIndex]; }
        }
        /// <summary>
        /// 当前执行的索引位置
        /// </summary>
        public int ExcuteIndex { get; set; }
        public ITAServiceProvider ServiceProvider { get; set; }

        public int FindStep(string id)
        {
            for (int i = 0; i < StepNodes.Count; i++)
            {
                if (StepNodes[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        public MZ_Flow Workflow { get; set; }
        public List<WorkflowStep> StepNodes { get; set; }

        public override long Creator { get => this.Workflow.createId.Value; set => this.Workflow.createId = value; }

        /// <summary>
        /// 执行后新增加的调试任务
        /// </summary>
        public List<MZ_Job> JobList = new List<MZ_Job>();
        /// <summary>
        /// 执行后新增加的消息通知
        /// </summary>
        public List<NoticeEvent> NoticeList = new List<NoticeEvent>();
    }
}
