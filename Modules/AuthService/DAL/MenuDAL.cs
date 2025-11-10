using MyAccess.DB;
using System.Collections.Generic;
using AuthService.Model;
using System.Threading.Tasks;
using Common;

namespace AuthService
{
    public class MenuDAL : BaseDbSupport
    {
        public virtual async Task<List<Out_ScopeItem>> SelectScopeList(long roleId)
        {
            SqlBuilder sql = new SqlBuilder(help).Append(@"select m.menu_name,m.menu_id,s.DataScope from mz_role_permission r left join mz_menu m on r.MenuId=m.menu_id left join mz_role_scope s on s.MenuId=m.menu_id and s.RoleID=r.RoleID where m.scope=1 and r.RoleID=").AppendParam(roleId);
            return (await help.DoCommandAsync<DoQuerySql<Out_ScopeItem>>(sql)).ToList();
        }
        public virtual async Task<List<MZ_Menu>> SelectMenuListByUserId(string menuName, string visible, string status, long uid, long orgId, bool withAlloca)
        {
            help.AddParam("@UserId", uid);
            help.AddParam("@OrgId", orgId);
            string twhere = string.Empty;
            if (!string.IsNullOrEmpty(menuName))
            {
                help.AddParam("@MenuName", menuName);
                twhere += " AND m.menu_name like concat('%', @MenuName, '%')";
            }
            if (!string.IsNullOrEmpty(visible))
            {
                help.AddParam("@Visible", visible);
                twhere += " AND m.visible = @Visible";
            }
            if (!string.IsNullOrEmpty(status))
            {
                help.AddParam("@Status", status);
                twhere += " AND m.status = @Status";
            }
            if (!withAlloca)
            {
                twhere += " AND ro.NoAlloca <> '1'";
            }
            SqlBuilder sql = new SqlBuilder(help).Append(@"select distinct m.menu_id, m.parent_id, m.menu_name, m.path, m.component, m.query, m.visible, m.status, m.perms, m.is_frame, m.is_cache, m.menu_type, m.icon, m.order_num, m.create_time
		from mz_menu m
		left join mz_role_permission rm on m.menu_id = rm.MenuId 
        left join mz_user_role ur on rm.RoleID = ur.RoleID
        left join mz_role ro on ur.RoleID = ro.RoleID
		where ur.UserId = @UserId and (ur.OrgId=@OrgId or ur.OrgId=0) " + twhere);

            DoQuerySql<MZ_Menu> execsql = await help.DoCommandAsync<DoQuerySql<MZ_Menu>>(sql);
            return execsql.ToList();
        }

        public virtual async Task<List<MZ_Menu>> SelectMenuList(string menuName, string visible, string status)
        {
            string twhere = string.Empty;
            if (!string.IsNullOrEmpty(menuName))
            {
                help.AddParam("@MenuName", menuName);
                twhere += " AND menu_name like concat('%', @MenuName, '%')";
            }
            if (!string.IsNullOrEmpty(visible))
            {
                help.AddParam("@Visible", visible);
                twhere += " AND visible = @Visible";
            }
            if (!string.IsNullOrEmpty(status))
            {
                help.AddParam("@Status", status);
                twhere += " AND status = @Status";
            }
            SqlBuilder sql = new SqlBuilder(help).Append(@"select menu_id, menu_name, parent_id, order_num, path, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_time 
		from mz_menu where 1=1 " + twhere + " order by parent_id, order_num");
            DoQuerySql<MZ_Menu> execsql = await help.DoCommandAsync<DoQuerySql<MZ_Menu>>(sql);
            return execsql.ToList();
        }

        public virtual async Task<MZ_Menu> SelectMenuById(long menuId)
        {
            help.AddParam("@MenuId", menuId);
            SqlBuilder sql = new SqlBuilder(help).Append(@"select menu_id, menu_name, parent_id, order_num, path, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_time from mz_menu where menu_id = @MenuId");
            DoQuerySql<MZ_Menu> execsql = await help.DoCommandAsync<DoQuerySql<MZ_Menu>>(sql);
            return execsql.ToFirst();
        }
        public virtual async Task<List<long>> SelectMenuListByRoleId(long roleId)
        {
            help.AddParam("@RoleID", roleId);
            SqlBuilder sql = new SqlBuilder(help).Append(@"select m.menu_id from mz_menu m left join mz_role_permission rm on m.menu_id = rm.MenuId where rm.RoleID = @RoleID order by m.parent_id, m.order_num");
            DoQuerySql<long> dqs = await help.DoCommandAsync<DoQuerySql<long>>(sql);
            return dqs.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inmenu"></param>
        /// <returns>返回false表示唯一</returns>
        public virtual async Task<bool> CheckMenuNameUnique(MZ_Menu inmenu)
        {
            help.AddParam("@menuName", inmenu.menu_name);
            help.AddParam("@parentId", inmenu.parent_id);
            var sql = new SqlBuilder(help).Query<long>().Append(@"select menu_id from mz_menu where menu_name=@menuName and parent_id = @parentId").Take(1);
            DoQuerySql<long> execsql = await sql.DoAsync<DoQuerySql<long>>();

            if (execsql.Count > 0)
            {
                long getmenuid = execsql.ToFirst();
                if (inmenu.menu_id != getmenuid)
                {
                    return false;
                }
            }
            return true;
        }
        public virtual async Task<int> InsertMenu(MZ_Menu menu)
        {
            return (await new SqlBuilder(help).Insert(menu).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> UpdateMenu(MZ_Menu menu)
        {
            return (await new SqlBuilder(help).Update(menu).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<bool> HasChildByMenuId(long menuId)
        {
            help.AddParam("@menuId", menuId);
            var sql = new SqlBuilder(help).Query<long>().Append("select menu_id from mz_menu where parent_id = @menuId").Take(1);
            DoQuerySql<long> dqs = await sql.DoAsync<DoQuerySql<long>>();
            return dqs.Count > 0;
        }
        public virtual async Task<int> DeleteMenuById(long menuId)
        {
            var sql = new SqlBuilder(help).Delete<MZ_Menu>("menu_id=").AppendParam(menuId);
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }
    }
}
