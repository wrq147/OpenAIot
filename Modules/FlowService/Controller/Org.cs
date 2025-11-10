
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using Common;
using AuthService.Controller;
using TemplateAction.Route;
using FlowService.Model;
using System.Threading.Tasks;
using AuthService;

namespace FlowService.Controller
{
    /// <summary>
    /// 获取流程设计所需的组织数据
    /// </summary>
    public class Org : AbstractLoginedController
    {
        private Business.OrgBLL _orgBLL;
        private UserBLL _userBLL;
        public Org(Business.OrgBLL orgBLL, UserBLL userBLL)
        {
            _orgBLL = orgBLL;
            _userBLL = userBLL;
        }
        [HttpGet]
        public async Task<AjaxResult> Tree(In_OrgList query = null)
        {
            if (query == null) query = new In_OrgList();
            var user = Data_ServerTokenInfo.From(Context);
            query.orgId = user.OrgId;
            return this.Success(await _orgBLL.GetTree(query));
        }
        [HttpGet]
        public async Task<AjaxResult> Search(string realName)
        {
            return this.Success(await _orgBLL.SearchUserList(realName));
        }
    }
}
