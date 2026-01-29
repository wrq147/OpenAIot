using Common.EventBus;
using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.TimerUtil
{
    public static class RulePluginExtends
    {
        public static void RegisterTime(this PluginObject plg, Func<TimeEvent, Task> ac)
        {
            var generalOption = plg.Collection.GetService<IOptions<GeneralOption>>();
            if (string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
            {
                plg.Dispatcher.Register<TimeEvent>(TimeEvent.EventKey, ac);
            }
            else
            {
                Task.Run(async () =>
                {
                    var bus = plg.Collection.GetService<NatsScope>().Bus;
                    await foreach (var msg in bus.SubscribeAsync(TimeEvent.EventKey, "Bussin" + plg.Name, DefalutNatsJsonSerializer<TimeEvent>.Default))
                    {
                        try
                        {
                            if (msg.Data == null)
                            {
                                continue;
                            }
                            await ac.Invoke(msg.Data);
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.Message);
                        }

                    }
                });

            }
        }
    }
}
