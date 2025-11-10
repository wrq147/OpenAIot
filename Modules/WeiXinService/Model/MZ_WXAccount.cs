using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    public class MZ_WXAccount
    {
        public string Name { get; set; }
        public string AgentId { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;

        public string AppSecret { get; set; } = string.Empty;
        /// <summary>
        /// 企业微信账号绑定的组织Id
        /// </summary>
        public long OrgId { get; set; } = 0;
        /// <summary>
        /// 存储账号绑定的组织名称
        /// </summary>
        public string OrgName { get; set;}
        /// <summary>
        /// 账号类型：公众号js,小程序applet，企业微信corp
        /// </summary>
        public string AccType { get; set; }
    }
}
