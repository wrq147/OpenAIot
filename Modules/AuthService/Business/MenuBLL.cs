
using AuthService.Model;
using Common.IdGenerator;
using Common.Share;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService
{
    public class MenuBLL
    {
        private ITAServiceProvider _provider;
        private MenuDAL _menu;
        private PermissionDAL _permission;
        private SnowflakeHelper _snowflake;
        public MenuBLL(ITAServiceProvider provider, MenuDAL menu, PermissionDAL permission, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _menu = menu;
            _permission = permission;
            _snowflake = snowflake;
        }
        public virtual async Task<BusResponse<string>> UpdateMenu(MZ_Menu menu)
        {
            try
            {
                await _menu.UpdateMenu(menu);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(611, ex.Message);
            }
        }
        public virtual async Task<BusResponse<string>> InsertMenu(MZ_Menu menu)
        {
            try
            {
                menu.query ??= string.Empty;
                menu.menu_id = _snowflake.NextId();
                await _menu.InsertMenu(menu);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(611, ex.Message);
            }
        }
        public virtual async Task<bool> CheckMenuNameUnique(MZ_Menu menu)
        {
            return await _menu.CheckMenuNameUnique(menu);
        }

        public virtual async Task<List<long>> SelectMenuListByRoleId(long roleId)
        {
            return await _menu.SelectMenuListByRoleId(roleId);
        }
        public virtual async Task<List<Out_ScopeItem>> SelectScopeList(long roleId)
        {
            return await _menu.SelectScopeList(roleId);
        }
        public virtual async Task<List<MZ_Menu>> SelectMenuList(string menuName, string visible, string status, IUserInfo user, bool withAlloca = true)
        {
            List<MZ_Menu> menuList = null;
            // 管理员显示所有菜单信息
            var userDAL = _provider.GetService<UserDAL>();
            if (await userDAL.ExistUserRole(1, user.UserId, user.OrgId))
            {
                menuList = await _menu.SelectMenuList(menuName, visible, status);
            }
            else
            {
                menuList = await _menu.SelectMenuListByUserId(menuName, visible, status, user.UserId, user.OrgId, withAlloca);
                menuList.Sort((x, y) =>
                {
                    int rt = (int)(x.parent_id - y.parent_id);
                    if (rt == 0)
                    {
                        return x.order_num.Value - y.order_num.Value;
                    }
                    return rt;
                });
            }
            return menuList;
        }

        public virtual async Task<BusResponse<MZ_Menu>> SelectMenuById(long menuId)
        {
            MZ_Menu m = await _menu.SelectMenuById(menuId);
            if (m == null)
            {
                return BusResponse<MZ_Menu>.Error(3, "数据不存在");
            }
            return BusResponse<MZ_Menu>.Success(m);
        }

        /// <summary>
        /// 构建前端所需要树结构
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual List<MZ_Menu> BuildMenuTree(List<MZ_Menu> menus)
        {
            List<MZ_Menu> returnList = new List<MZ_Menu>();
            HashSet<long> tempList = new HashSet<long>();
            foreach (MZ_Menu dept in menus)
            {
                tempList.Add(dept.menu_id.Value);
            }
            foreach (MZ_Menu menu in menus)
            {
                // 如果是顶级节点, 遍历该父节点的所有子节点
                if (!tempList.Contains(menu.parent_id.Value))
                {
                    _RecursionFn(menus, menu);
                    returnList.Add(menu);
                }
            }

            if (returnList.Count == 0)
            {
                returnList = menus;
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
                if (_GetChildList(list, tChild).Count > 0)
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
        public virtual async Task<bool> HasChildByMenuId(long menuId)
        {
            return await _menu.HasChildByMenuId(menuId);
        }
        [Trans]
        public virtual async Task<BusResponse<string>> DeleteMenuById(long menuId)
        {
            try
            {
                if (await _menu.HasChildByMenuId(menuId))
                {
                    return BusResponse<string>.Error(110, "存在子菜单,不允许删除");
                }

                MZ_Menu menu = await _menu.SelectMenuById(menuId);
                if (menu == null)
                {
                    return BusResponse<string>.Error(111, "菜单不存在");
                }
                await _menu.DeleteMenuById(menuId);
                await _permission.DeleteRolePermByMenuId(menuId);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(112, ex.Message);
            }
        }
    }
}
