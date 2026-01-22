
using Common;
using Common.Share;
using Common.UserAgent;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using FluentMigrator.Infrastructure.Extensions;
using AuthService.Model;
using AuthService.DAL;

namespace AuthService
{
    public class AuthBLL
    {
        private LoginLogDAL _auth;
        private IOptions<GeneralOption> _conf;
        private UserDAL _user;
        private ITAServiceProvider _provider;
        public AuthBLL(ITAServiceProvider provider, LoginLogDAL auth, UserDAL user, IOptions<GeneralOption> conf, OperatorHelper operatorHelper)
        {
            _provider = provider;
            _auth = auth;
            _user = user;
            _conf = conf;
        }


        /// <summary>
        /// 生成图形验证码
        /// </summary>
        /// <returns></returns>
        public virtual BusResponse<object> GenerateCaptchaImage()
        {
            CaptchaImageHelper securityCode = _provider.GetService<CaptchaImageHelper>();
            string code = securityCode.GetRandomEnDigitalText(4);
            byte[] imgbyte = securityCode.GetGifEnDigitalCodeByte(code);
            string base64 = Convert.ToBase64String(imgbyte);
            string id = "CapImg" + Guid.NewGuid().ToString("N");
            TimeSpan ts = DateTime.Now.AddMinutes(2) - DateTime.Now;
            _provider.GetService<GeneralRedisHelper>().StringSet(id, code, ts);
            return BusResponse<object>.Success(new
            {
                uuid = id,
                img = base64
            });
        }
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task Logout(long uid, ITAContext context)
        {
            string terminal = context.GetTerminal();
            var tmpOperator = _provider.GetService<OperatorHelper>();
            await tmpOperator.DeleteServerData(uid, terminal);
        }
        /// <summary>
        /// 系统登录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<Out_Login>> LoginBySys(long id, ITAContext context)
        {
            MZ_AdminInfo account = await _user.GetAdminById(id);
            var rss = await Login(account, context);
            return rss;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="ipt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<Out_Login>> Login(In_Login ipt, ITAContext context)
        {
            int errcount = 0;
            if (_conf.Value.login_need_code)
            {
                var redis = _provider.GetService<GeneralRedisHelper>();
                bool enablecode = true;
                if (ipt.uuid == null || ipt.code == null)
                {
                    //判断是否为前三次登录
                    var tmperrcc = await redis.StringGetAsync<string>("acclog_" + ipt.username);
                    if (tmperrcc == null)
                    {
                        enablecode = false;
                    }
                    else
                    {
                        errcount = int.Parse(tmperrcc);
                        if (errcount < 3)
                        {
                            enablecode = false;
                        }
                        else
                        {
                            return BusResponse<Out_Login>.Error(888, "超过错误次数，请输入验证码");
                        }
                    }
                }
                if (enablecode)
                {
                    string srcode = redis.StringGet(ipt.uuid);
                    if (srcode == null)
                    {
                        return BusResponse<Out_Login>.Error(19, "验证码过期,请重试");
                    }
                    redis.KeyDelete(ipt.uuid);
                    if (!ipt.code.Equals(srcode, StringComparison.OrdinalIgnoreCase))
                    {
                        return BusResponse<Out_Login>.Error(18, "验证码错误");
                    }
                }
            }
            bool isemailLogin = false;
            MZ_AdminInfo account = await _user.GetAdminByName(ipt.username);
            if (account == null)
            {
                account = await _user.GetAdminByMobile(ipt.username);
                if (account == null)
                {
                    account = await _user.GetAdminByActiveEmail(ipt.username);
                    isemailLogin = true;
                }
            }
            if (account == null) return BusResponse<Out_Login>.Error(11, "用户不存在");
            if (account.status == "1") return BusResponse<Out_Login>.Error(12, "用户已被停用");

            string limitmsg = _auth.LimitLoginTime(account.Id.Value);

            if (!string.IsNullOrEmpty(limitmsg))
            {
                return BusResponse<Out_Login>.Error(21, limitmsg);
            }

            ipt.password = MyAccess.Core.Crypter.MD5(string.Concat(ipt.password, account.Salt));
            if (!ipt.password.Equals(account.Password, StringComparison.OrdinalIgnoreCase))
            {
                if (_conf.Value.login_need_code)
                {
                    var redis = _provider.GetService<GeneralRedisHelper>();
                    await redis.StringSetAsync<int>("acclog_" + ipt.username, ++errcount, TimeSpan.FromMinutes(60));
                }
                if (errcount >= 3)
                {
                    return BusResponse<Out_Login>.Error(888, "登录密码错误！");
                }
                else
                {
                    return BusResponse<Out_Login>.Error(999, "登录密码错误！");
                }
            }

            //清除登录验证码限制
            if (_conf.Value.login_need_code)
            {
                var redis = _provider.GetService<GeneralRedisHelper>();
                await redis.KeyDeleteAsync("acclog_" + ipt.username);
            }

            var rss = await Login(account, context);
            if (isemailLogin && account.EmailActive == false)
            {
                return new BusResponse<Out_Login>(6, string.Empty, rss.Data);
            }
            else
            {
                return rss;
            }

        }




        /// <summary>
        /// 校验令牌
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<Data_ServerTokenInfo>> CheckToken(TAAction ac)
        {
            ITAContext context = ac.Context;
            string tk = context.Request.Header[AuthConstant.CONFIG_TOKEN_KEY];
            if (string.IsNullOrEmpty(tk))
            {
                tk = context.Request.Header[AuthConstant.CONFIG_OTHER_TOKEN_KEY];
                if (string.IsNullOrEmpty(tk))
                {
                    return BusResponse<Data_ServerTokenInfo>.Error(400, "您目前是在登出状态");
                }
            }
            string terminal = context.GetTerminal();
            var tmpOperator = _provider.GetService<OperatorHelper>();
            Data_ClientToken ct = tmpOperator.ParseClientToken(tk);
            if (ct == null)
            {
                return BusResponse<Data_ServerTokenInfo>.Error(8, "登录令牌信息错误");
            }

            Data_ServerTokenInfo savetk = await tmpOperator.GetServerData(ct.UserId, terminal);
            if (ct.Mode == TokenMode.Simple)
            {
                //使用简单模式
                if (!tmpOperator.CheckClientToken(ct, _conf.Value.secret_key))
                {
                    return BusResponse<Data_ServerTokenInfo>.Error(50013, "令牌签名校验失败");
                }

                if (savetk == null)
                {
                    //判断是否有强退标记
                    if (await tmpOperator.JudgeRelogin(ct.UserId))
                    {
                        return BusResponse<Data_ServerTokenInfo>.Error(50012, "服务端强制您重新登录");
                    }
                    MZ_AdminInfo account = await _user.GetAdminById(ct.UserId);
                    if (account == null)
                    {
                        return BusResponse<Data_ServerTokenInfo>.Error(50009, "用户不存在");
                    }
                    if (account.status == "1")
                    {
                        return BusResponse<Data_ServerTokenInfo>.Error(50010, "用户已被停用");
                    }
                    if (account.del_flag == "2")
                    {
                        return BusResponse<Data_ServerTokenInfo>.Error(50011, "用户已被删除");
                    }

                    var log = await _provider.GetService<LoginLogDAL>().SelectNearest(ct.UserId, terminal);
                    if (log != null)
                    {
                        DateTime sourT = MyAccess.Core.TypeConvert.Unix2Time(ct.Time);
                        if (Math.Abs((log.CreateDate.Value - sourT).TotalSeconds) > 5)
                        {
                            return BusResponse<Data_ServerTokenInfo>.Error(50012, "其他客户端正在使用本账户登录");
                        }
                    }
                    savetk = await tmpOperator.MakeServerData(account, ct.Time, context);
                }
                else
                {
                    if (savetk.LoginTime != ct.Time)
                    {
                        return BusResponse<Data_ServerTokenInfo>.Error(50012, "其他客户端正在使用本账户登录");
                    }
                }
            }
            else if (ct.Mode == TokenMode.Share)
            {
                if (!tmpOperator.CheckClientToken(ct, _conf.Value.secret_key))
                {
                    return BusResponse<Data_ServerTokenInfo>.Error(50013, "令牌签名校验失败");
                }

                bool hasShare = ac.ActionNode.Method.HasAttribute<ShareCheckAttribute>();
                if (!hasShare)
                {
                    return BusResponse<Data_ServerTokenInfo>.Error(466, "接口不允许共享令牌访问");
                }
                savetk = new Data_ServerTokenInfo();
                savetk.UserId = ct.UserId;

                string[] tarr = ct.Ext.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (tarr.Length > 0)
                {
                    if (tarr.Length > 1)
                    {
                        savetk.DeveloperSecKey = tarr[1];
                    }
                    savetk.OrgId = Convert.ToInt64(tarr[0]);
                }
                else
                {
                    savetk.OrgId = 0;
                }
                savetk.LoginTime = ct.Time;
            }
            else
            {
                //使用刷新令牌模式
                if (savetk == null)
                {
                    if (await tmpOperator.JudgeRelogin(ct.UserId))
                    {
                        return BusResponse<Data_ServerTokenInfo>.Error(50012, "服务端强制您重新登录");
                    }
                    return BusResponse<Data_ServerTokenInfo>.Error(401, "登录令牌过期");
                }

                DateTime exp = MyAccess.Core.TypeConvert.Unix2Time(savetk.Expire);
                //每过3分钟，则刷新令牌
                if ((DateTime.Now - exp.AddMinutes(-_conf.Value.expire_minutes)).TotalMinutes > 3)
                {
                    //自动刷新令牌时，需要验证ip与浏览器信息
                    UserAgent ua = UserAgentHelper.Parse(context.Request.UserAgent);
                    if (ua.Browser == savetk.Browser && ua.Platform == savetk.OSName)
                    {
                        MZ_AdminInfo account = await _user.GetAdminById(ct.UserId);
                        if (account == null) return BusResponse<Data_ServerTokenInfo>.Error(50009, "用户不存在");
                        if (account.status == "1") return BusResponse<Data_ServerTokenInfo>.Error(50010, "用户已被停用");
                        if (account.del_flag == "2") return BusResponse<Data_ServerTokenInfo>.Error(50011, "用户已被删除");
                        savetk = await tmpOperator.MakeServerData(account, savetk.LoginTime, context);
                    }
                }

                if (!tmpOperator.CheckClientToken(ct, _conf.Value.secret_key))
                {
                    return BusResponse<Data_ServerTokenInfo>.Error(50013, "令牌签名校验失败");
                }
                if (savetk.LoginTime != ct.Time)
                {
                    return BusResponse<Data_ServerTokenInfo>.Error(50012, "其他客户端正在使用本账户登录");
                }
            }
            return BusResponse<Data_ServerTokenInfo>.Success(savetk);
        }
        /// <summary>
        /// 校验权限
        /// </summary>
        /// <param name="savetk"></param>
        /// <param name="permis"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<Data_ServerTokenInfo>> CheckPermis(Data_ServerTokenInfo savetk, string permis)
        {
            //关联action未定义，则不进行权限验证
            if (!string.IsNullOrEmpty(permis))
            {
                //验证权限
                string[] permArr = permis.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (string perm in permArr)
                {
                    var rolePermis = await GetPermis(savetk.UserId, savetk.OrgId, savetk.LoginTime);
                    if (rolePermis.Contains(perm))
                    {
                        return BusResponse<Data_ServerTokenInfo>.Success(savetk);
                    }
                }
                return BusResponse<Data_ServerTokenInfo>.Error(403, "你的权限不足，请联系管理人员分配权限");
            }
            else
            {
                return BusResponse<Data_ServerTokenInfo>.Success(savetk);
            }
        }
        /// <summary>
        /// 刷新令牌（客户端用）
        /// </summary>
        /// <param name="refreshtk"></param>
        /// <param name="clientToken"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<Out_Login>> RefreshToken(string refreshtk, string clientToken, ITAContext context)
        {
            if (string.IsNullOrEmpty(refreshtk))
            {
                return BusResponse<Out_Login>.Error(7, "刷新令牌信息错误");
            }
            var tmpOperator = _provider.GetService<OperatorHelper>();
            Data_ClientToken ct = tmpOperator.ParseClientToken(clientToken);
            if (ct == null)
            {
                return BusResponse<Out_Login>.Error(8, "登录令牌信息错误");
            }
            MZ_AdminInfo account = await _user.GetAdminById(ct.UserId);
            if (account == null) return BusResponse<Out_Login>.Error(11, "用户不存在");
            if (account.status == "1") return BusResponse<Out_Login>.Error(12, "用户已被停用");

            string ip = IpHelper.GetIpAddr(context.Request);
            string mkrefreshtk = tmpOperator.MakeRefreshToken(clientToken, ip + context.Request.UserAgent);
            if (!mkrefreshtk.Equals(refreshtk))
            {
                return BusResponse<Out_Login>.Error(433, "刷新令牌失效,请重新登录");
            }


            var stinfo = await tmpOperator.MakeServerData(account, MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now), context);
            //新建令牌
            Out_Login lgdata = new Out_Login();
            lgdata.token = Data_ClientToken.MakeClientToken(account.Id.Value, stinfo.LoginTime, _conf.Value.secret_key, TokenMode.Authorization);
            lgdata.refresh_token = tmpOperator.MakeRefreshToken(lgdata.token, ip + context.Request.UserAgent);
            return BusResponse<Out_Login>.Success(lgdata);
        }

        /// <summary>
        /// 获取功能权限
        /// </summary>
        public virtual async Task<HashSet<string>> GetPermis(long uid, long org, long loginTime)
        {
            var tmpCache = _provider.GetService<CacheHelper>();
            string cacheKey = "$$Perm" + uid + "$" + org + "$" + loginTime;
            var _permis = tmpCache.GetCache<HashSet<string>>(cacheKey);
            if (_permis == null)
            {
                _permis = await _provider.GetService<PermissionBLL>().GetUserRolePermissions(uid, org);
                tmpCache.SetCache(cacheKey, _permis, DateTime.Now.AddSeconds(180));
            }
            return _permis;
        }

        /// <summary>
        /// 获取客户端所需登录信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<Out_LoginUserInfo>> GetLoginUserInfo(MZ_AdminInfo info, Data_ServerTokenInfo user)
        {
            Out_LoginUserInfo usrInfo = new Out_LoginUserInfo();
            usrInfo.Id = info.Id.Value;
            usrInfo.OrgId = info.OrgId.Value;
            usrInfo.name = info.RealName;
            usrInfo.introduction = info.Introduction;
            usrInfo.avatar = info.Avatar;
            usrInfo.Sex = info.Sex;
            usrInfo.mobile = info.Mobile;
            usrInfo.permissions = await GetPermis(user.UserId,user.OrgId,user.LoginTime);
            usrInfo.extObj = new Dictionary<string, string>();

            //初始化扩展信息
            var extlist = await _provider.GetService<AdminExtDAL>().SelectList(x => x.UserId == info.Id);
            foreach (var ext in extlist)
            {
                usrInfo.extObj.Add(ext.ExtField, ext.ExtValue);
            }
            return BusResponse<Out_LoginUserInfo>.Success(usrInfo);
        }
        /// <summary>
        /// 登录的通用代码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<Out_Login>> Login(MZ_AdminInfo account, ITAContext context)
        {
            //先清除强退标记
            await _provider.GetService<GeneralRedisHelper>().KeyDeleteAsync("ForceRelogin:" + account.Id.Value);
            var operatorHelper = _provider.GetService<OperatorHelper>();
            Data_ServerTokenInfo stinfo = await operatorHelper.MakeServerData(account, MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now), context);
            await _provider.GetService<PermissionBLL>().ClearUserRolePermissions(account.Id.Value, stinfo.OrgId);

            //新建令牌
            MZ_LoginLog log = new MZ_LoginLog();
            log.UserId = account.Id.Value;
            log.CreateDate = DateTime.Now;
            log.Status = 0;
            log.IPAddress = stinfo.Ipaddr;
            log.IPLocation = stinfo.Location;
            log.Browser = stinfo.Browser;
            log.OS = stinfo.OSName;
            log.Terminal = stinfo.Terminal;
            log.Info = "登录成功";
            await _provider.GetService<LoginLogDAL>().AddLoginLog(log);
            var extii = await GetLoginUserInfo(account, stinfo);
            Out_Login lgdata = new Out_Login();
            lgdata.token = Data_ClientToken.MakeClientToken(account.Id.Value, stinfo.LoginTime, _conf.Value.secret_key, _conf.Value.enable_simple_token ? TokenMode.Simple : TokenMode.Authorization);
            lgdata.refresh_token = operatorHelper.MakeRefreshToken(lgdata.token, stinfo.Ipaddr + context.Request.UserAgent);
            lgdata.ext_info = extii.Data;
            return BusResponse<Out_Login>.Success(lgdata);
        }
    }
}
