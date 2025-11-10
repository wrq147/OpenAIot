using HttpChannel;
using ChannelUtility;
using HttpChannel.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configSec = builder.Configuration.GetSection("HttpChannelOption");
builder.Services.Configure<HttpChannelOption>(configSec);
builder.Services.AddHostedService<HttpService>();
builder.Services.AddEventBus(x =>
{
    var option = configSec.Get<HttpChannelOption>();
    x.EventConn = option.event_conn;
    x.RedisConn = option.redis_conn;
    x.config = new ChannelUtility.Config.ChannelConfig();
    x.config.Name = "Http服务接入";
    x.config.Code = "http_only";
    x.config.Remark = "设备通过Http协议接收和发送数据。";
    x.config.CanScript = true;
    x.config.CanModbus = false;
    x.config.CanBind = false;
    x.config.CanDebug = true;
    x.config.CanModify = false;
});

builder.Services.AddControllers();

var app = builder.Build();
var config = builder.Configuration; // 获取appsettings.json中的配置信息
var port = config["HttpSettings:Port"] ?? "8800";
// Configure the HTTP request pipeline.

//var eventBus = app.Services.GetService<ClientBusProxy>();
//eventBus.OnSubProductMessage += async (msg, ret) =>
//{

//};
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Urls.Add($"http://*:{port}");
app.Run();
