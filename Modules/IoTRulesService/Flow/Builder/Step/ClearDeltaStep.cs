using IoTRulesService.Flow.Node;
using IoTService;
using JiebaNet.Segmenter.Common;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 数据清除
    /// </summary>
    public class ClearDeltaStep : RuleflowStep
    {
        public ClearProps props { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            try
            {
                switch (props.ClearType)
                {
                    case 0:
                        await context.ClearParam(props.Codes);
                        break;
                    case 1:
                        await context.ClearDevice(props.Codes);
                        break;
                }
            }
            catch (Exception ex)
            {
                await context.Print("异常：" + ex.Message);
            }
            if (context.IsDebug)
            {
                await context.Print($"完成{props.Codes.Join()}清除");
            }
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
