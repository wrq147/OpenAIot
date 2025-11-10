using Common;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService
{
    public static class AuthExtension
    {
        public static T GetItem<T>(this ITAContext context, string key)
        {
            return (T)context.Items[key];
        }
        public static async Task<string> GetIpLocation(this ITAContext context, string ip)
        {
            var tmpstr = context.GetItem<string>("$IpLocation");
            if (tmpstr == null)
            {
                tmpstr = await IpHelper.Instance.GetRealAddressByIP(ip);
                context.Items["$IpLocation"] = tmpstr;
            }
            return tmpstr;
        }
        public static string GetTerminal(this ITAContext context)
        {
            string agent = context.Request.UserAgent.ToLower();
            if (agent.Contains("micromessenger"))
            {
                return "weixin";
            }
            else if (agent.Contains("iphone") || agent.Contains("ipod") || agent.Contains("ipad"))
            {
                return "ios";
            }
            else if (agent.Contains("android"))
            {
                return "android";
            }
            else
            {
                return "pc";
            }
        }
    }
}
