using AuthService.Controller;
using Common.Share;
using MessageService.Business;
using MessageService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace MessageService.Controller
{
    public class Client : AbstractLoginedController
    {
        private PushBLL _pushBLL;
        public Client(PushBLL pushBLL)
        {
            _pushBLL = pushBLL;
        }
        /// <summary>
        /// 上报当前用户的App客户端Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> UpClientId(string clientId)
        {
            MZ_PushClient client = new MZ_PushClient();
            client.ClientId = clientId;
            client.UserId = GetUser().UserId;
            return (await _pushBLL.Update(client)).ToAjaxResult();
        }
    }
}
