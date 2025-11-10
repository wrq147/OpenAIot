using EasyNetQ;
using IoTRulesService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService
{
    /// <summary>
    /// 规则变更事件
    /// </summary>
    [QueueAttribute("rule_change_queue", ExchangeName = "rule_change_exchange")]
    public class RuleChangeEvent
    {
        public const string EventKey = "/EV.BUS.RULECHANGE";
        public List<MZ_RuleTrigger> Triggers { get; set; }
        public int ChangeType { get; set; }
        public RuleChangeEvent(List<MZ_RuleTrigger> list)
        {
            this.Triggers = list;
            ChangeType = 0;
        }
        public bool IsDebug { get; set; }
        public long RuleId { get; set; }
        public RuleChangeEvent() { }
        public RuleChangeEvent(bool debugState, long ruleId)
        {
            IsDebug = debugState;
            RuleId = ruleId;
            ChangeType = 1;
        }
    }
}
