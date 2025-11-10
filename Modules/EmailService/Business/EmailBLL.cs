using AuthService;
using AuthService.Model;
using Common;
using Common.Share;
using EmailService.Model;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;

namespace EmailService.Business
{
    public class EmailBLL
    {
        private IOptions<GeneralOption> _options;
        private ITAServiceProvider _provider;
        private UserDAL _userDAL;
        public EmailBLL(ITAServiceProvider provider, IOptions<GeneralOption> options, UserDAL userDAL)
        {
            _provider = provider;
            _options = options;
            _userDAL = userDAL;
        }
        private async Task<EmailNote> GetEmailNode(long uid)
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            return await redis.StringGetAsync<EmailNote>("email:" + uid);
        }
        private async Task SetEmailNode(EmailNote node)
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            await redis.StringSetAsync<EmailNote>("email:" + node.UserId, node, TimeSpan.FromMinutes(2));
        }
        public virtual async Task<BusResponse<string>> BindEmail(string note, long uid)
        {
            EmailNote noteEntity = await GetEmailNode(uid);
            if (noteEntity == null)
            {
                return BusResponse<string>.Error(113, "验证码已过期，请重新发送！");
            }
            if (noteEntity.Num > 10)
            {
                return BusResponse<string>.Error(114, "验证码错误次数太多,请重新获取");
            }
            if (!string.Equals(note, noteEntity.Message, StringComparison.OrdinalIgnoreCase))
            {
                ++noteEntity.Num;
                await SetEmailNode(noteEntity);
                return BusResponse<string>.Error(115, "验证码错误");
            }

            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string userEmailLock = "UserEmailLock" + noteEntity.Email;
            if (await redis.WaitLockTakeAsync(userEmailLock))
            {
                try
                {
                    MZ_AdminInfo info = await _userDAL.Select(uid);
                    if (info == null)
                    {
                        return BusResponse<string>.Error(116, "用户不存在");
                    }

                    MZ_AdminInfo newUpdate = new MZ_AdminInfo();
                    newUpdate.Id = uid;
                    newUpdate.Email = noteEntity.Email;
                    newUpdate.EmailActive = true;
                    await _userDAL.UpdateUser(newUpdate);

                    return BusResponse<string>.Success();
                }
                finally
                {
                    redis.LockRelease(userEmailLock);
                }
            }
            else
            {
                return BusResponse<string>.ErrorBusy();
            }
        }
        public virtual async Task<BusResponse<string>> SendCode(string email, string content, IUserInfo user)
        {
            if (!email.IsEmail())
            {
                return BusResponse<string>.Error(112, "邮箱格式错误");
            }
            long uid;
            if (user != null)
            {
                uid = user.UserId;
            }
            else
            {
                var adminInfo = await _userDAL.GetAdminByActiveEmail(email);
                if (adminInfo == null)
                {
                    return BusResponse<string>.Error(111, "用户不存在");
                }
                uid = adminInfo.Id.Value;
            }

            //生成随机数
            string note = new Random().Next(0, 100000).ToString().PadLeft(6, '0');
            EmailNote noteEntity = await GetEmailNode(uid);
            if (noteEntity == null)
            {
                noteEntity = new EmailNote();
                noteEntity.UserId = uid;
                noteEntity.UpdatedOn = DateTime.Now;
                noteEntity.Message = note;
                noteEntity.Num = 1;
                noteEntity.Email = email;
            }
            else
            {
                TimeSpan ts1 = DateTime.Now - noteEntity.UpdatedOn;
                if (ts1.Minutes < 2)
                {
                    return BusResponse<string>.Error(113, "2分钟内不能重复发邮件");
                }
                noteEntity.UpdatedOn = DateTime.Now;
                noteEntity.Email = email;
                noteEntity.Message = note;
                noteEntity.Num = 1;
            }
            await SetEmailNode(noteEntity);

            TemplateDocument td = new TemplateDocument(content);
            SimpleTemplateContext tdContext = new SimpleTemplateContext();
            tdContext.PushGlobal("code", note);
            if (user != null)
            {
                string tmpurl = string.IsNullOrEmpty(_options.Value.url) ? ("http://" + IpHelper.GetAvaOutIp()) : _options.Value.url;
                string linkurl = tmpurl + "/EmailService/Visitor/ActiveEmail?note=" + note + "&uid=" + uid;
                tdContext.PushGlobal("link", linkurl);
            }
            else
            {
                tdContext.PushGlobal("link", string.Empty);
            }
            string tdhtml = td.MakeHtml(tdContext);

            var emailSender = _provider.GetService<EmailSenderHelper>();
            var emailoption = await emailSender.GetEmailConfig();
            await emailSender.SendHtmlEmail(email, emailoption.email_bind_title, tdhtml);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<Out_Login>> Login(In_LoginEmail ipt, ITAContext context)
        {
            MZ_AdminInfo account = await _userDAL.GetAdminByActiveEmail(ipt.email);
            if (account == null)
            {
                return BusResponse<Out_Login>.Error(101, "用户不存在");
            }
            EmailNote noteEntity = await GetEmailNode(account.Id.Value);
            if (noteEntity == null)
            {
                return BusResponse<Out_Login>.Error(102, "验证码已过期，请重新发送！");
            }
            if (noteEntity.Num > 10)
            {
                return BusResponse<Out_Login>.Error(103, "验证码错误次数太多,请重新获取");
            }
            if (!string.Equals(ipt.code, noteEntity.Message))
            {
                ++noteEntity.Num;
                await SetEmailNode(noteEntity);
                return BusResponse<Out_Login>.Error(115, "验证码错误");
            }

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
        public virtual async Task<BusResponse<Out_Login>> Reg(In_RegData data, ITAContext context)
        {
            if (string.IsNullOrEmpty(data.RealName))
            {
                return BusResponse<Out_Login>.Error(111, "真实姓名不能为空");
            }
            if (string.IsNullOrEmpty(data.Email))
            {
                return BusResponse<Out_Login>.Error(112, "邮箱不能为空");
            }
            if (!StringHelper.IsEmail(data.Email))
            {
                return BusResponse<Out_Login>.Error(113, "邮箱格式错误");
            }
            if (string.IsNullOrEmpty(data.Password))
            {
                return BusResponse<Out_Login>.Error(114, "密码不能为空");
            }
            if (data.Password.Length < 3 || data.Password.Length > 20)
            {
                return BusResponse<Out_Login>.Error(115, "请输入3-20长度的密码");
            }

            data.Mobile = string.Empty;

            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string userEmailLock = "UserEmailLock" + data.Email;
            if (await redis.WaitLockTakeAsync(userEmailLock))
            {
                try
                {
                    var adminlist = await _userDAL.SelectList(x => x.Email == data.Email && x.del_flag == "0" && x.EmailActive == true);
                    if (adminlist.Count > 0)
                    {
                        return BusResponse<Out_Login>.Error(116, "邮箱已被注册，请通过邮箱找回");
                    }

                    UserBLL userBLL = _provider.GetService<UserBLL>();
                    return await userBLL.Reg(data, context);
                }
                finally
                {
                    redis.LockRelease(userEmailLock);
                }
            }
            else
            {
                return BusResponse<Out_Login>.ErrorBusy();
            }
        }

    }
}
