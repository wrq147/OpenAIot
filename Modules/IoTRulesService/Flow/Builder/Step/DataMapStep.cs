using System;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 数据转换节点
    /// </summary>
    public class DataMapStep : RuleflowStep
    {
        public string ScriptBody { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            if (context.IsDebug)
            {
                await context.Print("输入数据:" + Newtonsoft.Json.JsonConvert.SerializeObject(context.Data));
            }
            try
            {
                await context.ExeScript("handler", ScriptBody);
            }
            catch (Exception ex)
            {
                await context.Print("脚本异常:" + ex.Message);
            }

        }
    }
}
