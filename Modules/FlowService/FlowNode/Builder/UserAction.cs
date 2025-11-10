using AuthService;
using Common.Attr;
using Common.Share;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder
{
    public class UserAction
    {
        public ActionUser User { get; set; }
        public Dictionary<string, object> Model { get; set; }
        public string OutcomeValue { get; set; }
    }
    public class ActionUser : IUserInfo
    {
        public long OrgId { get; set; }
        public long UserId { get; set; }
        /// <summary>
        /// 处理的动作
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 签名图
        /// </summary>
        public string SignImg { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ActionDate { get; set; }
    }
    public class Out_ActionUser : ActionUser
    {
        public string RealName { get; set; }
        [JsonConverter(typeof(ImageUrl), true)]
        public string Avatar { get; set; }
    }
}
