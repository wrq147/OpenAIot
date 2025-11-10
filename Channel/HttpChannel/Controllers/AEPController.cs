using ChannelUtility;
using ChannelUtility.Message;
using HttpChannel.AEP;
using HttpChannel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
// using TemplateAction.Route;


namespace HttpChannel.Services
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AEPController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ClientBusProxy _eventBus;

        public AEPController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }
        [HttpGet]
        public string Test()
        {
            return "return OK();";
        }
        [HttpPost]
        public async Task<string> StartAction()
        {
            _eventBus = _serviceProvider.GetService<ClientBusProxy>();

            var httpOption = _serviceProvider.GetService<IOptions<HttpChannelOption>>();

            #region 获取上下文
            string result;
            var httpSream = Request.Body;

            var content = Request.HttpContext.Request.Body.ToString();
            var content1 = Request.BodyReader;

            var stream = content1.AsStream();

            using (var read = new StreamReader(stream))
            {
                var jsonStr = read.ReadToEnd();

                AepInput aepInput = new AepInput();

                aepInput = JsonConvert.DeserializeObject<AepInput>(jsonStr);

                if (aepInput != null)
                {
                    var base64Str = aepInput.EventContent.Data;
                    var byteArray = Convert.FromBase64String(base64Str);

                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (var item in byteArray)
                    {
                        stringBuilder.Append(item.ToString("X2"));
                        stringBuilder.Append(" ");
                    }
                }

                #region 获取产品信息
                string productStr = Aep_product_management.QueryProduct(httpOption.Value.appKey, httpOption.Value.appSecrete, aepInput.ProductId);
                var productResult = JsonConvert.DeserializeObject<ProductResult>(productStr);

                #endregion

                #region 下发命令给设备

                string appKey = httpOption.Value.appKey;
                string apiSecrete = httpOption.Value.appSecrete;
                //string appKey = "d2mbc7AaTpf";
                //string apiSecrete = "Ii0jNQt4vG";
                //string masterKey = "ea15da1eb2244ff1b45d7f96fba77d5d";          //紧急按钮的masterKey
                //string masterKey = "d549ee7ae82e4325bab44bafd1ae73c9";          //门磁的masterKey

                string masterKey = productResult.Result.ApiKey;          //门磁的masterKey
                string body = "";

                char[] hexStr = aepInput.IMEI.ToCharArray();

                StringBuilder strb = new StringBuilder();

                foreach (var hexItem in hexStr)
                {
                    int value = Convert.ToInt32(hexItem);

                    strb.Append($"{value:X}");
                }

                var imeiHex = strb.ToString();

                AEPCommandInput replayInput = new AEPCommandInput()
                {
                    deviceGroupId = 100,
                    level = 1,
                    ttl = 7200,
                    deviceId = aepInput.DeviceId,
                    //Operator = "15959955802",
                    Operator = productResult.Result.CreateBy,
                    productId = long.Parse(aepInput.ProductId),
                    content = new AEPConten()
                    {
                        serviceIdentifier = "MessageAck",       //8999 Use Service Flag
                        Params = new Params()
                        {
                            data = $"05F58300{imeiHex}"
                        }
                    }
                };

                var objectType = replayInput.GetType().GetProperties().Where(c => c.Name == "Operator").FirstOrDefault();

                body = JsonConvert.SerializeObject(replayInput);

                body = body.Replace("Operator", "operator");
                body = body.Replace("Params", "params");

                try
                {
                    result = Aep_device_command.CreateCommand(appKey, apiSecrete, masterKey, body);

                    // 发布
                    var tsl = await _eventBus.GetTsl(null, aepInput.IMEI);
                    DeviceEventMessage evt = new DeviceEventMessage();
                    evt.DeviceId = aepInput.IMEI;
                    evt.ProductId = tsl!=null ? tsl.ProductId:"";
                    evt.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                    evt.EventId = "Alarm";
                    await _eventBus.ConfirmReplyAsync(string.Empty, evt);
                    await _eventBus.Connected(aepInput.IMEI);

                }
                catch (Exception ex)
                {
                    var e = ex.Message;
                    throw;
                }

                #endregion
            }

            #endregion



            return result;
        }
    }

}