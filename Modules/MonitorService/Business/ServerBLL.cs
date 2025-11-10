using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using MonitorService.Hardware;
using Common.EventBus;

namespace MonitorService.Business
{
    public class ServerBLL
    {
        private ITAServiceProvider _provider;
        public ServerBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public async Task<ServerInfo> GetServerInfo()
        {
            ServerInfo info = new ServerInfo();
            try
            {

                info.cpu.Refresh();
            }
            catch { }
            try
            {
                info.mem.Refresh();
            }
            catch { }
            try
            {
                info.clr.Refresh();
            }
            catch { }
            try
            {
                info.sys.Refresh(_provider.GetService<IWebHostEnvironment>());
            }
            catch { }
            return info;
        }

    }
}
