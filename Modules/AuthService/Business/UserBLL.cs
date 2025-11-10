using AuthService.Model;
using Common;
using Common.IdGenerator;
using Common.Share;
using MyAccess.Aop;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemplateAction.Core;
using System.Transactions;
using AuthService.Controller;
using StackExchange.Redis;

namespace AuthService
{
    public class UserBLL
    {
        private UserDAL _user;
        private PermissionDAL _permission;
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private DeptDAL _deptDAL;
        private ConfigBLL _configBLL;
        private OrgDAL _org;
        public UserBLL(ITAServiceProvider provider, PermissionDAL permission, ConfigBLL configBLL, UserDAL user, SnowflakeHelper snowflake, DeptDAL deptDAL, OrgDAL org)
        {
            _permission = permission;
            _user = user;
            _provider = provider;
            _snowflake = snowflake;
            _configBLL = configBLL;
            _deptDAL = deptDAL;
            _org = org;
        }
        public virtual async Task<List<MZ_Role>> SelectSystemRoleList()
        {
            return await _user.SelectSystemRoles();
        }

        public virtual async Task<MZ_AdminInfo> GetUserInfoByName(string name)
        {
            return await _user.GetAdminByName(name);
        }

        public virtual async Task<MZ_AdminInfo> GetUserInfoById(long uid)
        {
            return await _user.GetAdminById(uid);
        }
        public virtual async Task<MZ_AdminInfo> GetUserInfoByCurentOrg(long uid)
        {
            ITAContext context = _provider.GetService<ITAContext>();
            var curuser = Data_ServerTokenInfo.From(context);
            return await _user.GetAdminByOrgId(uid, curuser.OrgId);
        }
        public virtual async Task<List<Out_SearchUser>> SeachUsers(string key)
        {
            List<Out_SearchUser> outlist = new List<Out_SearchUser>();
            var userlist = await _user.SearchUsers(key);
            foreach (var u in userlist)
            {
                Out_SearchUser tmp = new Out_SearchUser();
                tmp.Avatar = u.Avatar;
                tmp.Id = u.Id.Value;
                if (!string.IsNullOrEmpty(u.RealName))
                {
                    tmp.Name = u.RealName;
                }
                else
                {
                    if (!string.IsNullOrEmpty(u.Mobile))
                    {
                        tmp.Name = u.Mobile;
                    }
                    else
                    {
                        tmp.Name = u.UserName;
                    }
                }

                outlist.Add(tmp);
            }
            return outlist;
        }

        public virtual async Task<PageObject<MZ_Role>> SelectRoleList(In_RoleList query)
        {
            ITAContext context = _provider.GetService<ITAContext>();
            var curuser = Data_ServerTokenInfo.From(context);
            return await _user.SelectRoleList(query, curuser);
        }

        public virtual async Task<MZ_Role> GetRole(long id)
        {
            return await _user.GetRole(id);
        }
        public virtual async Task<PageObject<MZ_AdminInfo>> SelectAllocatedList(In_UserRoleList query)
        {
            var user = _provider.GetUser();
            query.orgId = user.OrgId;
            return await _user.SelectAllocatedList(query);
        }
        public virtual async Task<PageObject<MZ_AdminInfo>> SelectUnallocatedList(In_UserRoleList query)
        {
            var user = _provider.GetUser();
            query.orgId = user.OrgId;
            return await _user.SelectUnallocatedList(query);
        }
        [Trans]
        public virtual async Task<BusResponse<long>> AddRole(MZ_Role role)
        {
            try
            {
                var user = _provider.GetUser();
                if (user.OrgId == 0)
                {
                    return BusResponse<long>.Error(114, "未有所属企业,无法添加角色");
                }
                role.RoleDesc ??= string.Empty;
                if (user.OrgId == 1 && user.UserId == 1)
                {
                    role.IsSystem = "1";
                    role.OrgId = 0;
                }
                else
                {
                    role.IsSystem = "0";
                    role.OrgId = user.OrgId;
                }

                role.NoAlloca = "0";
                long id = await _user.AddRole(role);
                if (role.menuIds != null)
                {
                    if (user.OrgId != 1)
                    {
                        //可分配权限校验
                        var tmenulist = await _provider.GetService<MenuDAL>().SelectMenuListByUserId(null, null, "0", user.UserId, user.OrgId, false);
                        HashSet<long> hs = new HashSet<long>();
                        foreach (var menu in tmenulist)
                        {
                            hs.Add(menu.menu_id.Value);
                        }
                        foreach (long ckll in role.menuIds)
                        {
                            if (!hs.Contains(ckll))
                            {
                                return BusResponse<long>.Error(16, "存在不可分配权限");
                            }
                        }
                    }

                    await _permission.SetRolePermissions(id, role.menuIds);
                }
                return BusResponse<long>.Success(id);
            }
            catch (Exception ex)
            {
                return BusResponse<long>.Error(-12, ex.Message);
            }
        }
        [Trans]
        public virtual async Task<BusResponse<string>> UpdateRole(MZ_Role role)
        {
            try
            {
                if (role.RoleID == null)
                {
                    return BusResponse<string>.Error(14, "roleId参数不能为空");
                }
                var user = _provider.GetUser();
                MZ_Role old = await _user.GetRole(role.RoleID.Value);
                if (old.IsSystem == "1" && user.OrgId != 1)
                {
                    return BusResponse<string>.Error(13, "系统角色不可更改");
                }
                role.IsSystem = null;
                role.OrgId = null;
                if (await _user.UpdateRole(role) == 0)
                {
                    return BusResponse<string>.Error(15, "角色不存在");
                }
                if (role.menuIds != null)
                {
                    if (user.OrgId != 1)
                    {
                        //可分配权限校验
                        var tmenulist = await _provider.GetService<MenuDAL>().SelectMenuListByUserId(null, null, "0", user.UserId, user.OrgId, false);
                        HashSet<long> hs = new HashSet<long>();
                        foreach (var menu in tmenulist)
                        {
                            hs.Add(menu.menu_id.Value);
                        }
                        foreach (long ckll in role.menuIds)
                        {
                            if (!hs.Contains(ckll))
                            {
                                return BusResponse<string>.Error(16, "存在不可分配权限");
                            }
                        }
                    }

                    await _permission.SetRolePermissions(role.RoleID.Value, role.menuIds);
                }
                return BusResponse<string>.Success(null);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(12, ex.Message);
            }
        }
        [Trans]
        public virtual async Task<BusResponse<string>> DeleteRole(long id)
        {
            try
            {
                var user = _provider.GetUser();
                MZ_Role role = await _user.GetRole(id);
                if (user.UserId != 1 || user.OrgId != 1)
                {
                    if (role.IsSystem == "1")
                    {
                        return BusResponse<string>.Error(12, "系统角色不可删除");
                    }
                }

                await _user.DeleteRole(id);
                await _permission.SetRolePermissions(id, Array.Empty<long>());
                return BusResponse<string>.Success(null);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(12, ex.Message);
            }
        }
        public virtual async Task<PageObject<MZ_AdminInfo>> GetUserAllList(In_UserAllList query)
        {
            var userlist = await _user.GetUserALlList(query);
            if (userlist.List.Count > 0)
            {
                var idlist = userlist.List.Select(x => x.Id.Value).ToList();
                var orglist = await _org.SelectUserOrgListIn(idlist);
                Dictionary<long, List<Out_UserOrg>> outOrgs = new Dictionary<long, List<Out_UserOrg>>();
                foreach (Out_UserOrg outUO in orglist)
                {
                    List<Out_UserOrg> tmpuo;
                    if (!outOrgs.TryGetValue(outUO.UserId.Value, out tmpuo))
                    {
                        tmpuo = new List<Out_UserOrg>();
                        outOrgs.Add(outUO.UserId.Value, tmpuo);
                    }
                    tmpuo.Add(outUO);
                }
                foreach (MZ_AdminInfo user in userlist.List)
                {
                    List<Out_UserOrg> tmpuo;
                    if (outOrgs.TryGetValue(user.Id.Value, out tmpuo))
                    {
                        user.OrgNames = string.Join('、', tmpuo.Select(x => x.OrgName).ToList());
                    }

                }
            }

            return userlist;
        }

        public virtual async Task<PageObject<MZ_AdminInfo>> GetUserList(In_UserList query)
        {
            if (query.deptId != null)
            {
                if (query.deptIdWithChildren == false)
                {
                    query.depAncestors = null;
                }
                else
                {
                    query.depAncestors = (await _deptDAL.SelectById(query.deptId.Value)).ancestors;
                    query.deptId = null;
                }
            }
            Data_ServerTokenInfo user = _provider.GetUser();
            query.orgId = user.OrgId;
            if (query.isMyLeader == true)
            {
                var tmpuserorglist = await _org.SelectLeaderUserOrg(user.UserId, user.OrgId);
                var tmpdeptids = tmpuserorglist.Select(x => x.dept_id.Value).ToArray();
                HashSet<long> alldeptlist = new HashSet<long>();
                foreach (var tmpdeptid in tmpdeptids)
                {
                    if (!alldeptlist.Contains(tmpdeptid))
                    {
                        alldeptlist.Add(tmpdeptid);
                    }
                    var tmpdeptlist = await _deptDAL.SelectChildrenById(tmpdeptid);
                    var newdeptarr = tmpdeptlist.Select(x => x.dept_id.Value);
                    foreach (var dpid in newdeptarr)
                    {
                        if (!alldeptlist.Contains(dpid))
                        {
                            alldeptlist.Add(dpid);
                        }
                    }

                }
                query.deptIdList = alldeptlist.ToArray();
            }
            var scope = await user.GetScope(_provider);
            return await _user.GetUserList(query, scope);
        }

        public virtual async Task<PageObject<MZ_AdminInfo>> GetUserListForSync(In_UserList query)
        {
            if (query.deptId != null)
            {
                if (query.deptIdWithChildren == false)
                {
                    query.depAncestors = null;
                }
                else
                {
                    query.depAncestors = (await _deptDAL.SelectById(query.deptId.Value)).ancestors;
                    query.deptId = null;
                }
            }
            //Data_ServerTokenInfo user = _provider.GetUser();
            //query.orgId = user.OrgId;
            //var scope = await user.GetScope(_provider);
            return await _user.GetUserList(query, null);
        }

        public virtual BusResponse<string> DeleteUser(long id)
        {
            try
            {
                if (id < 3)
                {
                    return BusResponse<string>.Error(13, "不允许操作系统默认用户");
                }
                _user.DeleteUser(id);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(12, ex.Message);
            }
        }
        public virtual async Task<BusResponse<string>> AddUser(MZ_AdminInfo user)
        {
            try
            {
                if (!new Regex("^[a-zA-Z0-9_]+$").IsMatch(user.UserName))
                {
                    return BusResponse<string>.Error(111, "用户名只能包含数字、字母、下划线");
                }


                user.Mobile = string.Empty;
                user.Email = string.Empty;
                user.EmailActive = false;
                user.Avatar ??= string.Empty;
                user.Introduction ??= string.Empty;
                user.Signature ??= string.Empty;
                user.WaitSignature ??= string.Empty;
                if (string.IsNullOrEmpty(user.Sex))
                {
                    user.Sex = "2";
                }
                if (user.dept_id != null)
                {
                    var dept = await _deptDAL.SelectById(user.dept_id.Value);
                    user.OrgId = dept.OrgId;
                }
                else
                {
                    user.OrgId = 0;
                }
                ITAContext context = _provider.GetService<ITAContext>();
                user.Salt = MyAccess.Core.StringTool.GetEnglishChar(16);
                user.Password = MyAccess.Core.Crypter.MD5(string.Concat(user.Password, user.Salt));
                user.del_flag = "0";
                user.Id = _snowflake.NextId();
                if (string.IsNullOrEmpty(user.UserName))
                {
                    user.UserName = "$" + user.Id;
                }
                using (BLLTranScope scope = new BLLTranScope())
                {
                    // 新增用户角色关联
                    var roleVal = await _configBLL.SelectConfigByKey("sys.reg.roleId");
                    MZ_UserRole ur = new MZ_UserRole();
                    ur.UserId = user.Id;
                    ur.RoleID = long.Parse(roleVal);
                    ur.OrgId = 0;
                    List<MZ_UserRole> userRoles = new List<MZ_UserRole>();
                    userRoles.Add(ur);
                    await _user.AddUserRole(userRoles);


                    if (user.OrgId > 0)
                    {
                        // 新增用户组织关联
                        MZ_User_Org userOrg = new MZ_User_Org();
                        userOrg.dept_id = user.dept_id;
                        userOrg.OrgId = user.OrgId;
                        userOrg.post_name = user.post_name ?? "";
                        userOrg.UserId = user.Id;
                        userOrg.IsLeader = false;
                        userOrg.IsPrimary = true;
                        await _org.InsertUserOrg(userOrg);
                    }

                    await _user.AddUser(user);
                    // 完成
                    await scope.CompleteAsync();
                }

                return BusResponse<string>.Success(user.Id.ToString());
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(112, ex.Message);
            }
        }
        public virtual async Task<BusResponse<int>> UpdateUser(MZ_AdminInfo user)
        {
            try
            {
                if (user.Id == null)
                {
                    return BusResponse<int>.Error(14, "Id编号不能为null");
                }
                ITAContext context = _provider.GetService<ITAContext>();
                if (Data_ServerTokenInfo.From(context).UserId != 1)
                {
                    if (user.Id < 3)
                    {
                        return BusResponse<int>.Error(13, "不允许操作系统默认用户");
                    }
                }

                user.UserName = null;
                user.Mobile = null;
                user.Email = null;
                user.Password = null;
                user.Salt = null;

                if (user.dept_id != null)
                {
                    MZ_Dept dep = await _deptDAL.SelectById(user.dept_id.Value);
                    if (dep != null)
                    {
                        MZ_User_Org userOrg = new MZ_User_Org();
                        userOrg.OrgId = dep.OrgId;
                        userOrg.post_name = user.post_name ?? "";
                        userOrg.UserId = user.Id;
                        var userorg = await _org.SelectUserOrg(user.Id.Value, dep.OrgId.Value);
                        if (userorg == null)
                        {
                            userOrg.IsLeader = false;
                            userOrg.IsPrimary = true;
                            userOrg.dept_id = user.dept_id;
                            await _org.InsertUserOrg(userOrg);
                        }
                        else
                        {
                            await _org.UpdateUserOrg(userOrg);
                        }
                    }
                }

                int rt = await _user.UpdateUser(user);
                return BusResponse<int>.Success(rt);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(12, ex.Message);
            }
        }
        public virtual async Task<BusResponse<int>> AddSysRole(long uid, long roleId, long orgId)
        {
            MZ_UserRole ur = new MZ_UserRole();
            ur.UserId = uid;
            ur.RoleID = roleId;
            ur.OrgId = orgId;
            if (await _user.ExistUserRole(roleId, uid, orgId))
            {
                return BusResponse<int>.Error(111, "重复分配角色！");
            }
            return BusResponse<int>.Success(await _user.AddUserRoleItem(ur));
        }
        public virtual async Task<BusResponse<int>> DelSysRole(long uid, long roleId, long orgId)
        {
            return BusResponse<int>.Success(await _user.DeleteUserRoleById(roleId, uid, orgId));
        }
        public virtual async Task<List<Out_UserRole>> SelectUserRoleList(long uid)
        {
            return await _user.SelectUserRoleList(uid);
        }
        public virtual async Task<BusResponse<string>> SetUserRoles(long uid, long[] roleIds)
        {
            var curuser = _provider.GetUser();
            long orgId = curuser.OrgId;
            if (orgId <= 0)
            {
                return BusResponse<string>.Error(21, "非企业账号无法授权");
            }
            List<long> sysroles = await _permission.GetRoleIdsByUser(uid, 0);
            // 新增用户与角色关联
            List<MZ_UserRole> list = new List<MZ_UserRole>();
            foreach (long roleId in roleIds)
            {
                if (sysroles.Contains(roleId))
                {
                    continue;
                }
                else
                {
                    if (!await _user.ExistUserRole(roleId, curuser.UserId, orgId))
                    {
                        MZ_Role role = await _user.GetRole(roleId);
                        if (role.OrgId != orgId)
                        {
                            return BusResponse<string>.Error(123, "无权授权角色'" + role.RoleName + "'");
                        }
                    }
                }

                MZ_UserRole ur = new MZ_UserRole();
                ur.UserId = uid;
                ur.RoleID = roleId;
                ur.OrgId = orgId;
                list.Add(ur);
            }
            if (list.Count > 0)
            {
                // 删除用户与角色关联
                await _user.DeleteUserRoleByUserId(uid, orgId);
                await _user.AddUserRole(list);
            }
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<int>> ResetPwd(long userId, string password, IUserInfo updater)
        {
            MZ_AdminInfo user = InterceptFactory.CreateEntityOp<MZ_AdminInfo>();
            user.Id = userId;
            user.Salt = MyAccess.Core.StringTool.GetEnglishChar(16);
            user.Password = MyAccess.Core.Crypter.MD5(string.Concat(password, user.Salt));
            user.SetUpdateBy(updater);
            return BusResponse<int>.Success(await _user.UpdateUser(user));
        }
        public virtual async Task<BusResponse<int>> UpdateUserStatus(long userId, string status, IUserInfo updater)
        {
            if (status != "0" && status != "1")
            {
                return BusResponse<int>.Error(12, "状态值错误");
            }
            MZ_AdminInfo user = InterceptFactory.CreateEntityOp<MZ_AdminInfo>();
            user.Id = userId;
            user.status = status;
            user.SetUpdateBy(updater);
            return BusResponse<int>.Success(await _user.UpdateUser(user));
        }

        public virtual async Task<BusResponse<string>> ImportUser(List<MZ_AdminInfo> userList, bool isUpdateSupport, IUserInfo operName)
        {
            if (userList == null || userList.Count == 0)
            {
                return BusResponse<string>.Error(12, "导入用户数据不能为空");
            }
            int successNum = 0;
            int failureNum = 0;
            StringBuilder successMsg = new StringBuilder();
            StringBuilder failureMsg = new StringBuilder();
            string password = await _configBLL.SelectConfigByKey("sys.user.initPassword");
            foreach (MZ_AdminInfo user in userList)
            {
                try
                {
                    // 验证是否存在这个用户
                    MZ_AdminInfo u = await _user.GetAdminById(user.Id.Value);
                    if (u == null)
                    {
                        user.Password = password;
                        user.SetCreateBy(operName);
                        BusResponse<string> rt = await this.AddUser(user);
                        if (rt.IsSuccess())
                        {
                            successNum++;
                            successMsg.Append("<br/>" + successNum + "、账号 " + user.UserName + " 导入成功");
                        }
                        else
                        {
                            throw new Exception(rt.Message);
                        }
                    }
                    else if (isUpdateSupport)
                    {
                        user.SetUpdateBy(operName);
                        BusResponse<int> rt = await this.UpdateUser(user);
                        if (rt.IsSuccess())
                        {
                            successNum++;
                            successMsg.Append("<br/>" + successNum + "、账号 " + user.UserName + " 更新成功");
                        }
                        else
                        {
                            throw new Exception(rt.Message);
                        }
                    }
                    else
                    {
                        failureNum++;
                        failureMsg.Append("<br/>" + failureNum + "、账号 " + user.UserName + " 已存在");
                    }
                }
                catch (Exception e)
                {
                    failureNum++;
                    String msg = "<br/>" + failureNum + "、账号 " + user.UserName + " 导入失败：";
                    failureMsg.Append(msg + e.Message);
                }
            }
            if (failureNum > 0)
            {
                failureMsg.Insert(0, "很抱歉，共 " + failureNum + " 条数据格式不正确，错误如下：");
                return BusResponse<string>.Error(23, failureMsg.ToString());
            }
            else
            {
                successMsg.Insert(0, "恭喜您，数据已全部导入成功！共 " + successNum + " 条，数据如下：");
            }
            return BusResponse<string>.Success(null, successMsg.ToString());
        }

        /// <summary>
        /// 取消授权用户角色
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>

        public virtual async Task<BusResponse<string>> DeleteAuthUser(MZ_UserRole userRole)
        {
            try
            {
                ITAContext context = _provider.GetService<ITAContext>();
                long orgid = Data_ServerTokenInfo.From(context).OrgId;
                if (orgid <= 0)
                {
                    return BusResponse<string>.Error(21, "非企业账号无权取消授权");
                }
                userRole.OrgId = orgid;

                var user = _provider.GetUser();
                MZ_Role role = await _user.GetRole(userRole.RoleID.Value);
                if (role == null)
                {
                    return BusResponse<string>.Error(121, "角色不存在");
                }
                if (role.OrgId > 0 && role.OrgId != user.OrgId)
                {
                    return BusResponse<string>.Error(122, "无权取消此角色授权");
                }

                if (role.OrgId == 0 && !await _user.ExistUserRole(userRole.RoleID.Value, user.UserId, user.OrgId))
                {
                    return BusResponse<string>.Error(123, "无权取消此角色授权");
                }

                await _user.DeleteUserRoleByUserRole(userRole);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(13, ex.Message);
            }
        }


        /// <summary>
        /// 批量取消授权用户角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> DeleteAuthUsers(long roleId, long[] userIds)
        {
            try
            {
                ITAContext context = _provider.GetService<ITAContext>();
                long orgid = Data_ServerTokenInfo.From(context).OrgId;
                if (orgid <= 0)
                {
                    return BusResponse<string>.Error(21, "非企业账号无权取消授权");
                }
                var user = _provider.GetUser();
                MZ_Role role = await _user.GetRole(roleId);
                if (role == null)
                {
                    return BusResponse<string>.Error(121, "角色不存在");
                }
                if (role.OrgId > 0 && role.OrgId != user.OrgId)
                {
                    return BusResponse<string>.Error(122, "无权取消此角色授权");
                }

                if (role.OrgId == 0 && !await _user.ExistUserRole(roleId, user.UserId, user.OrgId))
                {
                    return BusResponse<string>.Error(123, "无权取消此角色授权");
                }
                await _user.DeleteUserRoleByRoleAndUserId(roleId, userIds, orgid);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(13, ex.Message);
            }
        }


        /// <summary>
        /// 批量选择授权用户角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> InsertAuthUsers(long roleId, long[] userIds)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(21, "非企业账号无权取消授权");
            }
            MZ_Role role = await _user.GetRole(roleId);
            if (role == null)
            {
                return BusResponse<string>.Error(121, "角色不存在");
            }
            if (role.OrgId > 0 && role.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(122, "无权授权此角色");
            }
            if (role.OrgId == 0 && !await _user.ExistUserRole(roleId, user.UserId, user.OrgId))
            {
                return BusResponse<string>.Error(123, "无此系统角色");
            }

            // 新增用户与角色管理
            List<MZ_UserRole> list = new List<MZ_UserRole>();
            foreach (long userId in userIds)
            {
                MZ_UserRole ur = new MZ_UserRole();
                ur.UserId = userId;
                ur.RoleID = roleId;
                ur.OrgId = user.OrgId;
                list.Add(ur);
            }
            try
            {
                await _user.AddUserRole(list);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(13, ex.Message);
            }

        }
        public virtual async Task<BusResponse<Out_Login>> Reg(In_RegData data, ITAContext context)
        {
            try
            {
                long orgId = 0;
                if (!string.IsNullOrEmpty(data.Code))
                {
                    orgId = InvitLinkHelper.ParseLink(_provider, data.Code);
                }

                data.Avatar ??= string.Empty;
                MZ_AdminInfo user = new MZ_AdminInfo();
                if (!string.IsNullOrEmpty(data.Avatar))
                {
                    user.Avatar = await _provider.GetService<FileHelper>().UploadBase64(data.Avatar);
                }
                else
                {
                    user.Avatar = string.Empty;
                }
                user.Mobile = data.Mobile ?? string.Empty;
                user.Email = data.Email ?? string.Empty;
                user.EmailActive = false;
                user.Introduction = string.Empty;
                user.RealName = data.RealName;
                user.Sex = "2";
                user.OrgId = orgId;
                user.Salt = MyAccess.Core.StringTool.GetEnglishChar(16);
                user.Password = MyAccess.Core.Crypter.MD5(string.Concat(data.Password, user.Salt));
                user.del_flag = "0";
                user.status = "0";
                user.Id = _snowflake.NextId();
                user.UserName = "$" + user.Id;
                user.Signature ??= string.Empty;
                user.WaitSignature ??= string.Empty;
                user.createId = 0;
                user.create_time = DateTime.Now;
                user.updateId = 0;
                user.update_time = user.create_time;
                var roleVal = await _configBLL.SelectConfigByKey("sys.reg.roleId");
                using (BLLTranScope scope = new BLLTranScope())
                {
                    // 新增用户角色关联
                    MZ_UserRole ur = new MZ_UserRole();
                    ur.UserId = user.Id;
                    ur.RoleID = long.Parse(roleVal);
                    ur.OrgId = 0;
                    List<MZ_UserRole> userRoles = new List<MZ_UserRole>();
                    userRoles.Add(ur);
                    await _user.AddUserRole(userRoles);

                    if (orgId > 0)
                    {
                        // 新增用户组织关联
                        MZ_User_Org userOrg = new MZ_User_Org();
                        userOrg.dept_id = data.DeptId;
                        userOrg.OrgId = orgId;
                        userOrg.post_name = data.PostName ?? "";
                        userOrg.UserId = user.Id;
                        userOrg.IsLeader = false;
                        userOrg.IsPrimary = true;
                        await _org.InsertUserOrg(userOrg);
                    }

                    await _user.AddUser(user);
                    // 完成
                    await scope.CompleteAsync();
                }
                var res = await _provider.GetService<AuthBLL>().Login(user, context);
                res.Data.isnew = true;
                return res;
            }
            catch (Exception ex)
            {
                return BusResponse<Out_Login>.Error(33, ex.Message + ex.StackTrace);
            }

        }
        public virtual async Task<BusResponse<string>> Join(In_JoinUser data, long targetOrgId)
        {
            if (data.depId > 0)
            {
                if (!_deptDAL.CheckDeptId(data.depId, targetOrgId))
                {
                    return BusResponse<string>.Error(103, "要加入的部门不存在");
                }
            }
            else
            {
                var rootDept = await _deptDAL.SelectRoot(targetOrgId);
                data.depId = rootDept.dept_id.Value;
            }

            if (await _org.CheckExistOrg(data.uid, targetOrgId))
            {
                return BusResponse<string>.Error(105, "无法重复加入企业");
            }
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    MZ_User_Org userOrg = new MZ_User_Org();
                    userOrg.dept_id = data.depId;
                    userOrg.OrgId = targetOrgId;
                    userOrg.UserId = data.uid;
                    userOrg.post_name = data.postName;
                    userOrg.IsLeader = false;
                    userOrg.IsPrimary = true;

                    MZ_AdminInfo upAdmin = new MZ_AdminInfo();
                    upAdmin.Id = data.uid;
                    upAdmin.OrgId = userOrg.OrgId;
                    await _user.UpdateUser(upAdmin);
                    await _org.InsertUserOrg(userOrg);
                    await scope.CompleteAsync();
                }

            }
            catch
            {
                return BusResponse<string>.Error(106, "数据繁忙，请重试");
            }

            return BusResponse<string>.Success();
        }
    }
}
