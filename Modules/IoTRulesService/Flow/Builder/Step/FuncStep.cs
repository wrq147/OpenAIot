using Castle.Core.Logging;
using IoTRulesService.Flow.Node;
using IoTService;
using IoTService.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 执行功能节点
    /// </summary>
    public class FuncStep : RuleflowStep
    {
        public RewriteProps props { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            var dataItem = context.Data.LastOrDefault();

            string productId = context.Source.ProductId;
            string deviceId = context.Source.DeviceId;
            if (props.TargetType == 1)
            {
                var device = await context.GetDevice(props.TargetId);
                if (device == null)
                {
                    await context.Print("设备不存在");
                    await context.ExcuteNext(RuleResult.Next());
                    return;
                }
                productId = device.ProductId;
                deviceId = device.DeviceId;
            }
            else if (props.TargetType == 2)
            {
                productId = props.TargetId;
            }
            var tsl = await context.GetTsl(productId);
            if (tsl == null)
            {
                await context.Print("物模型不存在");
                return;
            }
            var funcItem = tsl.Model.functions.Where(x => x.code == props.FunctionId).FirstOrDefault();
            if (funcItem == null)
            {
                await context.Print("功能不存在");
                return;
            }
            IDictionary<string, object> inputs = funcItem.CreateInputs();
            if (props.EventInput)
            {
                if (dataItem != null)
                {
                    foreach (var kvp in dataItem.Data)
                    {
                        if (inputs.ContainsKey(kvp.Key))
                        {
                            inputs[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            inputs.Add(kvp);
                        }
                    }
                }
            }
            else
            {
                if (props.InputData != null)
                {
                    foreach (var kvp in props.InputData)
                    {
                        if (inputs.ContainsKey(kvp.Key))
                        {
                            inputs[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            inputs.Add(kvp);
                        }
                    }
                }

            }


            if (context.IsDebug)
            {
                await context.Print("开始执行功能：" + funcItem.name);
            }

            if (props.TargetType == 2)
            {
                List<string> exeDevices = new List<string>();
                var deviceDal = context.Provider.GetService<IotDeviceDAL>();
                var dtuIds = await deviceDal.SelectDtuIdListByOnline(productId);
                foreach(var dtuId in dtuIds)
                {
                    await context.Provider.GetService<ServerBusProxy>().DownFunction(productId, dtuId, tsl.NetworkWay, props.FunctionId, inputs);
                }
            }
            else
            {
                var rs = await context.Provider.GetService<ServerBusProxy>().DownFunction(productId, deviceId, tsl.NetworkWay, props.FunctionId, inputs);
                if (!rs.IsSuccess())
                {
                    await context.Print("功能执行失败：" + rs.Message);
                    return;
                }
                if (props.ReturnOutput)
                {
                    if (rs.Data != null)
                    {
                        context.Data.Add(StreamData.Create(rs.Data));
                    }
                }
                if (context.IsDebug)
                {
                    await context.Print($"功能执行成功");
                }
            }

            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
