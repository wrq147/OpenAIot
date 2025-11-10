using ProducerService.Business;
using ProducerService.Model;
using AuthService.Controller;
using Common.Share;
using Common;
using System;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthService;

namespace ProducerService.Controller
{
    /// <summary>
    /// 代理API
    /// </summary>
    public class Agent : AbstractLoginedController
    {
        private AgentBLL _agentBLL;
        public Agent(AgentBLL agentBLL)
        {
            _agentBLL = agentBLL;
        }
        /// <summary>
        /// 所有代理商
        /// </summary>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_Agent>>> AllList(In_AgentList query)
        {
            return this.Success(await _agentBLL.SelectList(query));
        }

        /// <summary>
        /// 查询下级代理商（代理商用）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_Agent>>> List(In_AgentList query)
        {
            query.ParentOrgId = GetUser().OrgId;
            return this.Success(await _agentBLL.SelectList(query));
        }

        /// <summary>
        /// 获取代理商信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_Agent>> Info(string id)
        {
            return this.Success(await _agentBLL.Info(id, true));
        }

        /// <summary>
        /// 我的授权（代理商用）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_AgentFactory>>> AuthList()
        {
            return this.Success(await _agentBLL.SelectFactory(GetUser().OrgId));
        }
        /// <summary>
        /// 生成邀请（生产商用）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [Des("邀请代理")]
        public async Task<DefaultAjaxResult<string>> AddAllInvite(In_AllInvite data)
        {
            return (await _agentBLL.GenerateAllInvitLink(data)).ToAjaxResult();
        }

        /// <summary>
        /// 加入邀请
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Des("加入邀请")]
        public async Task<DefaultAjaxResult<string>> JoinInvite(In_JoinInvite data)
        {
            return (await _agentBLL.JoinByInvitLink(data)).ToAjaxResult();
        }

        /// <summary>
        /// 取消下级代理的权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancelDown">是否同时取消下级代理</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> CancelProxy(string id, bool cancelDown)
        {
            return (await _agentBLL.CancelProxy(id, cancelDown)).ToAjaxResult();
        }

        /// <summary>
        /// 获取代理商的打印数据源信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<OutCertInfo>> CertInfo(string id = "")
        {
            return (await _agentBLL.CertInfo(id)).ToAjaxResult();
        }
        /// <summary>
        /// 查询邀请记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_AgentFlow>>> List(In_AgentFlowList query)
        {
            return this.Success(await _agentBLL.SelectAgentFlowList(query, GetUser()));
        }


        /// <summary>
        /// 发送邀请短信
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [RepeatableMiddleware]
        public async Task<AjaxResult> SendYqSms(In_SendYqSms data)
        {
            return (await _agentBLL.SendYaoQing(data.tel, data.url, data.code, GetUser())).ToAjaxResult();
        }
    }
}
