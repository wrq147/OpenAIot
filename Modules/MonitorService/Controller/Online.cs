using AuthService;
using AuthService.Controller;
using Common;
using Common.Share;
using MonitorService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Linq;
namespace MonitorService.Controller
{
    /// <summary>
    /// 在线用户
    /// </summary>
    [About("/MonitorService/Online")]
    public class Online : AbstractLoginedController
    {
        private UserBLL _userBLL;
        public Online(UserBLL user)
        {
            _userBLL = user;
        }
        private OnlineUser ServerTokenInfo2User(long uid, Data_ServerTokenInfo user)
        {
            OnlineUser onlineUser = new OnlineUser();
            onlineUser.UserId = uid;
            onlineUser.UserName = user.UserName;
            onlineUser.Avatar = user.Avatar;
            onlineUser.OSName = user.OSName;
            onlineUser.Location = user.Location;
            onlineUser.Ipaddr = user.Ipaddr;
            onlineUser.Browser = user.Browser;
            onlineUser.LoginTime = user.LoginTime;
            return onlineUser;
        }
        [HttpGet]
        public async Task<AjaxResult> List(int pageNum, int pageSize, string userName = "")
        {
            string searchKeys = "Token";
            if (!string.IsNullOrEmpty(userName))
            {
                MZ_AdminInfo info = await _userBLL.GetUserInfoByName(userName);
                searchKeys = searchKeys + info.Id + "_*";
            }
            else
            {
                searchKeys += "*";
            }

            GeneralRedisHelper redis = this.ServiceProvider.GetService<GeneralRedisHelper>();
            var keys = await redis.KeysAsync(searchKeys);
            int totalCount = keys.Count;
            keys = keys.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            List<OnlineUser> userOnlineList = new List<OnlineUser>();
            foreach (string key in keys)
            {
                string[] tarr = key.Substring(5).Split("_");
                long uid = long.Parse(tarr[0]);
                Data_ServerTokenInfo user = await redis.StringGetAsync<Data_ServerTokenInfo>(key);
                userOnlineList.Add(ServerTokenInfo2User(uid, user));
            }


            return this.Success(new PageObject<OnlineUser>()
            {
                List = userOnlineList,
                Total = totalCount
            });
        }




        /// <summary>
        /// 强退用户
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ForceLogout(string id)
        {
            GeneralRedisHelper redis = this.ServiceProvider.GetService<GeneralRedisHelper>();
            var tmpkeys = await redis.KeysAsync("Token" + id + "*");
            foreach (string k in tmpkeys)
            {
                await redis.KeyDeleteAsync(k);
                await redis.StringSetAsync("ForceRelogin:" + id, "Y", TimeSpan.FromHours(1));
            }
            return this.Success<string>();
        }
    }
}
