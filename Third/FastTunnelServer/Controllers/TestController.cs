using FastTunnel.Core.Client;
using Microsoft.AspNetCore.Mvc;

namespace FastTunnelServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController: ControllerBase
    {
        [HttpGet]
        public ContentResult IsOk()
        {
            return Content("测试端口成功");
        }
    }
}
