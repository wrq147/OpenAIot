using CRMService.Business;
using CRMService.Model;
using AuthService.Controller;
using Common.Share;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace CRMService.Controller
{
    /// <summary>
    /// 代理API
    /// </summary>
    public class Agent : AbstractLoginedController
    {
        private CRMAgentBLL _agentBLL;
        public Agent(CRMAgentBLL agentBLL)
        {
            _agentBLL = agentBLL;
        }
        /// <summary>
        /// 生成邀请（代理用）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [Des("邀请客户")]
        public async Task<DefaultAjaxResult<string>> AddInvite(In_Invite data)
        {
            return (await _agentBLL.GenerateInvitLink(data)).ToAjaxResult();
        }

    }
}
