using AuthService.Controller;
using Common;
using MonitorService.Business;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace MonitorService.Controller
{
    /// <summary>
    /// 本机信息
    /// </summary>
    [About("/MonitorService/Server")]
    public class Server : AbstractLoginedController
    {
        private ServerBLL _server;
        public Server(ServerBLL server)
        {
            _server = server;
        }
        [HttpGet]
        public async Task<AjaxResult> Info()
        {
            var info = await _server.GetServerInfo();
            return this.Success(info);
        }
    }
}
