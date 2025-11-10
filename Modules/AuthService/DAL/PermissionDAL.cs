using MyAccess.Aop;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using AuthService.Model;
using System.Threading.Tasks;
using System.Linq;
using Common;

namespace AuthService
{
    public class PermissionDAL : BaseRepository<MZ_RoleScope>
    {
        /// <summary>
        /// 获取角色域
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public virtual async Task<MZ_RoleScope> SelectRoleScope(long roleId, long menuId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_role_scope where RoleID=").AppendParam(roleId).Append(" and MenuId=").AppendParam(menuId)
                    .DoAsync<DoQuerySql<MZ_RoleScope>>()).ToFirst();
            }
        }


        public virtual async Task<List<MZ_RoleScope>> GetUserScopeList(long uid, long orgId, string perm)
        {
            help.AddParam("@UserId", uid);
            string appendwh = "";
            if (orgId > 0)
            {
                appendwh = " and (nur.OrgId=" + orgId + " or nur.OrgId=0)";
            }
            else if (orgId == 0)
            {
                appendwh = " and nur.OrgId=0";
            }
            SqlBuilder sql = new SqlBuilder(help).Append("select s.* from mz_user_role nur left join mz_role ro on nur.RoleID = ro.RoleID left join mz_role_scope s on nur.RoleID=s.RoleID left join mz_menu m on s.MenuId=m.menu_id where nur.UserId=@UserId " + appendwh + " and ro.Status = 0 and m.perms=").AppendParam(perm);
            DoQuerySql<MZ_RoleScope> execsql = await help.DoCommandAsync<DoQuerySql<MZ_RoleScope>>(sql);
            return execsql.ToList();
        }
        public virtual async Task<List<MZ_Menu>> GetRoleMenuTreeByUserId(long uid, long orgId)
        {
            help.AddParam("@UserId", uid);
            string appendOrgFilter = "ur.OrgId=0";
            if (orgId > 0)
            {
                help.AddParam("@OrgId", orgId);
                appendOrgFilter = "(" + appendOrgFilter + " or ur.OrgId=@OrgId)";
            }

            SqlBuilder sql = new SqlBuilder(help).Append(@"select distinct m.menu_id, m.parent_id, m.menu_name,m.name, m.path, m.component, m.query, m.visible, m.status, m.perms, m.is_frame, m.is_cache, m.menu_type, m.icon, m.order_num, m.create_time
        from mz_menu m
        left join mz_role_permission rm on m.menu_id = rm.MenuId 
        left join mz_user_role ur on rm.RoleID = ur.RoleID
        left join mz_role ro on ur.RoleID = ro.RoleID
        where ur.UserId = @UserId and m.menu_type<>'F' and m.status = 0  AND ro.Status = 0 and " + appendOrgFilter);
            DoQuerySql<MZ_Menu> execsql = await help.DoCommandAsync<DoQuerySql<MZ_Menu>>(sql);
            return execsql.ToList();
        }


        /// <summary>
        /// 获取用户当前企业的角色权限
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public virtual async Task<List<Out_RolePermiss>> GetRolePermissionsByUser(long uid, long orgId)
        {
            help.AddParam("@UserId", uid);
            string appendwhere = "nur.OrgId=0";
            if (orgId > 0)
            {
                appendwhere = "(nur.OrgId=" + orgId + " or " + appendwhere + ")";
            }
            SqlBuilder sql = new SqlBuilder(help).Append("select nrr.RoleID,m.perms from mz_role_permission nrr left join mz_menu m on m.menu_id = nrr.MenuId left join mz_user_role nur on nrr.RoleID = nur.RoleID left join mz_role ro on nur.RoleID = ro.RoleID where nur.UserId=@UserId and ro.Status = 0 and " + appendwhere + " and m.status = 0");
            DoQuerySql<Out_RolePermiss> execsql = await help.DoCommandAsync<DoQuerySql<Out_RolePermiss>>(sql);
            return execsql.ToList();
        }

        public virtual async Task<List<string>> GetRolePermissions(long role)
        {
            help.AddParam("@RoleID", role);
            SqlBuilder sql = new SqlBuilder(help).Append("select m.perms from mz_role_permission nrr left join mz_menu m on m.menu_id = nrr.MenuId where nrr.RoleID=@RoleID");
            DoQuerySql<string> execsql = await help.DoCommandAsync<DoQuerySql<string>>(sql);
            return execsql.ToList();
        }
        public virtual async Task<List<MZ_Role>> GetRolesListByOrgId(long orgId)
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select * from mz_role where (OrgId=0 or OrgId=" + orgId + ") and Status = 0");
            DoQuerySql<MZ_Role> execsql = await help.DoCommandAsync<DoQuerySql<MZ_Role>>(sql);
            return execsql.ToList();
        }
        public virtual async Task<List<MZ_Role>> GetRolesListByUser(long uid, long orgId)
        {
            help.AddParam("@UserId", uid);
            string appendwh = "";
            if (orgId > 0)
            {
                appendwh = " and (nur.OrgId=" + orgId + " or nur.OrgId=0)";
            }
            else if (orgId == 0)
            {
                appendwh = " and nur.OrgId=0";
            }
            SqlBuilder sql = new SqlBuilder(help).Append("select ro.* from mz_user_role nur left join mz_role ro on nur.RoleID = ro.RoleID where nur.UserId=@UserId " + appendwh + " and ro.Status = 0");
            DoQuerySql<MZ_Role> execsql = await help.DoCommandAsync<DoQuerySql<MZ_Role>>(sql);
            return execsql.ToList();
        }

        public virtual async Task<List<long>> GetRoleIdsByUser(long uid, long orgId)
        {
            help.AddParam("@UserId", uid);
            string appendwh = "";
            if (orgId > 0)
            {
                appendwh = " and (OrgId=" + orgId + " or OrgId=0)";
            }
            else if (orgId == 0)
            {
                appendwh = " and OrgId=0";
            }
            SqlBuilder sql = new SqlBuilder(help).Append("select RoleID from mz_user_role where UserId=@UserId" + appendwh);
            DoQuerySql<long> execsql = await help.DoCommandAsync<DoQuerySql<long>>(sql);
            return execsql.ToList().Distinct().ToList();
        }


        public virtual async Task<List<long>> GetChildrenDeptIds(long deptId)
        {
            return (await new SqlBuilder(help).Append("SELECT dept_id FROM mz_dept where ancestors like concat((select ancestors from mz_dept where dept_id=" + deptId + "),'%');")
                .DoAsync<DoQuerySql<long>>()).ToList();
        }
        [Trans]
        public virtual async Task SetRolePermissions(long id, long[] menuIds)
        {
            var sql = new SqlBuilder(help).Delete<MZ_Role_Permission>("RoleID=").AppendParam(id);
            await sql.DoAsync<DoExecSql>();
            if (menuIds.Length > 0)
            {
                MZ_Role_Permission[] role_permiss = new MZ_Role_Permission[menuIds.Length];
                for (int i = 0; i < role_permiss.Length; i++)
                {
                    role_permiss[i] = new MZ_Role_Permission();
                    role_permiss[i].RoleID = id;
                    role_permiss[i].MenuId = menuIds[i];
                }
                await new SqlBuilder(help).Insert(role_permiss).DoAsync<DoExecSql>();
            }
        }

        [Trans]
        public virtual async Task<int> DeleteRolePermByMenuId(long menuId)
        {
            var sql = new SqlBuilder(help).Delete<MZ_Role_Permission>("MenuId=").AppendParam(menuId);
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }

    }
}
