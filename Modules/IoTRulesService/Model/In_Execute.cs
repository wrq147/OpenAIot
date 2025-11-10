using System;
using System.Collections.Generic;

namespace IoTRulesService.Model
{
    public class In_Execute
    {
        /// <summary>
        /// 规则Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 其它自定义参数
        /// </summary>
        public Dictionary<string,object> Inputs { get; set; }
    }
}
