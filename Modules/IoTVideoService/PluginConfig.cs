using ChannelUtility.Message;
using Common.EventBus;
using EasyNetQ;
using IoTRulesService.DataParser;
using IoTService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.NetCore;
namespace IoTVideoService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "IoTService", "IoTRulesService" };


        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {

        }

        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            TAAsyncHelper.RunSync(async () =>
            {
                var bus = app.ServiceProvider.GetService<RabbitScope>().Bus;
                await app.ServiceProvider.GetService<RabbitScope>().Bus.PubSub.SubscribeAsync<string>("VideoNodeInitId", (nodename) =>
                {
                    string[] sarr = nodename.Split("#", StringSplitOptions.RemoveEmptyEntries);
                    if (sarr.Length > 1)
                    {
                        if (sarr[0] == "0")
                        {
                            //获取gb28181

                        }
                        else if (sarr[0] == "1")
                        {
                            //固定地址
                        }
                        app.ServiceProvider.GetService<RabbitScope>().Bus.PubSub.PublishAsync();
                    }

                }, cfg =>
                {
                    cfg.WithTopic("/VideoNode.Init");
                    cfg.WithAutoDelete(true);
                });


            });

        }

    }
}
