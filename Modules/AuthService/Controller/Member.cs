using AuthService.Model;
using Common;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;
namespace AuthService.Controller
{
    /// <summary>
    /// 后台管理员工管理用API
    /// </summary>
    public class Member : AbstractLoginedController
    {
        private UserBLL _usrBLL;
        private OrgBLL _org;
        public Member(UserBLL usr, OrgBLL org)
        {
            _usrBLL = usr;
            _org = org;
        }
        /// <summary>
        /// 导出用户
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("/AuthService/Member/List")]
        [HttpGet]
        public async Task<IResult> Export(In_UserList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_AdminInfo> page = await _usrBLL.GetUserList(query);
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
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> List(In_UserList query)
        {
            return this.Success(await _usrBLL.GetUserList(query));
        }

        /// <summary>
        /// 获取员工选择列表树
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Tree(In_MemberTreeList query = null)
        {
            if (query == null) query = new In_MemberTreeList();
            var user = Data_ServerTokenInfo.From(Context);
            query.orgId = user.OrgId;
            return this.Success(await _org.GetTree(query));
        }
        /// <summary>
        /// 搜索员工
        /// </summary>
        /// <param name="realName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Search(string realName)
        {
            return this.Success(await _org.SearchUserList(realName));
        }
      
        /// <summary>
        /// 邀请指定用户加入当前企业
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AuthService/Member/YaoQing")]
        [HttpPost]
        public async Task<AjaxResult> Add(In_JoinUser data)
        {
            var user = GetUser();
            return (await _usrBLL.Join(data, user.OrgId)).ToAjaxResult();
        }
        /// <summary>
        /// 修改用户的企业信息
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        [HttpPost]
        [About]
        public async Task<AjaxResult> Edit(MZ_User_Org uo)
        {
            return (await _org.EditUserOrg(uo)).ToAjaxResult();
        }
        /// <summary>
        /// 给员工授权角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [About("/AuthService/Member/Edit")]
        [HttpPost]
        public async Task<AjaxResult> UpdateAuthRole(long userId, long[] roleIds)
        {
            return (await _usrBLL.SetUserRoles(userId, roleIds)).ToAjaxResult();
        }
        /// <summary>
        /// 获取指定用户的部门
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [About("/AuthService/Member/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_UserDept>>> UserDepts(long userId, long orgId)
        {
            return this.Success(await _org.SelectUserDept(userId, orgId));
        }
        /// <summary>
        /// 员工重置密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/AuthService/Member/Edit")]
        public async Task<AjaxResult> ResetPwd(long userId, string password)
        {
            if (!await _org.IsMember(userId))
            {
                return this.Error<string>(21, "无法重置非企业员工密码");
            }
            return (await _usrBLL.ResetPwd(userId, password, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 添加分身
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="deptId"></param>
        /// <returns></returns>
        [About("/AuthService/Member/Edit")]
        [HttpPost]
        public async Task<AjaxResult> AddClone(long uid, long deptId)
        {
            return (await _org.AddClone(uid, deptId)).ToAjaxResult();
        }
        /// <summary>
        /// 删除分身
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="deptId"></param>
        /// <returns></returns>
        [About("/AuthService/Member/Edit")]
        [HttpPost]
        public async Task<AjaxResult> DelClone(long uid, long deptId)
        {
            return (await _org.DelClone(uid, deptId)).ToAjaxResult();
        }
        /// <summary>
        /// 从当前企业中移除员工
        /// </summary>
        /// <param name="id">目标员工Id</param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            var user = GetUser();
            if (user.UserId == id)
            {
                return this.Error<string>(12, "禁止移除本人");
            }

            return (await _org.DeleteUser(id)).ToAjaxResult();
        }

        /// <summary>
        /// 生成邀请码
        /// </summary>
        /// <returns></returns>
        [About("/AuthService/Member/YaoQing")]
        [HttpGet]
        public async Task<AjaxResult> InviteLink()
        {
            return (await _org.GenerateInvitLink()).ToAjaxResult();
        }

    }
}
