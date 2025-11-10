using Common.Share;
using ProducerService.Business;
using ProducerService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.NetCore;
using TemplateAction.Route;
using Common;
using Minio;
using AuthService;
namespace ProducerService.Controller
{
    public class AgentInvite : TANetController
    {
        private AgentBLL _agentBLL;
        public AgentInvite(AgentBLL agentBLL)
        {
            _agentBLL = agentBLL;
        }
        /// <summary>
        /// 获取邀请码信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_AgentFlow>> Info(string id)
        {
            return this.Success(await _agentBLL.AgentFlowInfo(id));
        }
        /// <summary>
        /// 手机邀请码注册用户
        /// </summary>
        /// <param name="data"></param>
        /// <returns>返回登录的信息</returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Login>> Reg(In_RegTelYqData data)
        {
            return (await _agentBLL.RegByYq(data, Context)).ToAjaxResult();
        }
        /// <summary>
        /// 手机邀请登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Login>> Login(In_YqLogin data)
        {
            return (await _agentBLL.LoginByYq(data, Context)).ToAjaxResult();
        }
    }
}
