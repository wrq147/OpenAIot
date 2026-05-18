using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Common.Json;
using Common.Share;
using Microsoft.Extensions.Options;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace AuthService
{
    public class Data_ServerTokenInfo : IUserInfo
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public long Expire { get; set; }
        /// <summary>
        /// 所属组织
        /// </summary>
        public long OrgId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 真实姓名或用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string Ipaddr { get; set; }
        /// <summary>
        /// 登录地址
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 客户端浏览器
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 客户端操作系统
        /// </summary>
        public string OSName { get; set; }
        /// <summary>
        /// 客户端终端
        /// </summary>
        public string Terminal { get; set; }
        /// <summary>
        /// 分享令牌传的开发者密钥
        /// </summary>
        public string DeveloperSecKey { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public long LoginTime { get; set; }

        private DataScope _scope;
        private string _remoteToken;
        private string _remoteScopeUrl;
        private string _userAgent;
        public void SetRemoteInfo(string tk, string scopeUrl, string userAgent)
        {
            _remoteToken = tk;
            _remoteScopeUrl = scopeUrl;
            _userAgent = userAgent;
        }

        /// <summary>
        /// 当前操作数据权限
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="permis"></param>
        /// <returns></returns>
        public async Task<DataScope> GetScope(ITAServiceProvider provider, string permis = null)
        {
            if (_scope == null)
            {
                if (string.IsNullOrEmpty(permis))
                {
                    permis = TAAction.Current.ActionNode.AboutCode;
                }
                if (string.IsNullOrEmpty(permis))
                {
                    return null;
                }
                List<DataScope> scopeList = null;
                if (string.IsNullOrEmpty(_remoteToken))
                {
                    //本地获取数据权限
                    string[] permArr = permis.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    scopeList = new List<DataScope>();
                    foreach (string perm in permArr)
                    {
                        var dataPermiss = await provider.GetService<PermissionBLL>().GetUserScope(perm, UserId, OrgId);
                        scopeList.AddRange(dataPermiss);
                    }
                }
                else
                {
                    //远程获取数据权限
                    var httpReq = new HttpRequestMessage(HttpMethod.Get, _remoteScopeUrl + "?permis=" + Uri.EscapeDataString(permis));
                    httpReq.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
                    httpReq.Headers.TryAddWithoutValidation(AuthConstant.CONFIG_TOKEN_KEY, _remoteToken);

                    string rt = await HttpHelper.Instance.SendAsync(httpReq);
                    var scopers = System.Text.Json.JsonSerializer.Deserialize<DefaultAjaxResult<List<DataScope>>>(rt, MyDefaultTextJsonConfig.DefaultOptions);
                    if (scopers.code == 0)
                    {
                        scopeList = scopers.data;
                    }
                    else
                    {
                        return null;
                    }
                }
                if (scopeList == null || scopeList.Count == 0)
                {
                    return null;
                }
                _scope = DataScope.Contact(scopeList);
            }
            return _scope;
        }

        /// <summary>
        /// 转成分享令牌（分享令牌只能操作GET接口）
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="extInfo"></param>
        /// <returns></returns>
        public string ToShareToken(ITAServiceProvider provider, string extInfo)
        {
            var conf = provider.GetService<IOptions<GeneralOption>>();
            string secreKey = conf.Value.secret_key;
            return Data_ClientToken.MakeClientToken(this.UserId, MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now), secreKey, TokenMode.Share, this.OrgId.ToString() + "," + extInfo);
        }
        /// <summary>
        /// 从上下文获取服务端登录数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Data_ServerTokenInfo From(ITAContext context)
        {
            return context.Items["AuthToken"] as Data_ServerTokenInfo;
        }
        /// <summary>
        /// 上下文赋值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public static void To(ITAContext context, Data_ServerTokenInfo data)
        {
            context.Items["AuthToken"] = data;
        }
    }
}
