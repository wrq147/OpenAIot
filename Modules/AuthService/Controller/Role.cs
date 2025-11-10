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
    [About]
    public class Role : AbstractLoginedController
    {
        private PermissionBLL _permission;
        private UserBLL _user;
        private DeptBLL _dept;
        public Role(PermissionBLL permission, UserBLL user, DeptBLL dept)
        {
            _permission = permission;
            _user = user;
            _dept = dept;
        }
        [HttpGet]
        public async Task<AjaxResult> List(In_RoleList query)
        {
            return this.Success(await _user.SelectRoleList(query));
        }

        /// <summary>
        /// 查询已分配用户角色列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("List")]
        [HttpGet]
        public async Task<AjaxResult> AllocatedList(In_UserRoleList query)
        {
            return this.Success(await _user.SelectAllocatedList(query));
        }


        /// <summary>
        /// 查询未分配用户角色列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("List")]
        [HttpGet]
        public async Task<AjaxResult> UnallocatedList(In_UserRoleList query)
        {
            return this.Success(await _user.SelectUnallocatedList(query));
        }

        [About]
        [HttpGet]
        public async Task<IResult> Export(In_RoleList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_Role> page = await _user.SelectRoleList(query);
                List<MZ_Role> list = page.List;
                Dictionary<string, ParamRenderToExcel<MZ_Role>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_Role>>();
                FiedNames.Add("RoleID", new ParamRenderToExcel<MZ_Role>("编号"));
                FiedNames.Add("RoleName", new ParamRenderToExcel<MZ_Role>("角色名称"));
                FiedNames.Add("RoleDesc", new ParamRenderToExcel<MZ_Role>("角色备注"));
                FiedNames.Add("Status", new ParamRenderToExcel<MZ_Role>("状态"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_Role>("角色表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _user.GetRole(id));
        }
        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> GetRolePermission(long id)
        {
            return (await _permission.GetRolePermissions(id)).ToAjaxResult();
        }



        /// <summary>
        /// 加载对应角色部门列表树
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RoleScopeTreeSelect(long id, long menuId)
        {
            List<MZ_Dept> depts = await _dept.SelectDeptList(new MZ_Dept());
            List<TreeSelect<long>> tlist = MZ_Dept.DeptList2Tree(_dept.BuildDeptTree(depts));
            return this.Success(new
            {
                scope = await _permission.SelectRoleScope(id, menuId),
                depts = tlist
            });
        }

        [About]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Role role)
        {
            role.SetCreateBy(GetUser());
            return (await _user.AddRole(role)).ToAjaxResult();
        }
        [About]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Role role)
        {
            role.SetUpdateBy(GetUser());
            return (await _user.UpdateRole(role)).ToAjaxResult();
        }

        [About("Edit")]
        [HttpPost]
        public async Task<AjaxResult> EditScope(MZ_RoleScope scope)
        {
            return (await _permission.UpdateRoleScope(scope)).ToAjaxResult();
        }

        [About]
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _user.DeleteRole(id)).ToAjaxResult();
        }


        /// <summary>
        /// 取消授权用户
        /// </summary>
        /// <param name="SysUserRole"></param>
        /// <returns></returns>
        [About("Edit")]
        [HttpPost]
        public async Task<AjaxResult> Cancel(MZ_UserRole userRole)
        {
            return (await _user.DeleteAuthUser(userRole)).ToAjaxResult();
        }

        /// <summary>
        /// 批量取消授权用户
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [About("Edit")]
        [HttpPost]
        public async Task<AjaxResult> CancelAll(long roleId, long[] userIds)
        {
            return (await _user.DeleteAuthUsers(roleId, userIds)).ToAjaxResult();
        }


        /// <summary>
        /// 批量选择用户授权
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [About("Edit")]
        [HttpPost]
        public async Task<AjaxResult> SelectAll(long roleId, long[] userIds)
        {
            return (await _user.InsertAuthUsers(roleId, userIds)).ToAjaxResult();
        }
    }
}
