using Common.Attr;
using MyAccess.DB.Attr;
using System;

namespace IoTRulesService.Model
{
    [TableName("mz_rule_trigger")]
    public class MZ_RuleTrigger
    {
        [ID(true)]
        public long Id { get; set; }
        public long RuleId { get; set; }

        /// <summary>
        /// 订阅的设备：/Product/Device
        /// </summary>
        public string TopicDevice { get; set; }
        /// <summary>
        /// 订阅的消息类型：参考JsonMessageConverter里的（当触发方式为0时使用）
        /// </summary>
        public string TopicMsg { get; set; }
    }
}
