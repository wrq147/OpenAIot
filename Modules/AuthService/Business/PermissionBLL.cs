using AuthService.Model;
using Common;
using Common.Json;
using Common.Share;
using MyAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService
{
    public class PermissionBLL
    {
        private PermissionDAL _permission;
        private UserDAL _user;
        private ITAServiceProvider _provider;
        public PermissionBLL(PermissionDAL permission, UserDAL user, ITAServiceProvider provider)
        {
            _permission = permission;
            _user = user;
            _provider = provider;
        }
        public virtual async Task<BusResponse<int>> UpdateRoleScope(MZ_RoleScope roleScope)
        {
            try
            {
                return BusResponse<int>.Success(await _permission.CreateOrUpdate(roleScope));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(131, ex.Message);
            }
        }

        /// <summary>
        /// 获取角色域
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public virtual async Task<MZ_RoleScope> SelectRoleScope(long roleId, long menuId)
        {
            return await _permission.SelectRoleScope(roleId, menuId);
        }
        /// <summary>
        /// 获取指定用户的角色Id
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<List<long>> GetRoleIdsByUser(long uid, long orgId)
        {
            return await _permission.GetRoleIdsByUser(uid, orgId);
        }

        /// <summary>
        /// 获取指定用户的角色组
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_Role>> GetRolesListByUser(long uid, long orgId)
        {
            var rolelist = await _permission.GetRolesListByUser(uid, orgId);
            var newlist = rolelist.GroupBy(x => x.RoleID.Value).Select(y => y.First());
            return newlist.ToList();
        }
        /// <summary>
        /// 获取指定组织的角色组
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_Role>> GetRolesListByOrgId(long orgId)
        {
            return await _permission.GetRolesListByOrgId(orgId);
        }


        /// <summary>
        /// 获取当前用户的数据权限
        /// </summary>
        /// <param name="perm"></param>
        /// <param name="uid"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<List<DataScope>> GetUserScope(string perm, long uid, long orgId)
        {
            List<DataScope> dataPermiss = new List<DataScope>();
            List<MZ_RoleScope> rolelist = await _permission.GetUserScopeList(uid, orgId, perm);
            if (rolelist.Count == 0)
            {
                DataScope scope = new DataScope();
                scope.DeptList = null;
                scope.UserList = null;
                dataPermiss.Add(scope);
                return dataPermiss;
            }
            List<long> depids = null;
            List<long> childDeptlist = null;
            foreach (MZ_RoleScope r in rolelist)
            {
                DataScope scope = new DataScope();
                switch (r.DataScope)
                {
                    case "1":
                        scope.DeptList = null;
                        scope.UserList = null;
                        break;
                    case "2":
                        scope.DeptList = System.Text.Json.JsonSerializer.Deserialize<List<long>>(r.CustomScope, MyDefaultTextJsonConfig.DefaultOptions);
                        break;
                    case "3":
                        scope.DeptList = new List<long>();
                        if (depids == null)
                        {
                            depids = await _provider.GetService<OrgDAL>().SelectDeptIds(uid, orgId);
                        }
                        scope.DeptList.AddRange(depids);
                        break;
                    case "4":
                        scope.DeptList = new List<long>();
                        if (depids == null)
                        {
                            depids = await _provider.GetService<OrgDAL>().SelectDeptIds(uid, orgId);
                        }
                        scope.DeptList.AddRange(depids);

                        if (childDeptlist == null)
                        {
                            childDeptlist = new List<long>();
                            foreach (var tmid in depids)
                            {
                                var tmpidd = await _permission.GetChildrenDeptIds(tmid);
                                if (tmpidd.Count > 0)
                                {
                                    childDeptlist.AddRange(tmpidd);
                                }
                            }
                        }
                        if (childDeptlist.Count > 0)
                        {
                            scope.DeptList = scope.DeptList.Union(childDeptlist).ToList();
                        }
                        break;
                    case "5":
                        scope.UserList = new List<long>();
                        scope.UserList.Add(uid);
                        break;
                }
                dataPermiss.Add(scope);
            }
            return dataPermiss;
        }

        public virtual async Task ClearUserRolePermissions(long uid, long orgId)
        {
            string tkey = string.Format("Permiss_{0}_{1}", uid, orgId);
            await _provider.GetService<GeneralRedisHelper>().KeyDeleteAsync(tkey);
        }
        public virtual async Task<HashSet<string>> GetUserRolePermissions(long uid, long orgId)
        {
            string tkey = string.Format("Permiss_{0}_{1}", uid, orgId);
            HashSet<string> permiss = await _provider.GetService<GeneralRedisHelper>().StringGetAsync<HashSet<string>>(tkey);
            if (permiss == null)
            {
                List<Out_RolePermiss> roles = await _permission.GetRolePermissionsByUser(uid, orgId);
                permiss = new HashSet<string>();
                foreach (Out_RolePermiss r in roles)
                {
                    if (!permiss.Contains(r.perms))
                    {
                        permiss.Add(r.perms);
                    }
                }
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(tkey, permiss, TimeSpan.FromMinutes(30));
            }

            return permiss;
        }
        public virtual async Task<BusResponse<List<string>>> GetRolePermissions(long role)
        {
            List<string> tlist = await _permission.GetRolePermissions(role);
            return BusResponse<List<string>>.Success(tlist);
        }


        /// <summary>
        /// 根据父节点的ID获取所有子节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<MZ_Menu> _GetChildPerms(List<MZ_Menu> list, int parentId)
        {
            List<MZ_Menu> returnList = new List<MZ_Menu>();
            foreach (MZ_Menu t in list)
            {
                // 一、根据传入的某个父节点ID,遍历该父节点的所有子节点
                if (t.parent_id == parentId)
                {
                    _RecursionFn(list, t);
                    returnList.Add(t);
                }
            }
            return returnList;
        }

        /// <summary>
        /// 递归列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        private void _RecursionFn(List<MZ_Menu> list, MZ_Menu t)
        {
            // 得到子节点列表
            List<MZ_Menu> childList = _GetChildList(list, t);
            t.children = childList;
            foreach (MZ_Menu tChild in childList)
            {
                if (_HasChild(list, tChild))
                {
                    _RecursionFn(list, tChild);
                }
            }
        }

        /// <summary>
        /// 得到子节点列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private List<MZ_Menu> _GetChildList(List<MZ_Menu> list, MZ_Menu t)
        {
            List<MZ_Menu> tlist = new List<MZ_Menu>();
            foreach (MZ_Menu n in list)
            {
                if (n.parent_id == t.menu_id)
                {
                    tlist.Add(n);
                }
            }
            return tlist;
        }

        /// <summary>
        /// 判断是否有子节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool _HasChild(List<MZ_Menu> list, MZ_Menu t)
        {
            return _GetChildList(list, t).Count > 0 ? true : false;
        }
        public virtual async Task<List<MZ_Menu>> GetMenuTreeByUser(long uid)
        {
            var user = _provider.GetUser();
            List<MZ_Menu> menus = await _permission.GetRoleMenuTreeByUserId(uid, user.OrgId);

            menus.Sort((x, y) =>
            {
                int rt = (int)(x.parent_id - y.parent_id);
                if (rt == 0)
                {
                    return x.order_num.Value - y.order_num.Value;
                }
                return rt;
            });
            return _GetChildPerms(menus, 0);
        }

        public List<Out_RouterInfo> BuildMenus(List<MZ_Menu> menus)
        {
            List<Out_RouterInfo> routers = new List<Out_RouterInfo>();
            foreach (MZ_Menu menu in menus)
            {
                Out_RouterInfo router = new Out_RouterInfo();
                router.hidden = "1".Equals(menu.visible);
                router.name = _GetRouteName(menu);
                router.path = _GetRouterPath(menu);
                router.component = _GetComponent(menu);
                router.query = menu.query;
                if (_IsInnerLink(menu))
                {
                    router.meta = new Out_MetaInfo(menu.menu_name, menu.icon, menu.is_cache == 1, menu.component);
                }
                else
                {
                    router.meta = new Out_MetaInfo(menu.menu_name, menu.icon, menu.is_cache == 1, menu.path);
                }

                List<MZ_Menu> cMenus = menu.children;
                if (cMenus != null && cMenus.Count > 0 && "M".Equals(menu.menu_type))
                {
                    router.alwaysShow = true;
                    router.redirect = "noRedirect";
                    router.children = BuildMenus(cMenus);
                }
                routers.Add(router);
            }
            return routers;
        }


        /// <summary>
        /// 获取路由名称
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public string _GetRouteName(MZ_Menu menu)
        {
            if (string.IsNullOrEmpty(menu.name))
            {
                return StringTool.Capitalize(menu.path);
            }
            else
            {
                return menu.name;
            }

        }

        /// <summary>
        /// 获取路由地址
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public string _GetRouterPath(MZ_Menu menu)
        {
            string routerPath = menu.path;
            // 内链打开外网方式
            if (_IsInnerLink(menu))
            {
                routerPath = routerPath.Replace("http://", "", true, System.Globalization.CultureInfo.CurrentCulture);
                routerPath = routerPath.Replace("https://", "", true, System.Globalization.CultureInfo.CurrentCulture);
            }
            else if (0 == menu.parent_id && "M".Equals(menu.menu_type) && !menu.path.IsHttp())
            {
                // 非外链并且是一级目录（类型为目录）
                routerPath = "/" + menu.path;
            }
            return routerPath;
        }

        /// <summary>
        /// 获取组件信息
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        private string _GetComponent(MZ_Menu menu)
        {
            string component = "Layout";
            if (_IsInnerLink(menu))
            {
                component = "InnerLink";
            }
            else if (_IsParentView(menu))
            {
                component = "ParentView";
            }
            else if (!string.IsNullOrEmpty(menu.component))
            {
                component = menu.component;
            }
            return component;
        }


        /// <summary>
        /// 是否为内链组件
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public bool _IsInnerLink(MZ_Menu menu)
        {
            return menu.is_frame == 1;
        }


        /// <summary>
        /// 是否为parent_view组件
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public bool _IsParentView(MZ_Menu menu)
        {
            return menu.parent_id != 0 && "M".Equals(menu.menu_type);
        }

    }
}
