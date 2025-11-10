using AuthService;
using AuthService.Controller;
using CardService.Business;
using Common;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace CardService.Controller
{
    /// <summary>
    /// 名片专用用户管理API
    /// </summary>
    public class User : AbstractLoginedController
    {
        private UserBLL _usrBLL;
        private CardBLL _cardBLL;
        public User(UserBLL usr, CardBLL cardBLL, OrgBLL orgBLL)
        {
            _usrBLL = usr;
            _cardBLL = cardBLL;
        }
        [HttpGet]
        public async Task<AjaxResult> List(In_UserList query = null)
        {
            if (query == null)
            {
                query = new In_UserList();
            }
            return this.Success(await _usrBLL.GetUserList(query));
        }

        [HttpGet]
        public async Task<AjaxResult> UId2CardId(long id)
        {
            return this.Success(await _cardBLL.SelectByCAId(id));
        }
    }
}
