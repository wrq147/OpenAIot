using Common.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlowService.Model
{
    /// <summary>
    /// 用户选择节点
    /// </summary>
    public class Out_UserNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 或签，会签
        /// </summary>
        public string Tip { get; set; }
        /// <summary>
        /// Approval为审批，CS为抄送
        /// </summary>
        public string Type { get; set; }
        public bool CanAdd { get; set; }
        public List<Out_UserItem> Value { get; set; }
    }
    public class Out_UserItem
    {
        public string Id { get; set; }
        public string RealName { get; set; }
        [JsonConverter(typeof(AvatarUrl))]
        public string Avatar { get; set; }
    }
}
