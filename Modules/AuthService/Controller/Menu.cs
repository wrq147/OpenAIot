using System.Collections.Generic;
using TemplateAction.Core;
using Common;
using TemplateAction.NetCore;
using Common.Share;
using TemplateAction.Route;
using System.Threading.Tasks;

namespace AuthService.Controller
{
    [About]
    public class Menu : AbstractLoginedController
    {
        private MenuBLL _menu;
        private UserBLL _userBLL;
        public Menu(MenuBLL menu, UserBLL userBLL)
        {
            _menu = menu;
            _userBLL = userBLL;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="visible"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> List(string menuName = null, string visible = null, string status = null)
        {
            IUserInfo dataInfo = GetUser();
            List<MZ_Menu> menus = await _menu.SelectMenuList(menuName, visible, status, dataInfo);
            return this.Success(menus);
        }

        /// <summary>
        /// 获取可过滤功能权限列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ScopeList(long roleId)
        {
            return this.Success(await _menu.SelectScopeList(roleId));
        }
        /// <summary>
        /// 根据菜单编号获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return (await _menu.SelectMenuById(id)).ToAjaxResult();
        }
        /// <summary>
        /// 菜单树转选择树
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private List<TreeSelect<long>> MenuList2Tree(List<MZ_Menu> menuTree)
        {
            List<TreeSelect<long>> treeList = new List<TreeSelect<long>>();
            foreach (MZ_Menu mn in menuTree)
            {
                TreeSelect<long> ts = new TreeSelect<long>();
                ts.id = mn.menu_id.Value;
                ts.label = mn.menu_name;
                if (mn.children != null)
                {
                    ts.children = MenuList2Tree(mn.children);
                }
                treeList.Add(ts);
            }
            return treeList;
        }

        /// <summary>
        /// 获取菜单下拉树列表
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="visible"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> TreeSelect(string menuName = null, string visible = null, string status = null)
        {
            IUserInfo dataInfo = GetUser();
            List<MZ_Menu> menus = await _menu.SelectMenuList(menuName, visible, status, dataInfo, false);
            List<TreeSelect<long>> tlist = MenuList2Tree(_menu.BuildMenuTree(menus));
            return this.Success(tlist);
        }

        /// <summary>
        /// 加载对应角色菜单列表树
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RoleMenuTreeSelect(long id)
        {
            IUserInfo dataInfo = GetUser();
            var tmprole = await _userBLL.GetRole(id);
            if (tmprole == null)
            {
                return this.Error<string>(14, "角色不存在");
            }
            List<MZ_Menu> menus = await _menu.SelectMenuList(null, null, null, dataInfo, tmprole.IsSystem == "1");
            List<TreeSelect<long>> tlist = MenuList2Tree(_menu.BuildMenuTree(menus));
            return this.Success(new
            {
                checkedKeys = await _menu.SelectMenuListByRoleId(id),
                menus = tlist
            });
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [TANetValid]
        [About]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Menu menu)
        {
            if (!await _menu.CheckMenuNameUnique(menu))
            {
                return this.Error<string>(110, "新增菜单'" + menu.menu_name + "'失败，菜单名称已存在");
            }
            menu.path ??= string.Empty;
            menu.component ??= string.Empty;
            menu.scope = 0;
            menu.SetCreateBy(GetUser());
            return (await _menu.InsertMenu(menu)).ToAjaxResult();
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [TANetValid]
        [About]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Menu menu)
        {
            if (!await _menu.CheckMenuNameUnique(menu))
            {
                return this.Error<string>(110, "修改菜单'" + menu.menu_name + "'失败，菜单名称已存在");
            }
            else if (menu.menu_id == menu.parent_id)
            {
                return this.Error<string>(103, "修改菜单'" + menu.menu_name + "'失败，上级菜单不能选择自己");
            }
            menu.scope = null;
            menu.SetUpdateBy(GetUser());
            return (await _menu.UpdateMenu(menu)).ToAjaxResult();
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _menu.DeleteMenuById(id)).ToAjaxResult();
        }
    }
}
