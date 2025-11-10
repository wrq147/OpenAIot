using MyAccess.Aop;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using Common.Share;
using Common;
using System.Threading.Tasks;
using TemplateAction.Core;
using AuthService.Model;
using Common.DataAc;

namespace AuthService
{
    public class UserDAL : BaseRepository<MZ_AdminInfo>
    {
        public async Task<int> DoAsync(ActionChangeData evt, List<ActionCondition> list1, List<ActionInfo> list2)
        {
            using DbHelp db = CreateDB();
            return await evt.DoAsync(db, list1, list2);
        }
        public virtual async Task<List<MZ_AdminInfo>> SearchUsers(string key)
        {
            return await new SqlBuilder(help).Query<MZ_AdminInfo>().Append("select Id,UserName,RealName,Mobile,Avatar,Email from  mz_admin where Id>10 and del_flag = '0' and status='0' and (RealName=")
                .AppendParam(key).Append(" or UserName=").AppendParam(key).Append(" or Mobile=").AppendParam(key).Append(" or Email=").AppendParam(key).Append(")")
                .ToListAsync();
        }
        public virtual async Task<PageObject<MZ_AdminInfo>> GetUserALlList(In_UserAllList query)
        {
            var tsql = new SqlBuilder(help).Query<MZ_AdminInfo>().Append("select u.* from  mz_admin u where u.del_flag = '0'")
               .Then(!string.IsNullOrEmpty(query.status), sql =>
               {
                   sql.Append(" AND u.status=").AppendParam(query.status);
               })
               .Then(query.beginTime != null, sql =>
               {
                   sql.Append(" and u.create_time >= ").AppendParam(query.beginTime);
               })
               .Then(query.endTime != null, sql =>
               {
                   sql.Append(" and u.create_time <= ").AppendParam(query.endTime);
               })
               .Then(!string.IsNullOrEmpty(query.key), c =>
               {
                   string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.key) + "%";
                   c.Append(" AND (u.UserName like ").AppendParam(tmpkey)
                   .Append(" or u.Mobile like ").AppendParam(tmpkey)
                   .Append(" or u.RealName like ").AppendParam(tmpkey)
                   .Append(" or EXISTS(select o.Id from mz_user_org uo left join mz_org o on uo.OrgId=o.Id where uo.UserId=u.Id and o.OrgName like ").AppendParam(tmpkey).Append("))");
               });
            return await tsql.GeneratePageObjectAsync(query, "u.Id desc");
        }
        public virtual async Task<List<MZ_AdminInfo>> GetUserListByIds(List<long> ids)
        {
            return await new SqlBuilder(help).Query<MZ_AdminInfo>().Where(x => ids.Contains(x.Id.Value)).ToListAsync();
        }
        public virtual async Task<List<MZ_AdminInfo>> GetMembersByOrgId(long orgId)
        {
            var tsql = new SqlBuilder(help).Query<MZ_AdminInfo>().Append("select u.*,uo.dept_id,uo.post_name,d.dept_name,uo.IsLeader,uo.IsPrimary from  mz_user_org uo left join mz_admin u  on u.Id=uo.UserId left join mz_dept d on uo.dept_id = d.dept_id  where u.del_flag = '0' and uo.OrgId=").AppendParam(orgId);
            return await tsql.ToListAsync();
        }
        public virtual async Task<PageObject<MZ_AdminInfo>> GetUserList(In_UserList query, DataScope scope)
        {
            ITAContext context = Provider.GetService<ITAContext>();

            var tsql = new SqlBuilder(help).Query<MZ_AdminInfo>().Append("select u.*,uo.dept_id,uo.post_name,d.dept_name,uo.IsLeader,uo.IsPrimary from  mz_user_org uo left join mz_admin u  on u.Id=uo.UserId left join mz_dept d on uo.dept_id = d.dept_id  where u.del_flag = '0'")
                .Then(query.orgId != null, sql =>
                {
                    sql.Append(" and uo.OrgId=").AppendParam(query.orgId);
                })
                .Then(query.isPrimaryDept != null, sql =>
                {
                    sql.Append(" AND uo.IsPrimary=").AppendParam(query.isPrimaryDept);
                })
                .Then(query.deptIdList != null && query.deptIdList.Length > 0, sql =>
                {
                    sql.Append(" AND uo.dept_id in (").AppendParam(query.deptIdList).Append(")");
                })
                .Then(query.userId != null, sql =>
                {
                    sql.Append(" AND uo.UserId=").AppendParam(query.userId);
                })
                .Then(!string.IsNullOrEmpty(query.status), sql =>
                {
                    sql.Append(" AND u.status=").AppendParam(query.status);
                })
                .Then(!string.IsNullOrEmpty(query.userName), sql =>
                {
                    sql.Append(" AND u.UserName like concat('%',").AppendParam(query.userName).Append(", '%')");
                })
                .Then(!string.IsNullOrEmpty(query.phonenumber), sql =>
                {
                    sql.Append(" AND u.Mobile like concat('%',").AppendParam(query.phonenumber).Append(", '%')");
                })
                .Then(query.beginTime != null, sql =>
                {
                    sql.Append(" and u.create_time >= ").AppendParam(query.beginTime);
                })
                .Then(query.endTime != null, sql =>
                {
                    sql.Append(" and u.create_time <= ").AppendParam(query.endTime);
                })
                .Then(!string.IsNullOrEmpty(query.depAncestors), sql =>
                {
                    sql.Append(" and d.ancestors like concat(").AppendParam(query.depAncestors).Append(", '%')");
                })
                .Then(query.deptId != null, sql =>
                {
                    sql.Append(" AND uo.dept_id=").AppendParam(query.deptId);
                })
                .Then(!string.IsNullOrEmpty(query.key), c =>
                {
                    c.Append(" AND (u.UserName like concat('%',").AppendParam(query.key).Append(", '%')")
                    .Append(" or u.Mobile like concat('%',").AppendParam(query.key).Append(", '%')")
                    .Append(" or u.RealName like concat('%',").AppendParam(query.key).Append(", '%'))");
                }).Then(scope != null, sql =>
                {
                    sql.Append(scope.GenerateFilter("uo.dept_id", "u.Id"));
                });

            return await tsql.GeneratePageObjectAsync(query, "u.Id desc");
        }

        public virtual async Task<MZ_AdminInfo> GetAdminByName(string username)
        {
            help.AddParam("@UserName", username);
            SqlBuilder sql = new SqlBuilder(help).Append("select * from mz_admin_v where UserName=@UserName and del_flag='0'");
            DoQuerySql<MZ_AdminInfo> execsql = await help.DoCommandAsync<DoQuerySql<MZ_AdminInfo>>(sql);
            return execsql.ToFirst();
        }
        public virtual async Task<MZ_AdminInfo> GetAdminByActiveEmail(string email)
        {
            help.AddParam("@Email", email);
            SqlBuilder sql = new SqlBuilder(help).Append("select * from mz_admin_v where Email=@Email and del_flag='0' and EmailActive=1");
            DoQuerySql<MZ_AdminInfo> execsql = await help.DoCommandAsync<DoQuerySql<MZ_AdminInfo>>(sql);
            return execsql.ToFirst();
        }
        public virtual async Task<MZ_AdminInfo> GetAdminByEmail(string email)
        {
            help.AddParam("@Email", email);
            SqlBuilder sql = new SqlBuilder(help).Append("select * from mz_admin_v where Email=@Email and del_flag='0'");
            DoQuerySql<MZ_AdminInfo> execsql = await help.DoCommandAsync<DoQuerySql<MZ_AdminInfo>>(sql);
            return execsql.ToFirst();
        }
        public virtual async Task<MZ_AdminInfo> GetAdminByMobile(string mobile)
        {
            help.AddParam("@Mobile", mobile);
            SqlBuilder sql = new SqlBuilder(help).Append("select * from mz_admin_v where Mobile=@Mobile and del_flag='0'");
            DoQuerySql<MZ_AdminInfo> execsql = await help.DoCommandAsync<DoQuerySql<MZ_AdminInfo>>(sql);
            return execsql.ToFirst();
        }
        public virtual async Task<MZ_AdminInfo> GetAdminById(long uid)
        {
            help.AddParam("@Id", uid);
            SqlBuilder sql = new SqlBuilder(help).Append("select uv.*,d.dept_name from mz_admin_v uv left join mz_dept d on uv.dept_id = d.dept_id where uv.Id=@Id");
            DoQuerySql<MZ_AdminInfo> execsql = await help.DoCommandAsync<DoQuerySql<MZ_AdminInfo>>(sql);
            return execsql.ToFirst();
        }
        public virtual async Task<MZ_AdminInfo> GetAdminByOrgId(long uid, long orgId)
        {
            return await new SqlBuilder(help).Query<MZ_AdminInfo>().Append("select u.*,uo.dept_id,uo.post_name,d.dept_name from  mz_user_org uo left join mz_admin u  on u.Id=uo.UserId left join mz_dept d on uo.dept_id = d.dept_id  where uo.IsPrimary=1 and u.del_flag = '0' and uo.OrgId=")
           .AppendParam(orgId).Append(" and u.Id=").AppendParam(uid).ToFirstAsync();
        }
        public virtual async Task<PageObject<MZ_AdminInfo>> SearchByKey(string key, long orgId, int page, int pageSize, DataScope scope)
        {
            var dqs = new SqlBuilder(help).Query<MZ_AdminInfo>().Append("select u.*,uo.dept_id,uo.post_name,d.dept_name,uo.IsLeader,uo.IsPrimary from  mz_user_org uo left join mz_admin u on u.Id=uo.UserId left join mz_dept d on uo.dept_id = d.dept_id  where u.del_flag = '0' and uo.OrgId=").AppendParam(orgId);
            dqs.Then(!string.IsNullOrEmpty(key), c =>
            {
                c.Append(" AND (u.UserName like concat('%',").AppendParam(key).Append(", '%')")
                .Append(" or u.Mobile like concat('%',").AppendParam(key).Append(", '%')")
                .Append(" or u.RealName like concat('%',").AppendParam(key).Append(", '%'))");
            }).Then(scope != null, sql =>
            {
                sql.Append(scope.GenerateFilter("uo.dept_id", "u.Id"));
            });

            BaseQueryParam bqp = new BaseQueryParam();
            bqp.pageNum = page;
            bqp.pageSize = pageSize;
            bqp.showAll = true;
            return await dqs.GeneratePageObjectAsync(bqp);
        }
        public virtual async Task<long> AddUser(MZ_AdminInfo user)
        {
            if (string.IsNullOrEmpty(user.RealName))
            {
                user.RealName = user.UserName;
            }
            var sql = new SqlBuilder(help).Insert(user);
            return (await sql.DoReturnIdentityAsync()).LastInsertedId;
        }
        public virtual async Task<int> UpdateUser(MZ_AdminInfo user)
        {
            var sql = new SqlBuilder(help).Update(user);
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }
        public virtual int DeleteUser(long uid)
        {
            return new SqlBuilder(help).Append("update mz_admin set del_flag = '2' where Id = ").AppendParam(uid).Do<DoExecSql>().RowCount;
        }
        public virtual async Task<MZ_AdminInfo> CheckEmailUnique(string email)
        {
            return (await new SqlBuilder(help).Append("select Id, Email from mz_admin where del_flag = '0' and Email = ").AppendParam(email).Append(" limit 1").DoAsync<DoQuerySql<MZ_AdminInfo>>()).ToFirst();
        }
        public virtual async Task<MZ_AdminInfo> CheckPhoneUnique(string mobile)
        {
            return (await new SqlBuilder(help).Append("select Id, Mobile from mz_admin where del_flag = '0' and Mobile = ").AppendParam(mobile).Append(" limit 1").DoAsync<DoQuerySql<MZ_AdminInfo>>()).ToFirst();
        }
        public virtual async Task<MZ_AdminInfo> CheckUserNameUnique(string username)
        {
            return (await new SqlBuilder(help).Append("select Id, UserName from mz_admin where del_flag = '0' and UserName = ").AppendParam(username).Append(" limit 1").DoAsync<DoQuerySql<MZ_AdminInfo>>()).ToFirst();
        }
        public virtual async Task<int> AddUserRoleItem(MZ_UserRole userRole)
        {
            return (await new SqlBuilder(help).Insert(userRole).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> AddUserRole(List<MZ_UserRole> list)
        {
            return (await new SqlBuilder(help).Insert(list).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> DeleteUserRoleByUserRole(MZ_UserRole userRole)
        {
            return (await new SqlBuilder(help).Delete<MZ_UserRole>("RoleID=").AppendParam(userRole.RoleID).Append(" and UserId=").AppendParam(userRole.UserId).Append(" and OrgId=").AppendParam(userRole.OrgId).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> DeleteUserRoleByRoleAndUserId(long roleId, long[] userIds, long orgId)
        {
            return (await new SqlBuilder(help).Delete<MZ_UserRole>("RoleID=").AppendParam(roleId).Append(" and OrgId=").AppendParam(orgId).Append(" and UserId in (").AppendParam(userIds).Append(")").DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> DeleteUserRoleByUserId(long uid, long orgId)
        {
            return (await new SqlBuilder(help).Delete<MZ_UserRole>("UserId=").AppendParam(uid).Append(" and OrgId=").AppendParam(orgId).DoAsync<DoExecSql>()).RowCount;
        }

        public virtual async Task<int> DeleteUserRoleByOrg(long orgId)
        {
            return (await new SqlBuilder(help).Delete<MZ_UserRole>("OrgId=").AppendParam(orgId).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> DeleteUserRoleByOrg(long roleId, long orgId)
        {
            return (await new SqlBuilder(help).Delete<MZ_UserRole>("RoleID=").AppendParam(roleId).Append(" and OrgId=").AppendParam(orgId).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> DeleteUserRoleById(long roleid, long uid, long orgId)
        {
            return (await new SqlBuilder(help).Delete<MZ_UserRole>("RoleID=").AppendParam(roleid).Append(" and UserId=").AppendParam(uid).Append(" and OrgId=").AppendParam(orgId).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> DeleteUserMan(long uid, long orgId)
        {
            return (await new SqlBuilder(help).Delete<MZ_UserRole>("RoleID=2 and UserId=").AppendParam(uid).Append(" and OrgId=").AppendParam(orgId).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<bool> ExistUserRole(long roleid, long uid, long orgId)
        {
            return (await new SqlBuilder(help).Query<MZ_UserRole>().SomeAsync(x => x.RoleID == roleid && x.UserId == uid && x.OrgId == orgId));
        }
        public virtual async Task<List<long>> SelectManUserIds(long orgId)
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select UserId from mz_user_role where RoleID=2 and OrgId=").AppendParam(orgId);
            DoQuerySql<long> execsql = await help.DoCommandAsync<DoQuerySql<long>>(sql);
            return execsql.ToList();
        }
        public virtual async Task<List<MZ_AdminInfo>> SelectManUsers(long orgId)
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select u.* from mz_user_role r left join mz_admin u on r.UserId=u.Id where r.RoleID=2 and r.OrgId=").AppendParam(orgId);
            DoQuerySql<MZ_AdminInfo> execsql = await help.DoCommandAsync<DoQuerySql<MZ_AdminInfo>>(sql);
            return execsql.ToList();
        }
        public virtual async Task<List<MZ_AdminInfo>> SelectUsersFrom(long roleId, long orgId)
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select u.* from mz_user_role r left join mz_admin u on r.UserId=u.Id where r.RoleID=").AppendParam(roleId).Append(" and r.OrgId=").AppendParam(orgId);
            DoQuerySql<MZ_AdminInfo> execsql = await help.DoCommandAsync<DoQuerySql<MZ_AdminInfo>>(sql);
            return execsql.ToList();
        }
        public virtual async Task<PageObject<MZ_AdminInfo>> SelectAllocatedList(In_UserRoleList query)
        {
            var tsql = new SqlBuilder(help).Query<MZ_AdminInfo>().Append("select u.* from mz_admin_v u left join mz_dept d on u.dept_id = d.dept_id inner join mz_user_role ur on u.Id = ur.UserId and ur.RoleID =").AppendParam(query.roleId).Append(" and ur.OrgId=").AppendParam(query.orgId)
                .Append(" left join mz_role r on r.RoleID = ur.RoleID where u.del_flag = '0'")
                .Then(!string.IsNullOrEmpty(query.userName), sql =>
                {
                    sql.Append(" AND u.UserName like concat('%',").AppendParam(query.userName).Append(", '%')");
                })
                .Then(!string.IsNullOrEmpty(query.key), sql =>
                {
                    sql.Append(" AND (u.UserName like concat('%',").AppendParam(query.key).Append(", '%')")
            .Append(" or u.Mobile like concat('%',").AppendParam(query.key).Append(", '%')")
            .Append(" or u.RealName like concat('%',").AppendParam(query.key).Append(", '%'))");
                })
                .Then(!string.IsNullOrEmpty(query.phonenumber), sql =>
                {
                    sql.Append(" AND u.Mobile like concat('%',").AppendParam(query.phonenumber).Append(", '%')");
                });
            return await tsql.GeneratePageObjectAsync(query);
        }
        public virtual async Task<PageObject<MZ_AdminInfo>> SelectUnallocatedList(In_UserRoleList query)
        {
            var tsql = new SqlBuilder(help).Query<MZ_AdminInfo>().Append("select u.* from mz_admin_v u left join mz_dept d on u.dept_id = d.dept_id where u.del_flag = '0' and EXISTS(select Id from mz_user_org where u.Id=UserId and OrgId=").AppendParam(query.orgId).Append(") and not EXISTS(select ur.UserId from mz_user_role ur where u.Id=ur.UserId and ur.RoleID=").AppendParam(query.roleId).Append(" and ur.OrgId=").AppendParam(query.orgId).Append(")")
                .Then(!string.IsNullOrEmpty(query.userName), sql =>
                {
                    sql.Append(" AND u.UserName like concat('%',").AppendParam(query.userName).Append(", '%')");
                })
                .Then(!string.IsNullOrEmpty(query.key), sql =>
                {
                    sql.Append(" AND (u.UserName like concat('%',").AppendParam(query.key).Append(", '%')")
.Append(" or u.Mobile like concat('%',").AppendParam(query.key).Append(", '%')")
.Append(" or u.RealName like concat('%',").AppendParam(query.key).Append(", '%'))");
                })
                .Then(!string.IsNullOrEmpty(query.phonenumber), sql =>
                {
                    sql.Append(" AND u.Mobile like concat('%',").AppendParam(query.phonenumber).Append(", '%')");
                });

            return await tsql.GeneratePageObjectAsync(query);
        }
        public virtual async Task<List<MZ_Role>> SelectSystemRoles()
        {
            var dsql = new SqlBuilder(help).Query<MZ_Role>().Append("select * from mz_role where IsSystem='1'");
            return await dsql.ToListAsync();
        }
        public virtual async Task<PageObject<MZ_Role>> SelectRoleList(In_RoleList query, IUserInfo curuser)
        {
            string orgwhere = string.Empty;
            if (curuser.OrgId > 0)
            {
                if (curuser.OrgId == 1)
                {
                    orgwhere = "(OrgId=0 or OrgId=" + curuser.OrgId + ")";
                }
                else
                {
                    orgwhere = "((OrgId=0 and EXISTS(select * from mz_user_role where UserId=" + curuser.UserId + " and RoleID=mz_role.RoleID and OrgId=" + curuser.OrgId + ")) or OrgId=" + curuser.OrgId + ")";
                }
            }
            else
            {
                return PageObject<MZ_Role>.Empty();
            }
            var dsql = new SqlBuilder(help).Query<MZ_Role>().Append("select * from mz_role where ").Append(orgwhere)
.Then(!string.IsNullOrEmpty(query.roleName), sql =>
{
    sql.Append(" and RoleName like concat('%', ").AppendParam(query.roleName).Append(", '%')");
})
.Then(!string.IsNullOrEmpty(query.status), sql =>
{
    sql.Append(" and Status=").AppendParam(query.status);
})
            .Then(query.beginTime != null, sq => sq.Append(" and create_time >= ").AppendParam(query.beginTime))
            .Then(query.endTime != null, sq => sq.Append(" and create_time <= ").AppendParam(query.endTime));

            return await dsql.GeneratePageObjectAsync(query, "RoleID desc");
        }

        public virtual async Task<List<MZ_Role>> GetRoleListByIds(List<long> ids)
        {
            return await new SqlBuilder(help).Query<MZ_Role>().Where(x => ids.Contains(x.RoleID.Value)).ToListAsync();
        }
        public virtual async Task<List<long>> SelectUserByRoles(List<long> roles)
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select UserId from mz_user_role ur where ur.RoleID in (").AppendParam(roles).Append(")");
            return (await sql.DoAsync<DoQuerySql<long>>()).ToList();
        }
        public virtual async Task<List<Out_UserRole>> SelectUserRoleList(long uid)
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select ur.*,r.RoleName,r.IsSystem,r.RoleDesc,o.OrgName from mz_user_role ur left join mz_role r on ur.RoleID=r.RoleID  left join mz_org o on ur.OrgId=o.Id where ur.UserId=").AppendParam(uid);
            return (await sql.DoAsync<DoQuerySql<Out_UserRole>>()).ToList();
        }
        public virtual async Task<MZ_Role> GetRole(long id)
        {
            help.AddParam("@RoleID", id);
            SqlBuilder sql = new SqlBuilder(help).Append("select * from mz_role where RoleID=@RoleID");
            DoQuerySql<MZ_Role> dqs = await help.DoCommandAsync<DoQuerySql<MZ_Role>>(sql);
            return dqs.ToFirst();
        }


        [Trans]
        public virtual async Task<long> AddRole(MZ_Role role)
        {
            return (await new SqlBuilder(help).Insert(role).DoReturnIdentityAsync()).LastInsertedId;
        }
        [Trans]
        public virtual async Task<int> UpdateRole(MZ_Role role)
        {
            return (await new SqlBuilder(help).Update(role).DoAsync<DoExecSql>()).RowCount;
        }
        [Trans]
        public virtual async Task DeleteRole(long id)
        {
            await new SqlBuilder(help).Delete<MZ_Role>("RoleID=").AppendParam(id).DoAsync<DoExecSql>();
        }

    }
}
