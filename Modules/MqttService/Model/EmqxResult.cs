using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MqttService.Model
{
    public class EmqxResult: AjaxResult
    {
        /// <summary>
        /// 认证结果通过 body 中的 result 标示，可选 allow、deny、ignore。
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 是否为超级用户
        /// </summary>
        public bool is_superuser { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
