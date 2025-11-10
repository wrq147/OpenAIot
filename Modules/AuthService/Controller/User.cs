using TemplateAction.Core;
using Common;
using Common.Share;
using TemplateAction.Label;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.IO;
using TemplateAction.Route;
using AuthService.Model;

namespace AuthService.Controller
{
    /// <summary>
    /// 后台管理用户用API
    /// </summary>
    public class User : AbstractLoginedController
    {
        private AuthBLL _authBLL;
        private UserBLL _usrBLL;
        private PermissionBLL _permissionBLL;
        private OperatorHelper _operator;
        public User(AuthBLL auth, UserBLL usr, PermissionBLL permission, OperatorHelper operatorHelper)
        {
            _authBLL = auth;
            _usrBLL = usr;
            _permissionBLL = permission;
            _operator = operatorHelper;
        }
        /// <summary>
        /// 获取用户的数据权限信息
        /// </summary>
        /// <param name="permis"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<DataScope>>> Scopes(string permis)
        {
            var user = GetUser();
            string[] permArr = permis.Split(',', StringSplitOptions.RemoveEmptyEntries);
            List<DataScope> scopeList = new List<DataScope>();
            foreach (string perm in permArr)
            {
                var dataPermiss = await this.ServiceProvider.GetService<PermissionBLL>().GetUserScope(perm, user.UserId, user.OrgId);
                scopeList.AddRange(dataPermiss);
            }
            return this.Success(scopeList);
        }
        /// <summary>
        /// 全局搜索指定用户
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Search(string key)
        {
            return this.Success(await _usrBLL.SeachUsers(key));
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">指定用户编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id = 0)
        {
            if (id == 0)
            {
                id = GetUser().UserId;
            }
            MZ_AdminInfo usrInfo = await _usrBLL.GetUserInfoById(id);
            if (usrInfo != null)
            {
                usrInfo.roleIds = (await _permissionBLL.GetRoleIdsByUser(id, usrInfo.OrgId.Value)).ToArray();
            }

            return this.Success(new
            {
                user = usrInfo,
                roles = await _permissionBLL.GetRolesListByUser(id, usrInfo.OrgId.Value)
            });
        }
        /// <summary>
        /// 获取当前企业的指定用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> UserInfo(long id)
        {
            MZ_AdminInfo usrInfo = await _usrBLL.GetUserInfoByCurentOrg(id);
            return this.Success(usrInfo);
        }
        /// <summary>
        /// 获取登录用户信息（客户端同步用户信息用）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> LoginInfo()
        {
            var user = GetUser();
            var info = await _usrBLL.GetUserInfoById(user.UserId);
            if (info == null)
            {
                return this.Error<string>(11, "用户信息不存在");
            }
            await _permissionBLL.ClearUserRolePermissions(user.UserId, user.OrgId);
            await _operator.RefreshServerData(info, Context);

            return (await _authBLL.GetLoginUserInfo(info, user)).ToAjaxResult();
        }


        /// <summary>
        /// 获取平台所有用户列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> List(In_UserAllList query)
        {
            return this.Success(await _usrBLL.GetUserAllList(query));
        }
        /// <summary>
        /// 获取系统角色列表
        /// </summary>
        /// <returns></returns>
        [About("/AuthService/User/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_Role>>> SysRoleList()
        {
            return this.Success(await _usrBLL.SelectSystemRoleList());
        }
        /// <summary>
        /// 获取用户的所有角色
        /// </summary>
        /// <returns></returns>
        [About("/AuthService/User/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_UserRole>>> UserRoleList(long uid)
        {
            return this.Success(await _usrBLL.SelectUserRoleList(uid));
        }
        /// <summary>
        /// 获取用户的所有企业
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [About("/AuthService/User/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_Org>>> UserOrgList(long uid)
        {
            var orgBLL = this.ServiceProvider.GetService<OrgBLL>();
            return this.Success(await orgBLL.SelectUserOrgList(uid));
        }
        /// <summary>
        /// 通过账号管理登录用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/AuthService/User/List")]
        [HttpGet]
        public async Task<AjaxResult> LoginBySys(long id)
        {
            BusResponse<Out_Login> response = await _authBLL.LoginBySys(id, Context);
            return response.ToAjaxResult();
        }
        /// <summary>
        /// 导出用户
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<IResult> Export(In_UserAllList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_AdminInfo> page = await _usrBLL.GetUserAllList(query);
                List<MZ_AdminInfo> list = page.List;
                Dictionary<string, ParamRenderToExcel<MZ_AdminInfo>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_AdminInfo>>();
                FiedNames.Add("Id", new ParamRenderToExcel<MZ_AdminInfo>("编号"));
                FiedNames.Add("UserName", new ParamRenderToExcel<MZ_AdminInfo>("账号"));
                FiedNames.Add("RealName", new ParamRenderToExcel<MZ_AdminInfo>("姓名"));
                FiedNames.Add("Avatar", new ParamRenderToExcel<MZ_AdminInfo>("头像"));
                FiedNames.Add("Introduction", new ParamRenderToExcel<MZ_AdminInfo>("备注"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_AdminInfo>("用户表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }
        [About]
        [HttpPost]
        public async Task<AjaxResult> Import(bool updateSupport)
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            Stream st = form.Files[0].OpenReadStream();
            Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
            FiedNames.Add("编号", new ParamImportToList("Id", x => new Guid(x)));
            FiedNames.Add("账号", new ParamImportToList("UserName"));
            FiedNames.Add("姓名", new ParamImportToList("RealName"));
            FiedNames.Add("头像", new ParamImportToList("Avatar"));
            FiedNames.Add("备注", new ParamImportToList("Introduction"));
            List<MZ_AdminInfo> list = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExcelToList<MZ_AdminInfo>(st, FiedNames);
            return (await _usrBLL.ImportUser(list, updateSupport, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        [About("/AuthService/User/Import")]
        [HttpGet]
        public IResult ExportTemplate()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<MZ_AdminInfo>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_AdminInfo>>();
                FiedNames.Add("Id", new ParamRenderToExcel<MZ_AdminInfo>("编号"));
                FiedNames.Add("UserName", new ParamRenderToExcel<MZ_AdminInfo>("账号"));
                FiedNames.Add("RealName", new ParamRenderToExcel<MZ_AdminInfo>("姓名"));
                FiedNames.Add("Avatar", new ParamRenderToExcel<MZ_AdminInfo>("头像"));
                FiedNames.Add("Introduction", new ParamRenderToExcel<MZ_AdminInfo>("备注"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_AdminInfo>("用户表", new List<MZ_AdminInfo>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        [About]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_AdminInfo user)
        {
            user.SetCreateBy(GetUser());
            return (await _usrBLL.AddUser(user)).ToAjaxResult();
        }
        [About]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_AdminInfo user)
        {
            user.SetUpdateBy(GetUser());
            return (await _usrBLL.UpdateUser(user)).ToAjaxResult();
        }
        [About]
        [HttpGet]
        public AjaxResult Remove(long id)
        {
            return _usrBLL.DeleteUser(id).ToAjaxResult();
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        public async Task<AjaxResult> ResetPwd(long userId, string password)
        {
            return (await _usrBLL.ResetPwd(userId, password, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [About("/AuthService/User/Edit")]
        [HttpPost]
        public async Task<AjaxResult> ChangeStatus(long userId, string status)
        {
            return (await _usrBLL.UpdateUserStatus(userId, status, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 为用户添加系统角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [About("/AuthService/User/Edit")]
        [HttpPost]
        public async Task<AjaxResult> AddSysRole(long userId, long roleId, long orgId)
        {
            return (await _usrBLL.AddSysRole(userId, roleId, orgId)).ToAjaxResult();
        }

        /// <summary>
        /// 删除用户系统角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [About("/AuthService/User/Edit")]
        [HttpPost]
        public async Task<AjaxResult> DelSysRole(long userId, long roleId, long orgId)
        {
            return (await _usrBLL.DelSysRole(userId, roleId, orgId)).ToAjaxResult();
        }
        /// <summary>
        /// 根据用户编号获取授权角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> AuthRole(long id)
        {
            var user = GetUser();
            return this.Success(new
            {
                user = await _usrBLL.GetUserInfoById(id),
                roles = await _permissionBLL.GetRolesListByOrgId(user.OrgId),
                roleIds = await _permissionBLL.GetRoleIdsByUser(id, user.OrgId)
            });
        }


        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Logout()
        {
            await _authBLL.Logout(GetUser().UserId, Context);
            return this.Success<string>();
        }
    }
}
