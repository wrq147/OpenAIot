using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttService.Model
{
    public class EmqxAclParam
    {
        public string username { get; set; }
        /// <summary>
        /// 当前请求想要发布或订阅的主题（或主题过滤器）
        /// </summary>
        public string topic { get; set; }
        /// <summary>
        /// 当前执行的动作请求，例如 publish，subscribe。
        /// </summary>
        public string action { get; set; }
    }
}
