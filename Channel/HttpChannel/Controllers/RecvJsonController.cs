using ChannelUtility;
using ChannelUtility.Buffers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Text.Encodings;
using System.Text;
namespace HttpChannel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecvJsonController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        public RecvJsonController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }
        [HttpGet]
        public async Task<string> Get(string field = "")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                string bodyText = await reader.ReadToEndAsync();
                string pattern = $"\"{Regex.Escape(field)}\":\\s*\"?([^\",\\]]+)\"?";
                Match match = Regex.Match(bodyText, pattern);

                if (match.Success && match.Groups.Count > 1)
                {
                    var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                    string dtuId = match.Groups[1].Value;
                    var ret = await eventBus.GetTsl(null, dtuId, true);
                    if (ret == null)
                    {
                        return "error:DtuId";
                    }
                    var readBuffer = new FastReader(Encoding.UTF8.GetBytes(bodyText));
                    if (await eventBus.PushCustom(ret.ProductId, dtuId, null, readBuffer, ret.script, ret.Model, null))
                    {
                        return "ok";
                    }
                }
            }
            return "error";
        }
        [HttpPost]
        public async Task<string> Post(string field = "")
        {
            using (var reader = new StreamReader(Request.Body))
            {
                string bodyText = await reader.ReadToEndAsync();
                string pattern = $"\"{Regex.Escape(field)}\":\\s*\"?([^\",\\]]+)\"?";
                Match match = Regex.Match(bodyText, pattern);

                if (match.Success && match.Groups.Count > 1)
                {
                    var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                    string dtuId = match.Groups[1].Value;
                    var ret = await eventBus.GetTsl(null, dtuId, true);
                    if (ret == null)
                    {
                        return "error:DtuId";
                    }
                    var readBuffer = new FastReader(Encoding.UTF8.GetBytes(bodyText));
                    if (await eventBus.PushCustom(ret.ProductId, dtuId, null, readBuffer, ret.script, ret.Model, null))
                    {
                        return "ok";
                    }
                }
            }
            return "error";
        }
    }
}
