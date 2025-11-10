using Common;
using Common.Share;
using Common.UserAgent;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService
{
    /// <summary>
    /// 令牌操作类
    /// </summary>
    public class OperatorHelper
    {
        private ITAServiceProvider _provider;
        private IOptions<GeneralOption> _conf;
        public OperatorHelper(ITAServiceProvider provider, IOptions<GeneralOption> conf)
        {
            _provider = provider;
            _conf = conf;
        }


        /// <summary>
        /// 判断令牌签名是否正确
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        public bool CheckClientToken(Data_ClientToken info, string tk)
        {
            StringBuilder sb = new StringBuilder(200);
            sb.Append(info.UserId);
            sb.Append(info.Time);
            sb.Append(info.Mode);
            sb.Append(info.Ext);
            sb.Append(tk);
            return string.Equals(info.Sign, MyAccess.Core.Crypter.SHA1(sb.ToString(), System.Text.Encoding.UTF8));
        }


        /// <summary>
        /// 生成刷新令牌（长期使用，ip与客户端信息变更后不可使用)
        /// </summary>
        /// <param name="clientToken"></param>
        /// <param name="ip"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public string MakeRefreshToken(string clientToken, string agent)
        {
            return MyAccess.Core.Crypter.SHA1(clientToken + agent + _conf.Value.secret_key, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 解释token字符串
        /// </summary>
        /// <param name="tk"></param>
        /// <returns></returns>
        public Data_ClientToken ParseClientToken(string tk)
        {
            Data_ClientToken ct = null;
            try
            {
                string decode = MyAccess.Core.Crypter.DecodeBase64(tk, Encoding.UTF8);
                ct = JsonConvert.DeserializeObject<Data_ClientToken>(decode);
            }
            catch { }
            return ct;
        }

        /// <summary>
        /// 获取服务端登录数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Data_ServerTokenInfo> GetServerData(long uid, string terminal)
        {
            string tkey = string.Format("Token{0}_{1}", uid, terminal);
            return await _provider.GetService<GeneralRedisHelper>().StringGetAsync<Data_ServerTokenInfo>(tkey);
        }
        /// <summary>
        /// 删除服务端登录数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task DeleteServerData(long uid, string terminal)
        {
            string tkey = string.Format("Token{0}_{1}", uid, terminal);
            await _provider.GetService<GeneralRedisHelper>().KeyDeleteAsync(tkey);
        }

        /// <summary>
        /// 刷新服务端登录数据
        /// </summary>
        /// <param name="account"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task RefreshServerData(MZ_AdminInfo account, ITAContext context)
        {
            string tkey = string.Format("Token{0}_{1}", account.Id, context.GetTerminal());
            Data_ServerTokenInfo savetk = await _provider.GetService<GeneralRedisHelper>().StringGetAsync<Data_ServerTokenInfo>(tkey);
            if (savetk != null)
            {
                savetk.OrgId = account.OrgId.GetValueOrDefault(0);
                savetk.UserId = account.Id.Value;
                savetk.UserName = string.IsNullOrEmpty(account.RealName) ? account.UserName : account.RealName;
                savetk.Avatar = account.Avatar;
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(tkey, savetk, MyAccess.Core.TypeConvert.Unix2Time(savetk.Expire) - DateTime.Now);
            }
        }
        /// <summary>
        /// 清除指定登录数据
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task ClearServerData(long uid)
        {
            var tmpredis = _provider.GetService<GeneralRedisHelper>();
            var allkeys = await tmpredis.KeysAsync(string.Format("Token{0}_*", uid));
            foreach (string k in allkeys)
            {
                tmpredis.KeyDelete(k);
            }
        }
        /// <summary>
        /// 判断是否需要重新登录
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<bool> JudgeRelogin(long uid)
        {
            return await _provider.GetService<GeneralRedisHelper>().KeyExistsAsync("ForceRelogin:" + uid);
        }

        /// <summary>
        /// 重新生成服务端数据
        /// </summary>
        /// <param name="account"></param>
        /// <param name="loginTime"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<Data_ServerTokenInfo> MakeServerData(MZ_AdminInfo account, long loginTime, ITAContext context)
        {
            string terminal = context.GetTerminal();
            string tkey = string.Format("Token{0}_{1}", account.Id.Value, terminal);
            Data_ServerTokenInfo savetk = new Data_ServerTokenInfo();
            savetk.OrgId = account.OrgId.GetValueOrDefault(0);
            savetk.UserId = account.Id.Value;
            savetk.Avatar = account.Avatar;
            savetk.UserName = string.IsNullOrEmpty(account.RealName) ? account.UserName : account.RealName;
            savetk.Ipaddr = IpHelper.GetIpAddr(context.Request);
            savetk.Location = await context.GetIpLocation(savetk.Ipaddr);
            UserAgent ua = UserAgentHelper.Parse(context.Request.UserAgent);
            savetk.Browser = ua.Browser;
            savetk.OSName = ua.Platform;
            savetk.LoginTime = loginTime;
            savetk.Terminal = terminal;


            DateTime expireTime = DateTime.Now.AddMinutes(_conf.Value.expire_minutes);
            savetk.Expire = MyAccess.Core.TypeConvert.Time2Unix(expireTime);
            await _provider.GetService<GeneralRedisHelper>().StringSetAsync(tkey, savetk, expireTime - DateTime.Now);
            return savetk;
        }
    
      
    }
}
