
using System.Collections.Generic;
using TemplateAction.Core;
using Common;
using Common.Share;
using TemplateAction.Route;
using System.Threading.Tasks;

namespace AuthService.Controller
{
    public class Permission : AbstractLoginedController
    {
        private PermissionBLL _permission;
        public Permission(PermissionBLL permission)
        {
            _permission = permission;
        }

        /// <summary>
        /// 获取路由信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> GetRouters()
        {
            IUserInfo dataInfo = GetUser();
            List<MZ_Menu> menus = await _permission.GetMenuTreeByUser(dataInfo.UserId);
            return this.Success(_permission.BuildMenus(menus));
        }

    }
}
