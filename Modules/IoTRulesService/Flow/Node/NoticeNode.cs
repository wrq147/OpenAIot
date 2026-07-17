using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    public class NoticeNode : RuleBaseNode
    {
        public NoticeProps props { get; set; }
    }
    public class NoticeProps
    {
        /// <summary>
        /// 通知方式:APP站内通知,EMAIL邮件通知,SMS短信通知
        /// </summary>
        public string NoticeWay { get; set; }
     
        /// <summary>
        /// 推送的内容（长度不超过80字符）
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 邮件通知时传固定邮箱，短信通知时传固定手机号，APP站内通知时传用户Id
        /// </summary>
        public string TargetValue { get; set; }
    }
}
