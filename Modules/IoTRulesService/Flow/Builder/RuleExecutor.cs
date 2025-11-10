using ChannelUtility;
using ChannelUtility.Message;
using Common;
using IoTRulesService.DAL;
using IoTRulesService.Flow.Builder.Step;
using IoTRulesService.Model;
using MonitorService.Business;
using MonitorService.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder
{
    public class RuleExecutor
    {
        private List<RuleflowStep> _steps;
        public List<RuleflowStep> Steps
        {
            get { return _steps; }
        }
        private ITAServiceProvider _provider;
        public RuleExecutor(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        public void Init(List<RuleflowStep> steps)
        {
            _steps = steps;
        }
        public RuleExecutionContext CreateContext(BaseDeviceMessage data, long ruleId, Dictionary<string, object> globalParams)
        {
            return new RuleExecutionContext(_provider, 0, ruleId, data, globalParams, _steps);
        }
        public async Task<RuleExecutionContext> Execute(ExecuteRuleMessage source, Dictionary<string, string> paramTypes, Dictionary<string, object> globalParams, bool isDebug, long ruleId)
        {
            var context = this.CreateContext(source, ruleId, globalParams);
            context.IsDebug = isDebug;
            context.ParamTypeDict = paramTypes;
            context.Data.Add(StreamData.Create(source.Inputs));
            context.ExecuteTime = DateTime.Now;
            await Execute(context);
            return context;
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(RuleExecutionContext context)
        {
            try
            {
                await this.ExcuteStep(context);
                if (!context.FinishExecute && context.JobList.Count > 0)
                {
                    //添加定时任务
                    var jobBLL = _provider.GetService<JobBLL>();
                    foreach (var job in context.JobList)
                    {
                        await jobBLL.InsertJob(job);
                    }

                    MZ_RuleEvent evt = new MZ_RuleEvent();
                    evt.Id = context.RuleInstanceId;
                    evt.InstanceJson = JsonConvert.SerializeObject(_steps);
                    evt.StackJson = JsonConvert.SerializeObject(context.Data);
                    evt.RuleId = context.RuleId;
                    evt.SourceJson = JsonConvert.SerializeObject(context.Source);
                    evt.Index = context.Step.Index;
                    await _provider.GetService<RuleEventDAL>().Insert(evt);
                }
            }
            catch (Exception ex)
            {
                await context.Print(ex.Message);
            }

        }


        /// <summary>
        /// 执行完成就返回true
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task ExcuteStep(RuleExecutionContext context)
        {
            if (context.ExcuteIndex >= context.Steps.Count)
            {
                context.FinishExecute = true;
                return;
            }


            await context.Step.Run(context);
        }
    }
}
