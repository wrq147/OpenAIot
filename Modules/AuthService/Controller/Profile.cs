using Common;
using Common.EventBus;
using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace AuthService.Controller
{
    /// <summary>
    /// 个人信息 业务处理
    /// </summary>
    public class Profile : AbstractLoginedController
    {
        private OrgBLL _orgBLL;
        private UserBLL _usrBLL;
        private PermissionBLL _permissionBLL;
        private FileHelper _file;
        private IOptions<GeneralOption> _conf;
        public Profile(UserBLL usr, OrgBLL org, PermissionBLL permission, FileHelper file, IOptions<GeneralOption> conf)
        {
            _orgBLL = org;
            _usrBLL = usr;
            _permissionBLL = permission;
            _file = file;
            _conf = conf;
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info()
        {
            Data_ServerTokenInfo usrInfo = GetUser();
            var rolelist = await _permissionBLL.GetRolesListByUser(usrInfo.UserId, usrInfo.OrgId);
            var roleNames = string.Join(',', rolelist.Select(x => x.RoleName));

            return this.Success(new
            {
                user = await _usrBLL.GetUserInfoById(usrInfo.UserId),
                roleGroup = roleNames
            });
        }
        /// <summary>
        /// 当前用户加入的组织列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<MZ_Org>>))]
        public async Task<AjaxResult> OrgList()
        {
            var user = GetUser();
            var tlist = await _orgBLL.SelectUserOrgList(user.UserId);
            return this.Success(tlist);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_AdminInfo user)
        {
            Data_ServerTokenInfo usrInfo = GetUser();
            user.Id = usrInfo.UserId;
            user.Password = null;
            user.SetUpdateBy(usrInfo);
            return (await _usrBLL.UpdateUser(user)).ToAjaxResult();
        }
        /// <summary>
        /// 删除本人账号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Delete()
        {
            return _usrBLL.DeleteUser(GetUser().UserId).ToAjaxResult();
        }
        /// <summary>
        /// 通过旧密码修改密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> UpdatePwd(string oldPassword, string newPassword)
        {
            Data_ServerTokenInfo usrInfo = GetUser();
            MZ_AdminInfo oldUser = await _usrBLL.GetUserInfoById(usrInfo.UserId);
            string oldpassword = MyAccess.Core.Crypter.MD5(string.Concat(oldPassword, oldUser.Salt));
            if (!string.Equals(oldpassword, oldUser.Password))
            {
                return this.Error<string>(12, "修改密码失败，旧密码错误");
            }
            return (await _usrBLL.ResetPwd(usrInfo.UserId, newPassword, usrInfo)).ToAjaxResult();
        }
        /// <summary>
        /// 通过登录返回的PwdCode修改密码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> UpdatePwdByCode(string code, string newPassword)
        {
            string tmpkey = _conf.Value.secret_key;
            string tmpiv = tmpkey.Length > 16 ? tmpkey.Substring(0, 16) : tmpkey;
            string oldpassword = MyAccess.Core.Crypter.DecodeAES(code, tmpkey, tmpiv);
            Data_ServerTokenInfo usrInfo = GetUser();
            MZ_AdminInfo oldUser = await _usrBLL.GetUserInfoById(usrInfo.UserId);
            if (!string.Equals(oldpassword, oldUser.Password))
            {
                return this.Error<string>(12, "无权限修改密码");
            }
            return (await _usrBLL.ResetPwd(usrInfo.UserId, newPassword, usrInfo)).ToAjaxResult();
        }
        /// <summary>
        /// 切换企业
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Switch(long id)
        {
            return (await _orgBLL.Switch(id)).ToAjaxResult();
        }

        /// <summary>
        /// 头像上传（登录后修改头像用）
        /// </summary>
        /// <param name="withDomain">是否带上域名</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Avatar(bool withDomain = false)
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            string fileExt = Path.GetExtension(form.Files[0].FileName);
            if (string.IsNullOrEmpty(fileExt))
            {
                fileExt = _file.ContentTypeToExt(form.Files[0].ContentType);
            }
            string errorType = _conf.Value.limit_file_type.FirstOrDefault(m => m.Equals(fileExt, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(errorType))
            {
                return this.Error<string>(13, "非法的文件类型");
            }
            var file = form.Files[0];
            if (file.ContentLength > _conf.Value.limit_file_size * 1024 * 1024)
            {
                return this.Error<string>(14, string.Format("头像不能超过{0}MB", _conf.Value.limit_file_size));
            }
            string rt = await _file.UploadFile(form.Files[0], withDomain);
            MZ_AdminInfo user = new MZ_AdminInfo();
            Data_ServerTokenInfo usrInfo = GetUser();
            user.Id = usrInfo.UserId;
            user.Avatar = rt;
            user.SetUpdateBy(usrInfo);
            var uprt = await _usrBLL.UpdateUser(user);
            if (uprt.IsSuccess())
            {
                return this.Success(rt);
            }
            else
            {
                return uprt.ToAjaxResult();
            }
        }

    }
}
