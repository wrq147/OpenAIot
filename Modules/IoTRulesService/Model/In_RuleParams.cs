using System;

namespace IoTRulesService.Model
{
    public class In_RuleParams
    {
        /// <summary>
        /// 规则Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 规则参数Json格式字符串
        /// </summary>
        public string HttpParams { get; set; }
    }
}
