using AuthService;
using AuthService.Model;
using Common;
using Common.EventBus;
using Common.Share;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SMSService.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace SMSService.Business
{
    /// <summary>
    /// 短信验证码处理服务
    /// </summary>
    public class SmsCodeBLL
    {
        private ITAServiceProvider _provider;
        private UserDAL _userDAL;
        public SmsCodeBLL(ITAServiceProvider provider, UserDAL userDAL)
        {
            _provider = provider;
            _userDAL = userDAL;
        }

        public async Task<SmsNote> GetPhoneNode(string phone)
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            return await redis.StringGetAsync<SmsNote>("sms_" + phone);
        }
        public async Task SetPhoneNode(SmsNote node)
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            await redis.StringSetAsync<SmsNote>("sms_" + node.Phone, node, TimeSpan.FromMinutes(5));
        }
        /// <summary>
        /// 生成验证令牌,10分钟内有效
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> MakeImgToken(string code)
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            string tkey = "Img" + Guid.NewGuid().ToString("N");
            await redis.StringSetAsync(tkey, code, TimeSpan.FromMinutes(10));
            return tkey;
        }

        /// <summary>
        /// 验证码验证
        /// </summary>
        /// <param name="tk"></param>
        /// <param name="code"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private async Task<bool> _CheckImgToken(string tk, string code, string ip)
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            if (string.IsNullOrEmpty(tk))
            {
                string top = await redis.StringGetAsync<string>("ip" + ip);
                if (string.IsNullOrEmpty(top))
                {
                    await redis.StringSetAsync("ip" + ip, "yes", TimeSpan.FromMinutes(5));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            string tkcode = await redis.StringGetAsync<string>(tk);
            if (tkcode != null && string.Compare(tkcode, code, true) == 0)
            {
                return true;
            }
            redis.KeyDelete(tk);
            return false;
        }
        public async Task<BusResponse<string>> SendCode(string phone, string code, string imgid, ITAContext context)
        {
            if (!phone.IsMobile())
            {
                return BusResponse<string>.Error(110, "手机格式错误");
            }

            string ip = MyAccess.Core.Crypter.MD5(context.Request.UserAgent);
            //生成随机数
            string note = new Random().Next(0, 100000).ToString().PadLeft(6, '0');

            if (!await _CheckImgToken(imgid, code, ip))
            {
                return BusResponse<string>.Error(2, "当前ip发送次数太多,需要图形验证码");
            }

            SmsNote noteEntity = await GetPhoneNode(phone);
            if (noteEntity == null)
            {
                noteEntity = new SmsNote();
                noteEntity.UpdatedOn = DateTime.Now;
                noteEntity.Message = note;
                noteEntity.Num = 1;
                noteEntity.Phone = phone;
            }
            else
            {
                TimeSpan ts1 = DateTime.Now - noteEntity.UpdatedOn;
                if (ts1.Minutes < 2)
                {
                    return BusResponse<string>.Error(103, "2分钟内不能重复发短信");
                }
                noteEntity.UpdatedOn = DateTime.Now;
                noteEntity.Phone = phone;
                noteEntity.Message = note;
                noteEntity.Num = 1;
            }
            await SetPhoneNode(noteEntity);
            //发送短信
            TargetUser[] targetUsers = new TargetUser[1];
            targetUsers[0] = new TargetUser();
            targetUsers[0].phone = phone;
            var nt = new NoticeEvent(2, targetUsers, new string[] { "SMS" });
            nt.TargetType = "验证码";
            nt.Content = note;
            await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);

            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<Out_Login>> Login(In_LoginSMS ipt, ITAContext context)
        {
            SmsNote smsNode = await GetPhoneNode(ipt.tel);
            if (smsNode == null)
            {
                return BusResponse<Out_Login>.Error(102, "短信验证码过期");
            }
            if (smsNode.Num > 10)
            {
                return BusResponse<Out_Login>.Error(103, "验证码错误次数太多,请重新获取");
            }
            if (!string.Equals(ipt.code, smsNode.Message, StringComparison.OrdinalIgnoreCase))
            {
                ++smsNode.Num;
                await SetPhoneNode(smsNode);
                return BusResponse<Out_Login>.Error(104, "短信验证码错误");
            }

            MZ_AdminInfo account = await _userDAL.GetAdminByMobile(ipt.tel);
            if (account == null) return BusResponse<Out_Login>.Error(11, "用户不存在");
            if (account.status == "1") return BusResponse<Out_Login>.Error(12, "用户已被停用");

            var authDAL = _provider.GetService<LoginLogDAL>();
            string limitmsg = authDAL.LimitLoginTime(account.Id.Value);

            if (!string.IsNullOrEmpty(limitmsg))
            {
                return BusResponse<Out_Login>.Error(21, limitmsg);
            }
            var loginInfo = await _provider.GetService<AuthBLL>().Login(account, context);
            var option = _provider.GetService<IOptions<GeneralOption>>();
            string tmpkey = option.Value.secret_key;
            string tmpiv = tmpkey.Length > 16 ? tmpkey.Substring(0, 16) : tmpkey;
            string passwordcode = MyAccess.Core.Crypter.EncodeAES(account.Password, tmpkey, tmpiv);
            loginInfo.Data.ext_info.extObj.Add("UpdatePasswordCode", passwordcode);
            return loginInfo;
        }

        public async Task<BusResponse<Out_Login>> Reg(In_RegTelData data, ITAContext context)
        {
            if (string.IsNullOrEmpty(data.RealName))
            {
                return BusResponse<Out_Login>.Error(111, "真实姓名不能为空");
            }
            if (string.IsNullOrEmpty(data.Mobile))
            {
                return BusResponse<Out_Login>.Error(112, "手机号不能为空");
            }
            if (!StringHelper.IsMobile(data.Mobile))
            {
                return BusResponse<Out_Login>.Error(113, "手机格式错误");
            }
            if (string.IsNullOrEmpty(data.Password))
            {
                return BusResponse<Out_Login>.Error(114, "密码不能为空");
            }
            if (data.Password.Length < 3 || data.Password.Length > 20)
            {
                return BusResponse<Out_Login>.Error(115, "请输入3-20长度的密码");
            }

            data.Email = string.Empty;

            SmsNote smsNode = await GetPhoneNode(data.Mobile);
            if (smsNode == null)
            {
                return BusResponse<Out_Login>.Error(102, "短信验证码过期");
            }
            if (smsNode.Num > 10)
            {
                return BusResponse<Out_Login>.Error(103, "验证码错误次数太多,请重新获取");
            }
            if (!string.Equals(data.MobileCode, smsNode.Message, StringComparison.OrdinalIgnoreCase))
            {
                ++smsNode.Num;
                await SetPhoneNode(smsNode);
                return BusResponse<Out_Login>.Error(104, "短信验证码错误");
            }
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string userMobileLock = "UserMobileLock" + data.Mobile;
            if (await redis.WaitLockTakeAsync(userMobileLock))
            {
                try
                {
                    MZ_AdminInfo info = await _userDAL.GetAdminByMobile(data.Mobile);
                    if (info != null)
                    {
                        return BusResponse<Out_Login>.Error(116, "手机号已被注册");
                    }

                    UserBLL userBLL = _provider.GetService<UserBLL>();
                    return await userBLL.Reg(data, context);
                }
                finally
                {
                    redis.LockRelease(userMobileLock);
                }
            }
            else
            {
                return BusResponse<Out_Login>.ErrorBusy();
            }
        }
        public async Task<BusResponse<string>> Bind(string phone, string code, IUserInfo user)
        {
            if (!phone.IsMobile())
            {
                return BusResponse<string>.Error(110, "手机格式错误");
            }
            SmsNote smsNode = await GetPhoneNode(phone);
            if (smsNode == null)
            {
                return BusResponse<string>.Error(102, "短信验证码过期");
            }
            if (smsNode.Num > 10)
            {
                return BusResponse<string>.Error(103, "验证码错误次数太多,请重新获取");
            }
            if (!string.Equals(code, smsNode.Message, StringComparison.OrdinalIgnoreCase))
            {
                ++smsNode.Num;
                await SetPhoneNode(smsNode);
                return BusResponse<string>.Error(104, "短信验证码错误");
            }
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string userMobileLock = "UserMobileLock" + phone;
            if (await redis.WaitLockTakeAsync(userMobileLock))
            {
                try
                {
                    MZ_AdminInfo info = await _userDAL.GetAdminByMobile(phone);
                    if (info != null && user.UserId != info.Id)
                    {
                        return BusResponse<string>.Error(116, "手机号已被绑定");
                    }

                    MZ_AdminInfo newUpdate = new MZ_AdminInfo();
                    newUpdate.Id = user.UserId;
                    newUpdate.Mobile = phone;
                    await _userDAL.UpdateUser(newUpdate);

                    return BusResponse<string>.Success();
                }
                finally
                {
                    redis.LockRelease(userMobileLock);
                }
            }
            else
            {
                return BusResponse<string>.ErrorBusy();
            }
        }



    }
}
