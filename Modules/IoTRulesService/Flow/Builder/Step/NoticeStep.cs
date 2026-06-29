using IoTRulesService.Flow.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    public class NoticeStep : RuleflowStep
    {
        public NoticeProps props { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            var dataItem = context.Data.LastOrDefault();
            IDictionary<string, object> inputs = null;
            if (dataItem != null)
            {
                inputs = dataItem.Data;
            }
            else
            {
                inputs = new Dictionary<string, object>();
            }
            switch (props.NoticeWay)
            {
                case "APP":
                    break;
                case "EMAIL":
                    break;
                case "SMS":
                    break;
            }

            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
