using AuthService.Controller;
using AuthService.DAL;
using AuthService.Fields;
using AuthService.Model;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using JiebaNet.Segmenter;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TemplateAction.Core;

namespace AuthService
{
    public class OrgBLL
    {
        protected OrgDAL _orgDAL;
        protected SnowflakeHelper _snowflake;
        protected UserDAL _userDAL;
        protected ITAContext _context;
        protected ITAServiceProvider _provider;
        public OrgBLL(ITAServiceProvider serviceProvider, OrgDAL org, UserDAL userDAL, SnowflakeHelper snowflake, ITAContext context)
        {
            _provider = serviceProvider;
            _orgDAL = org;
            _userDAL = userDAL;
            _snowflake = snowflake;
            _context = context;
        }
        public virtual async Task<List<Out_UserDept>> SelectUserDept(long userId, long orgId)
        {
            return await _orgDAL.SelectUserDept(userId, orgId);
        }
        public virtual async Task<BusResponse<string>> AddClone(long uid, long deptId)
        {
            if (await _orgDAL.CheckExistDept(uid, deptId))
            {
                return BusResponse<string>.Error(114, "目标部门已存在分身");
            }
            var targetDept = await _provider.GetService<DeptDAL>().SelectById(deptId);
            MZ_User_Org userOrg = new MZ_User_Org();
            userOrg.OrgId = targetDept.OrgId;
            userOrg.UserId = uid;
            userOrg.dept_id = deptId;
            userOrg.post_name = string.Empty;
            userOrg.IsLeader = false;
            userOrg.IsPrimary = false;
            await _orgDAL.InsertUserOrg(userOrg);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> DelClone(long uid, long deptId)
        {
            var olduser = await _orgDAL.SelectUserOrgByDept(uid, deptId);
            if (olduser == null)
            {
                return BusResponse<string>.Error(114, "分身不存在");
            }
            if (olduser.IsPrimary == true)
            {
                return BusResponse<string>.Error(115, "无法删除主要分身");
            }
            await _orgDAL.DeleteUserOrgByDept(uid, deptId);
            return BusResponse<string>.Success();
        }
        public virtual async Task<bool> IsMember(long uid)
        {
            var user = _provider.GetUser();
            return await _orgDAL.CheckExistOrg(uid, user.OrgId);
        }
        public virtual async Task<BusResponse<long>> Join(In_JoinAO data)
        {
            if (string.IsNullOrEmpty(data.code))
            {
                return BusResponse<long>.Error(110, "邀请码不能为空");
            }
            var user = _provider.GetUser();
            long orgId = InvitLinkHelper.ParseLink(_provider, data.code);

            if (await _orgDAL.CheckExistOrg(user.UserId, orgId))
            {
                return BusResponse<long>.Error(114, "无法重复加入企业");
            }

            MZ_User_Org userOrg = new MZ_User_Org();
            userOrg.dept_id = data.deptId;
            if (userOrg.dept_id == null || userOrg.dept_id <= 0)
            {
                var rootDept = await _provider.GetService<DeptDAL>().SelectRoot(orgId);
                if (rootDept == null)
                {
                    return BusResponse<long>.Error(111, "邀请码所属组织不存在");
                }
                userOrg.dept_id = rootDept.dept_id;
            }
            userOrg.OrgId = orgId;
            userOrg.UserId = user.UserId;
            userOrg.post_name = data.postName ?? "";
            userOrg.IsLeader = false;
            userOrg.IsPrimary = true;
            MZ_AdminInfo upAdmin = new MZ_AdminInfo();
            upAdmin.Id = user.UserId;
            upAdmin.OrgId = userOrg.OrgId;
            upAdmin.SetUpdateBy(user);
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await _orgDAL.InsertUserOrg(userOrg);
                    await _userDAL.UpdateUser(upAdmin);
                    await scope.CompleteAsync();
                }
                return BusResponse<long>.Success(orgId);
            }
            catch (Exception ex)
            {
                return BusResponse<long>.Error(21, ex.Message);
            }
        }
        public virtual async Task<List<MZ_Org>> SearchOrg(string key)
        {
            bool isSystem = _provider.GetUser().OrgId == 1;
            return await _orgDAL.SearchOrg(key, isSystem);
        }
        public virtual async Task<BusResponse<int>> EditUserOrg(MZ_User_Org uo)
        {
            try
            {
                Data_ServerTokenInfo user = Data_ServerTokenInfo.From(_context);
                uo.OrgId = user.OrgId;
                if (uo.UserId == null)
                {
                    return BusResponse<int>.Error(110, "编辑用户的企业信息");
                }
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await this.OnEditUserOrg(uo);
                    await _orgDAL.UpdateUserOrg(uo);
                    // 完成
                    await scope.CompleteAsync();
                }

                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(102, ex.Message);
            }
        }
        protected virtual async Task OnEditUserOrg(MZ_User_Org uo)
        {
            await Task.CompletedTask;
        }
        public virtual async Task<BusResponse<int>> Recovery(In_RecoveryOrg org)
        {
            var user = _provider.GetUser();
            if (!await _userDAL.ExistUserRole(1, user.UserId, user.OrgId))
            {
                return BusResponse<int>.Error(110, "无操作权限");
            }
            MZ_Org old = await _orgDAL.SelectById(org.OrgId);
            if (old == null || old.del_flag == "0")
            {
                return BusResponse<int>.Error(111, "目标组织无法恢复");
            }

            MZ_Org newOrg = new MZ_Org();
            newOrg.Id = old.Id;
            newOrg.SetUpdateBy(user);
            newOrg.del_flag = "0";
            await _orgDAL.UpdateOrg(newOrg);

            MZ_Dept dept = await _provider.GetService<DeptDAL>().SelectRoot(org.OrgId);
            if (dept == null)
            {
                return BusResponse<int>.Error(112, "目标组织的部门不存在");
            }
            MZ_User_Org userOrg = new MZ_User_Org();
            userOrg.OrgId = org.OrgId;
            userOrg.UserId = org.UserId;
            userOrg.dept_id = dept.dept_id;
            userOrg.post_name = string.Empty;
            userOrg.IsLeader = false;
            userOrg.IsPrimary = true;
            await _orgDAL.InsertUserOrg(userOrg);

            List<MZ_UserRole> userRoles = new List<MZ_UserRole>();
            foreach (long tmproleId in org.Roles)
            {
                MZ_UserRole userRole = new MZ_UserRole();
                userRole.UserId = org.UserId;
                userRole.OrgId = org.OrgId;
                userRole.RoleID = tmproleId;
                userRoles.Add(userRole);
            }
            if (userRoles.Count > 0)
            {
                await _userDAL.AddUserRole(userRoles);
            }
            return BusResponse<int>.Success();
        }
        public virtual async Task<BusResponse<int>> Delete(long id)
        {
            try
            {
                if (id == 1)
                {
                    return BusResponse<int>.Error(114, "系统组织不可删除");
                }
                Data_ServerTokenInfo user = Data_ServerTokenInfo.From(_context);
                var org = await _orgDAL.SelectById(id);
                if (org == null)
                {
                    return BusResponse<int>.Error(115, "企业不存在");
                }
                //判断是否有权限
                if (!await _orgDAL.CheckOrgPerm(user.UserId, org.Id.Value, "/AuthService/Org/Remove"))
                {
                    return BusResponse<int>.Error(113, "没有解散当前企业的权限");
                }
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await OnDelete(org, user);
                    await _orgDAL.DeleteTrans(id);
                    await _orgDAL.DeleteUserOrg(id);
                    await _userDAL.DeleteUserRoleByOrg(id);
                    // 完成
                    await scope.CompleteAsync();
                }

                long newOrg = 0;
                var uo = await _orgDAL.SelectUserOrgByNewest(user.UserId);
                if (uo != null)
                {
                    newOrg = uo.OrgId.Value;
                }
                MZ_AdminInfo upAdmin = new MZ_AdminInfo();
                upAdmin.Id = user.UserId;
                upAdmin.OrgId = newOrg;
                await _userDAL.UpdateUser(upAdmin);

                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(102, ex.Message);
            }
        }
        protected virtual async Task OnDelete(MZ_Org org, Data_ServerTokenInfo user)
        {
            await Task.CompletedTask;
        }

        public virtual async Task<BusResponse<string>> UpdateUserOrg(MZ_User_Org userOrg)
        {
            await _orgDAL.UpdateUserOrg(userOrg);
            return BusResponse<string>.Success(null);
        }
        public virtual async Task<BusResponse<string>> ChangeCreator(long id, long uid)
        {
            try
            {
                if (id == 1)
                {
                    return BusResponse<string>.Error(101, "无法操作系统企业");
                }
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                if (!await _orgDAL.CheckOrgPerm(user.UserId, id, "/AuthService/Org/ChangeCreator"))
                {
                    return BusResponse<string>.Error(102, "没有移交当前企业的权限");
                }
                if (!await _orgDAL.CheckExistOrg(uid, id))
                {
                    return BusResponse<string>.Error(103, "目标用户未加入企业");
                }
                MZ_Org org = new MZ_Org();
                org.Id = id;
                org.createId = uid;
                await _orgDAL.UpdateOrg(org);

                //移交组织开发者
                await BusUtility.Dispatch("ChangeCreator", new
                {
                    id = id,
                    uid = uid
                });

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(12, ex.Message);
            }
        }
        public virtual async Task<BusResponse<string>> UpdateOrg(MZ_Org org)
        {

            try
            {
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                //判断是否有当前组织的管理员权限
                if (!await _orgDAL.CheckOrgPerm(user.UserId, org.Id.Value, "/AuthService/Org/Edit"))
                {
                    return BusResponse<string>.Error(102, "没有当前组织的编辑权限");
                }
                MZ_Org old = await _orgDAL.SelectById(org.Id.Value);
                if (old == null)
                {
                    return BusResponse<string>.Error(103, "当前组织不存在");
                }

                if (org.OrgName != null || org.Intro != null)
                {
                    string newname = org.OrgName ?? old.OrgName;
                    string newIntro = org.Intro ?? old.Intro;
                    org.KeyWords = string.Join(" ", new JiebaSegmenter().CutForSearch(newname + " " + newIntro ?? string.Empty));
                }
                if (org.Lng != null && org.Lat != null)
                {
                    org.Geo = MyAccess.Core.GeoHash.Encode(org.Lat.Value, org.Lng.Value);
                }
                org.del_flag = null;
                org.SetUpdateBy(user);
                await _orgDAL.UpdateOrg(org);
                return BusResponse<string>.Success(null);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(12, ex.Message);
            }
        }
        public virtual async Task<List<MZ_Org>> SelectManOrgList()
        {
            ITAContext context = _provider.GetService<ITAContext>();
            var curuser = Data_ServerTokenInfo.From(context);
            return await _orgDAL.SelectOrgListByRole(curuser.UserId, 2);
        }
        public virtual async Task<List<MZ_Org>> SelectUserOrgList(long uid)
        {
            return await _orgDAL.SelectUserOrgList(uid);
        }
        public virtual async Task<MZ_User_Org_V> SelectUserOrgViewById(long orgId)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            return await _orgDAL.SelectUserOrgViewById(user.UserId, orgId);
        }

        public virtual async Task<MZ_Org> SelectById(long id)
        {
            var org = await _orgDAL.SelectById(id);
            if (org != null)
            {
                org.Creator = await _userDAL.GetAdminById(org.createId.Value);
            }
            return org;
        }
        public virtual async Task<BusResponse<bool>> IsJoin(long id)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            return BusResponse<bool>.Success(await _orgDAL.CheckExistOrg(user.UserId, id));
        }
        public virtual async Task OnDeleteUserFromOrg(long uid, long orgId)
        {
            await Task.CompletedTask;
        }
        public virtual async Task<BusResponse<string>> DeleteUser(long id)
        {
            try
            {
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                if (!await _orgDAL.CheckExistOrg(id, user.OrgId))
                {
                    return BusResponse<string>.Error(111, "参数错误");
                }

                var uids = await _userDAL.SelectManUserIds(id);
                if (uids.Count < 2 && uids.Contains(user.UserId))
                {
                    return BusResponse<string>.Error(103, "唯一管理员无法退出组织");
                }

                using (BLLTranScope scope = new BLLTranScope())
                {
                    await OnDeleteUserFromOrg(id, user.OrgId);
                    await _userDAL.DeleteUserRoleByUserId(id, user.OrgId);
                    await _orgDAL.DeleteUserOrg(id, user.OrgId);
                    // 完成
                    await scope.CompleteAsync();
                }

                long newOrg = 0;
                var uo = await _orgDAL.SelectUserOrgByNewest(id);
                if (uo != null)
                {
                    newOrg = uo.OrgId.Value;
                }
                MZ_AdminInfo upAdmin = new MZ_AdminInfo();
                upAdmin.Id = id;
                upAdmin.OrgId = newOrg;
                await _userDAL.UpdateUser(upAdmin);
                await _provider.GetService<OperatorHelper>().ClearServerData(id);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> ExitOrg(long id)
        {
            try
            {
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                if (await _orgDAL.CheckExistOrg(user.UserId, id))
                {
                    var uids = await _userDAL.SelectManUserIds(id);
                    if (uids.Count < 2 && uids.Contains(user.UserId))
                    {
                        return BusResponse<string>.Error(103, "唯一管理员无法退出组织");
                    }
                    await _userDAL.DeleteUserMan(user.UserId, id);
                }


                await _orgDAL.DeleteUserOrg(user.UserId, id);
                long newOrg = 0;
                var uo = await _orgDAL.SelectUserOrgByNewest(user.UserId);
                if (uo != null)
                {
                    newOrg = uo.OrgId.Value;
                }
                MZ_AdminInfo upAdmin = new MZ_AdminInfo();
                upAdmin.Id = user.UserId;
                upAdmin.OrgId = newOrg;
                await _userDAL.UpdateUser(upAdmin);

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<string>> Switch(long id)
        {
            try
            {
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                if (!await _orgDAL.CheckExistOrg(user.UserId, id))
                {
                    return BusResponse<string>.Error(111, "参数错误");
                }

                MZ_AdminInfo uinfo = new MZ_AdminInfo();
                uinfo.OrgId = id;
                uinfo.Id = user.UserId;
                await _userDAL.UpdateUser(uinfo);

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
        [Trans]
        public virtual async Task<BusResponse<long>> Create(MZ_Org org, bool oncreated = true, MZ_User_Org userOrg = null)
        {
            if (await _orgDAL.CheckOrgNameUnique(org.OrgName))
            {
                return BusResponse<long>.Error(111, "公司名称已经被认证");
            }
            if (org.Lng != null && org.Lat != null)
            {
                org.Geo = MyAccess.Core.GeoHash.Encode(org.Lat.Value, org.Lng.Value);
            }

            IUserInfo user = Data_ServerTokenInfo.From(_context);
            org.Id = _snowflake.NextId();
            org.SetCreateBy(user);
            org.del_flag = "0";
            org.status = "0";
            org.Lng ??= 0;
            org.Lat ??= 0;
            org.Geo ??= string.Empty;
            org.KeyWords = string.Join(" ", new JiebaSegmenter().CutForSearch(org.OrgName + " " + org.Intro ?? string.Empty));
            await _orgDAL.InsertOrgTrans(org);

            MZ_Dept dept = new MZ_Dept();
            dept.dept_id = _snowflake.NextId();
            dept.ancestors = dept.dept_id + ",";
            dept.SetCreateBy(user);
            dept.del_flag = "0";
            dept.dept_name = org.OrgName;
            dept.email = string.Empty;
            dept.phone = string.Empty;
            dept.order_num = 0;
            dept.OrgId = org.Id;
            dept.parent_id = 0;
            dept.status = "0";
            await _orgDAL.InsertDeptTrans(dept);

            if (userOrg == null)
            {
                userOrg = new MZ_User_Org();
                userOrg.post_name = string.Empty;
            }
            userOrg.dept_id = dept.dept_id;
            userOrg.OrgId = org.Id;
            userOrg.UserId = user.UserId;
            userOrg.IsLeader = false;
            userOrg.IsPrimary = true;
            await _orgDAL.InsertUserOrgTrans(userOrg);


            //为指定用户添加企业的管理角色
            MZ_UserRole ur = new MZ_UserRole();
            ur.UserId = user.UserId;
            ur.RoleID = 2;
            ur.OrgId = org.Id;
            await _userDAL.AddUserRoleItem(ur);

            if (oncreated)
            {
                return await OnCreate(org, user);
            }
            else
            {
                return BusResponse<long>.Success(org.Id.Value);
            }
        }
        protected virtual async Task<BusResponse<long>> OnCreate(MZ_Org org, IUserInfo user)
        {

            //如果用户未有默认企业,则当前创建的企业作为默认企业
            if (user.OrgId <= 0)
            {
                MZ_AdminInfo upAdmin = new MZ_AdminInfo();
                upAdmin.Id = user.UserId;
                upAdmin.OrgId = org.Id;
                await _userDAL.UpdateUser(upAdmin);
            }

            return BusResponse<long>.Success(org.Id.Value);
        }

        public virtual async Task<BusResponse<string>> GenerateInvitLink()
        {
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(_context);
            return await Task.FromResult(BusResponse<string>.Success(InvitLinkHelper.GenerateLink(_provider, user.OrgId)));
        }
        public virtual async Task<BusResponse<Out_InvitData>> ParseInvitCode(string code)
        {
            try
            {
                long orgId = InvitLinkHelper.ParseLink(_provider, code);
                var org = await _orgDAL.SelectById(orgId);
                Out_InvitData data = new Out_InvitData();
                data.OrgName = org.OrgName;
                data.OrgId = org.Id.Value;

                MZ_Dept iptdept = new MZ_Dept();
                iptdept.OrgId = orgId;
                var deptlist = _provider.GetService<DeptDAL>().SelectDeptList(iptdept);
                data.DeptList = MZ_Dept.DeptList2Tree(deptlist);
                return BusResponse<Out_InvitData>.Success(data);
            }
            catch (Exception ex)
            {
                return BusResponse<Out_InvitData>.Error(33, ex.Message);
            }

        }
        public virtual async Task<BusResponse<List<MZ_AppStyle>>> StyleList()
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<List<MZ_AppStyle>>.Error(111, "企业用户才有主题功能");
            }
            var orgDAL = _provider.GetService<OrgDAL>();
            //判断是否有当前组织的管理员权限
            if (!await orgDAL.CheckOrgPerm(user.UserId, user.OrgId, "/AuthService/Org/Edit"))
            {
                return BusResponse<List<MZ_AppStyle>>.Error(112, "没有当前组织的编辑权限");
            }
            var orgStyleDAL = _provider.GetService<OrgStyleDAL>();
            var tlist = await orgStyleDAL.OrgStyleList(user.OrgId);
            return BusResponse<List<MZ_AppStyle>>.Success(tlist);
        }
        public virtual async Task<BusResponse<string>> ChangeStyle(string styleId)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "企业用户才有主题功能");
            }
            var orgDAL = _provider.GetService<OrgDAL>();
            //判断是否有当前组织的管理员权限
            if (!await orgDAL.CheckOrgPerm(user.UserId, user.OrgId, "/AuthService/Org/Edit"))
            {
                return BusResponse<string>.Error(112, "没有当前组织的编辑权限");
            }
            var orgStyleDAL = _provider.GetService<OrgStyleDAL>();
            if (!await orgStyleDAL.Some(x => x.OrgId == user.OrgId && x.StyleId == styleId))
            {
                return BusResponse<string>.Error(113, "主题不存在");
            }
            if (styleId == "")
            {
                await orgStyleDAL.ClearUsing(user.OrgId);
                return BusResponse<string>.Success();
            }
            else
            {
                MZ_OrgStyle style = new MZ_OrgStyle();
                style.OrgId = user.OrgId;
                style.StyleId = styleId;
                style.IsUsing = true;

                try
                {
                    using (BLLTranScope scope = new BLLTranScope())
                    {
                        await orgStyleDAL.ClearUsing(user.OrgId);
                        await orgStyleDAL.Update(style);
                        // 完成
                        await scope.CompleteAsync();
                    }

                    return BusResponse<string>.Success();
                }
                catch (Exception ex)
                {
                    return BusResponse<string>.Error(131, ex.Message);
                }
            }


        }

        /// <summary>
        /// 获取当前级别的组织数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<List<Out_MemberTreeItem>> GetTree(In_MemberTreeList query)
        {
            if (query.type == "org")
            {
                var deplist = await _orgDAL.GetDeptList(query);
                var ulist = await _orgDAL.GetUserList(query);
                return deplist.Concat(ulist).ToList();
            }
            else if (query.type == "user")
            {
                var deplist = await _orgDAL.GetDeptList(query);
                var ulist = await _orgDAL.GetUserList(query);
                return deplist.Concat(ulist).ToList();
            }
            else if (query.type == "dept")
            {
                return await _orgDAL.GetDeptList(query);
            }
            else if (query.type == "role")
            {
                In_RoleList rquery = new In_RoleList();
                rquery.showAll = true;
                rquery.status = "0";
                var curuser = Data_ServerTokenInfo.From(_context);
                var rolelist = await _userDAL.SelectRoleList(rquery, curuser);

                List<Out_MemberTreeItem> tlist = new List<Out_MemberTreeItem>();
                foreach (var it in rolelist.List)
                {
                    tlist.Add(new Out_MemberTreeItem()
                    {
                        type = "role",
                        id = it.RoleID.Value,
                        name = it.RoleName
                    });
                }
                return tlist;
            }
            else
            {
                return new List<Out_MemberTreeItem>();
            }
        }

        public virtual async Task<List<Out_MemberTreeItem>> SearchUserList(string key)
        {
            return await _orgDAL.SearchUserList(key);
        }

        public virtual async Task<MZ_OrgExt> SelectOrgExt(long orgId, string field)
        {
            var orgExtDAL = _provider.GetService<OrgExtDAL>();
            var torgextlist = await orgExtDAL.SelectList(x => x.OrgId == orgId && x.ExtField == field);
            if (torgextlist.Count > 0)
            {
                return torgextlist[0];
            }
            return new MZ_OrgExt()
            {
                OrgId = orgId,
                ExtField = field,
                ExtValue = "[]"
            };
        }
        public virtual async Task<BusResponse<int>> DeleteOrgExt(long orgId, string field)
        {
            var orgExtDAL = _provider.GetService<OrgExtDAL>();
            int rs = await orgExtDAL.Delete(x => x.OrgId == orgId && x.ExtField == field);
            return BusResponse<int>.Success(rs);
        }
        public virtual async Task<BusResponse<int>> SaveOrgExt(long orgId, string field, string val)
        {
            var orgExtDAL = _provider.GetService<OrgExtDAL>();
            MZ_OrgExt item = new MZ_OrgExt()
            {
                OrgId = orgId,
                ExtField = field,
                ExtValue = val
            };
            int rs = await orgExtDAL.InsertOrUpdate(item);
            return BusResponse<int>.Success(rs);
        }
        public virtual async Task<List<FieldBase>> GetExtFormFields(long orgId, string field)
        {
            var orgExtDAL = _provider.GetService<OrgExtDAL>();
            var torgextlist = await orgExtDAL.SelectList(x => x.OrgId == orgId && x.ExtField == field);
            if (torgextlist.Count == 0)
            {
                return new List<FieldBase>();
            }
            return System.Text.Json.JsonSerializer.Deserialize<List<FieldBase>>(torgextlist[0].ExtValue, FieldJsonSerializerConfig.FieldOptions);
        }
        public virtual async Task<BusResponse<List<FieldBase>>> FormFields(long orgId, string field, bool ext, bool isfixed)
        {
            var orgExtDAL = _provider.GetService<OrgExtDAL>();

            List<FieldBase> tlist = new List<FieldBase>();
            if (isfixed)
            {
                var redis = _provider.GetService<GeneralRedisHelper>();
                var tmpstr = redis.HashGet("FixedFields", field);
                if (!string.IsNullOrEmpty(tmpstr))
                {
                    var fixedList = System.Text.Json.JsonSerializer.Deserialize<List<FieldBase>>(tmpstr, FieldJsonSerializerConfig.FieldOptions);
                    tlist.AddRange(fixedList);
                }
            }

            if (ext)
            {
                var torgextlist = await orgExtDAL.SelectList(x => x.OrgId == orgId && x.ExtField == field);
                if (torgextlist.Count > 0)
                {
                    var extList = System.Text.Json.JsonSerializer.Deserialize<List<FieldBase>>(torgextlist[0].ExtValue, FieldJsonSerializerConfig.FieldOptions);
                    tlist.AddRange(extList);
                }
            }

            return BusResponse<List<FieldBase>>.Success(tlist);
        }
    }
}
