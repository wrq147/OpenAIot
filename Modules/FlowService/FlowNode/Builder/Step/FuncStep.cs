using ChannelUtility.Tsl;
using FlowService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 执行功能
    /// </summary>
    public class FuncStep : WorkflowStep
    {
        public FuncProps props { get; set; }
        public FuncStep()
        {
            this.PersistenceNode = false;
        }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            object tmpval;
            if (!context.FormItems.TryGetValue(props.FieldId, out tmpval))
            {
                throw new Exception($"表单项 {props.FieldId} 不存在");
            }
            var selectedlist = tmpval as IEnumerable<object>;
            if (selectedlist == null)
            {
                throw new Exception($"表单项 {props.FieldId} 不存在设备");
            }
            List<string> devids = new List<string>();
            foreach (var node in selectedlist)
            {
                var nodeobj = node as IDictionary<string, object>;
                string devId = Convert.ToString(nodeobj["id"]);
                devids.Add(devId);
            }
            var dinfos = await context.ServiceProvider.GetService<FlowDeviceDAL>().SelectFlowDeviceList(devids);
            foreach (var item in props.Items)
            {
                if (string.IsNullOrEmpty(item.ProductId))
                {
                    continue;
                }
                var taretDevices = dinfos.Where(x => x.ProductId == item.ProductId);
                if (taretDevices.Count() <= 0)
                {
                    continue;
                }
                var model = TslModel.CreateFrom(taretDevices.First().ModelTSL);
                if (model == null)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(item.FunctionId))
                {
                    continue;
                }
                if (item.IsFunc == 0)
                {
                    var funcItem = model.functions.Where(x => x.code == item.FunctionId).FirstOrDefault();
                    if (funcItem == null)
                    {
                        continue;
                    }
                    IDictionary<string, object> inputs = funcItem.CreateInputs();
                    if (item.InputData != null)
                    {
                        foreach (var kvp in item.InputData)
                        {
                            if (inputs.ContainsKey(kvp.Key))
                            {
                                inputs[kvp.Key] = kvp.Value;
                            }
                            else
                            {
                                inputs.Add(kvp.Key, kvp.Value);
                            }
                        }
                    }
                    foreach (var target in taretDevices)
                    {
                        var res = await context.ServiceProvider.GetService<DeviceBusProxy>().DownFunction(target, item.FunctionId, inputs);
                        if (!res.IsSuccess())
                        {
                            throw new Exception("执行功能失败：" + res.Message);
                        }
                    }
                }
                else
                {
                    var evtItem = model.events.Where(x => x.code == item.FunctionId).FirstOrDefault();
                    if (evtItem == null)
                    {
                        continue;
                    }
                    IDictionary<string, object> outputs = new Dictionary<string, object>();
                    if (item.InputData != null)
                    {
                        foreach (var kvp in item.InputData)
                        {
                            string tmpkv = kvp.Value as string;
                            if (!string.IsNullOrEmpty(tmpkv))
                            {
                                object tmpvv = context.GetFormObject(tmpkv);
                                if (tmpvv != null)
                                {
                                    if (!outputs.ContainsKey(kvp.Key))
                                    {
                                        outputs.Add(kvp.Key, tmpvv);
                                    }

                                }

                            }
                        }
                    }
                    foreach (var target in taretDevices)
                    {
                        await context.ServiceProvider.GetService<DeviceBusProxy>().SendEvent(target.ProductId, target.DeviceId, item.FunctionId, outputs);
                    }
                }

            }
            return await ExecutionResult.Next();
        }

    }

}
