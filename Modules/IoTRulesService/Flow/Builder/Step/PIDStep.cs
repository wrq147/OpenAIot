using Common;
using IoTRulesService.Flow.Node;
using IoTService;
using MyAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// PID算法节点
    /// </summary>
    public class PIDStep : RuleflowStep
    {
        public PIDProps props { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            try
            {
                var refdev = await context.GetDevice(props.refdevid);
                if (refdev == null)
                {
                    await context.Print("参考设备不存在");
                    return;
                }
                var devprops = await context.DeviceProp(refdev.DeviceId);
                double refval;
                if (!devprops.TryGetValue(props.refprop, out var tmpva))
                {
                    await context.Print("参考属性不存在");
                    return;
                }
                refval = Convert.ToDouble(tmpva);
                double targetval = Convert.ToDouble(context.GetParam(props.targetval));
                double errdelta = targetval - refval;

                double pdata = context.GetParamByDefault<double>(props.pname, 0);
                double idata = context.GetParamByDefault<double>(props.iname, 0);
                double ddata = context.GetParamByDefault<double>(props.dname, 0);


                //位置式 PID
                if (props.pidtype == 0)
                {
                    string terrtotalkey = string.Empty;
                    string terrlastkey = string.Empty;
                    string tlasttimekey = string.Empty;
                    if (context.Source.DeviceId != null)
                    {
                        terrtotalkey = $"$errtotal-{context.RuleId}-{context.Source.DeviceId}";
                        terrlastkey = $"$errlast-{context.RuleId}-{context.Source.DeviceId}";
                        tlasttimekey = $"$timelast-{context.RuleId}-{context.Source.DeviceId}";
                    }
                    else
                    {
                        terrtotalkey = $"$errtotal-{context.RuleId}";
                        terrlastkey = $"$errlast-{context.RuleId}";
                        tlasttimekey = $"$timelast-{context.RuleId}";
                    }
                    var errtt = context.GetParamByDefault<double>(terrtotalkey, 0);
                    long nowtime = TypeConvert.Time2Unix(DateTime.Now);
                    var timelast = context.GetParamByDefault<long>(tlasttimekey, nowtime);
                    double timedelta = (double)((double)(nowtime - timelast) / 1000);
                    errtt += errdelta * timedelta;

                    var lasterr = context.GetParamByDefault(terrlastkey, errdelta);
                    var errrate = (errdelta - lasterr) / timedelta;

                    double result = pdata * (errdelta + errtt / idata + ddata * errrate);

                    if (result > 0)
                    {
                        if (context.IsDebug)
                        {
                            await context.Print($"开始正向调控输出{result}");
                        }
                        //正向调控
                        double yue = result;
                        foreach (var pp in props.items)
                        {
                            var device = await context.GetDevice(pp.id);
                            if (device == null)
                            {
                                continue;
                            }

                            var tsl = await context.GetTsl(device.ProductId);
                            if (tsl == null)
                            {
                                continue;
                            }

                            var funcItem = tsl.Model.functions.Where(x => x.code == pp.code).FirstOrDefault();
                            if (funcItem == null)
                            {
                                continue;
                            }
                            double tmpmax = context.GetParamByDefault<double>(pp.maxval, 0);
                            IDictionary<string, object> inputs = funcItem.CreateInputs();
                            double iptval = 0;
                            if (yue > 0)
                            {
                                if (yue > tmpmax)
                                {
                                    iptval = tmpmax;
                                }
                                else
                                {
                                    iptval = yue;
                                }
                            }

                            if (inputs.ContainsKey(pp.codeval))
                            {
                                inputs[pp.codeval] = iptval;
                            }
                            else
                            {
                                inputs.Add(pp.codeval, iptval);
                            }
                            var rs = await context.Provider.GetService<ServerBusProxy>().DownFunction(device.ProductId, device.DeviceId, tsl.NetworkWay, funcItem.code, inputs);
                            if (!rs.IsSuccess())
                            {
                                await context.Print("功能执行失败：" + rs.Message);
                                continue;
                            }
                            yue = yue - iptval;
                        }
                    }
                    else if (result < 0)
                    {
                        if (context.IsDebug)
                        {
                            await context.Print($"开始反向调控输出{result}");
                        }
                        //反向调控
                        double yue = result;
                        foreach (var pp in props.items)
                        {
                            var device = await context.GetDevice(pp.id);
                            if (device == null)
                            {
                                continue;
                            }

                            var tsl = await context.GetTsl(device.ProductId);
                            if (tsl == null)
                            {
                                continue;
                            }

                            var funcItem = tsl.Model.functions.Where(x => x.code == pp.code).FirstOrDefault();
                            if (funcItem == null)
                            {
                                continue;
                            }
                            IDictionary<string, object> inputs = funcItem.CreateInputs();
                            double tmpmin = context.GetParamByDefault<double>(pp.minval, 0);

                            double iptval = 0;
                            if (yue < 0)
                            {
                                if (yue < tmpmin)
                                {
                                    iptval = tmpmin;
                                }
                                else
                                {
                                    iptval = yue;
                                }
                            }

                            if (inputs.ContainsKey(pp.codeval))
                            {
                                inputs[pp.codeval] = iptval;
                            }
                            else
                            {
                                inputs.Add(pp.codeval, iptval);
                            }
                            var rs = await context.Provider.GetService<ServerBusProxy>().DownFunction(device.ProductId, device.DeviceId, tsl.NetworkWay, funcItem.code, inputs);
                            if (!rs.IsSuccess())
                            {
                                await context.Print("功能执行失败：" + rs.Message);
                                continue;
                            }
                            yue = yue - iptval;

                        }
                    }


                    context.SetParam(terrtotalkey, errtt);
                    context.SetParam(terrlastkey, errdelta);
                    context.SetParam(tlasttimekey, nowtime);


                    this.IsActive = true;
                    await context.ExcuteNext(RuleResult.Next());
                }
                else if (props.pidtype == 1)
                {

                }
                else
                {
                    await context.Print($"节点{context.Step.Name}pid类型错误");
                }
            }
            catch (Exception ex)
            {
                await context.Print($"节点{context.Step.Name}异常{ex.Message}");
            }

        }
    }
}
