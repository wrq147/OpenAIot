using IoTRulesService.Flow.Node;
using IoTService;
using IoTService.Models;
using Jint;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 设备调度节点
    /// </summary>
    public class TimeSchedulerStep : RuleflowStep
    {
        public TimeSchedulerProps props { get; set; }
        private RuleExecutionContext _context;
        private object getParamObj(string key)
        {
            return _context.GetParam(key);
        }
        private object getDeviceTag(string id, string key)
        {
            var rs = TAAsyncHelper.RunSync<object>(async () =>
            {
                IDictionary<string, object> tagDict = await _context.DeviceTag(id).ConfigureAwait(false);
                object tmpval;
                if (tagDict.TryGetValue(key, out tmpval))
                {
                    return tmpval;
                }
                return null;
            });
            return rs;
        }
        private object getDeviceProp(string id, string key)
        {
            var rs = TAAsyncHelper.RunSync<object>(async () =>
            {
                MZ_IotDevice dev = await _context.GetDevice(id).ConfigureAwait(false);
                if (dev == null)
                {
                    return null;
                }
                IDictionary<string, object> propDict = await _context.DeviceProp(dev.DeviceId).ConfigureAwait(false);
                object tmpval;
                if (propDict.TryGetValue(key, out tmpval))
                {
                    return tmpval;
                }
                return null;
            });
            return rs;
        }
        public override async Task Run(RuleExecutionContext context)
        {
            _context = context;
            this.IsActive = false;
            int ac = 0;
            if (props.DeviceList != null && props.DeviceList.Length > 0)
            {
                //生成优先级序列
                int[] intList = new int[props.DeviceList.Length];
                for (int i = 0; i < props.DeviceList.Length; i++)
                {
                    intList[i] = i + 1;
                    var ditem = props.DeviceList[i];
                    var itemset = props.DeviceSets.Where(x => x.id == ditem).FirstOrDefault();
                    if (itemset == null)
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(itemset.level))
                    {
                        intList[i] = context.GetParamByDefault<int>(itemset.level, 0);
                    }
                }
                var combined = props.DeviceList.Zip(intList, (str, num) => new { StringValue = str, IntValue = num });
                var sortedCombined = combined.OrderBy(item => item.IntValue);
                var sortedStringList = sortedCombined.Select(item => item.StringValue).ToList();
                foreach (string did in sortedStringList)
                {
                    if (ac >= props.ScheAmount)
                    {
                        break;
                    }
                    var itemset = props.DeviceSets.Where(x => x.id == did).FirstOrDefault();
                    if (itemset == null)
                    {
                        continue;
                    }
                    var device = await context.GetDevice(itemset.id);
                    if (device == null)
                    {
                        continue;
                    }
                    bool canExe = true;
                    int condidxi = 0;
                    if (itemset.groups == null)
                    {
                        itemset.groups = Array.Empty<string>();
                    }
                    foreach (var condi in itemset.conditions)
                    {
                        object compare1;
                        if (condi.enablecode.StartsWith("$"))
                        {
                            compare1 = context.GetParam(condi.enablecode.Substring(1));
                        }
                        else if (condi.enablecode.StartsWith("#"))
                        {
                            var tagdict = await context.DeviceTag(device.Id);
                            if (!tagdict.TryGetValue(condi.enablecode.Substring(1), out compare1))
                            {
                                canExe = false;
                                break;
                            }
                        }
                        else
                        {
                            var prodict = await context.DeviceProp(device.DeviceId);
                            if (!prodict.TryGetValue(condi.enablecode, out compare1))
                            {
                                canExe = false;
                                break;
                            }
                        }
                        bool curcondrs;
                        if (condi.valtype == "Double")
                        {
                            double cm1 = Convert.ToDouble(compare1);
                            double cm2;
                            if (condi.valuefrom == 1)
                            {
                                cm2 = Convert.ToDouble(context.GetParam(condi.val));
                            }
                            else
                            {
                                cm2 = Convert.ToDouble(condi.val);
                            }

                            switch (condi.compare)
                            {
                                case "=":
                                    curcondrs = cm1 == cm2;
                                    break;
                                case "!=":
                                    curcondrs = cm1 != cm2;
                                    break;
                                case ">":
                                    curcondrs = cm1 > cm2;
                                    break;
                                case "<":
                                    curcondrs = cm1 < cm2;
                                    break;
                                case ">=":
                                    curcondrs = cm1 >= cm2;
                                    break;
                                case "<=":
                                    curcondrs = cm1 <= cm2;
                                    break;
                                default:
                                    curcondrs = false;
                                    break;
                            }
                        }
                        else if (condi.valtype == "Long" || condi.valtype == "Date")
                        {
                            long cm1 = Convert.ToInt64(compare1);
                            long cm2;
                            if (condi.valuefrom == 1)
                            {
                                cm2 = Convert.ToInt64(context.GetParam(condi.val));
                            }
                            else
                            {
                                cm2 = Convert.ToInt64(condi.val);
                            }

                            switch (condi.compare)
                            {
                                case "=":
                                    curcondrs = cm1 == cm2;
                                    break;
                                case "!=":
                                    curcondrs = cm1 != cm2;
                                    break;
                                case ">":
                                    curcondrs = cm1 > cm2;
                                    break;
                                case "<":
                                    curcondrs = cm1 < cm2;
                                    break;
                                case ">=":
                                    curcondrs = cm1 >= cm2;
                                    break;
                                case "<=":
                                    curcondrs = cm1 <= cm2;
                                    break;
                                default:
                                    curcondrs = false;
                                    break;
                            }
                        }
                        else
                        {
                            if (condi.compare == "=")
                            {
                                string compareval = condi.valuefrom == 1 ? TAConverter.Cast<string>(context.GetParam(condi.val)) : condi.val;
                                curcondrs = TAConverter.Cast<string>(compare1) == compareval;
                            }
                            else if (condi.compare == "!=")
                            {
                                string compareval = condi.valuefrom == 1 ? TAConverter.Cast<string>(context.GetParam(condi.val)) : condi.val;
                                curcondrs = TAConverter.Cast<string>(compare1) != compareval;
                            }
                            else
                            {
                                curcondrs = false;
                            }
                        }

                        if (condidxi == 0)
                        {
                            canExe = curcondrs;
                        }
                        else
                        {
                            var ccccidd = condidxi - 1;
                            if (itemset.groups.Length > ccccidd)
                            {
                                if (itemset.groups[ccccidd] == "&")
                                {
                                    canExe = canExe && curcondrs;
                                }
                                else
                                {
                                    canExe = canExe || curcondrs;
                                }
                            }
                            else
                            {
                                canExe = canExe && curcondrs;
                            }
                        }

                        ++condidxi;
                    }

                    if (canExe)
                    {
                        if (context.IsDebug)
                        {
                            await context.Print($"{device.Name}[{device.DeviceId}]满足条件");
                        }

                        bool isExe = true;
                        foreach (var actionItem in itemset.actions)
                        {
                            MZ_IotDevice actionDev = device;
                            if (actionItem.targettype == 1)
                            {
                                actionDev = await context.GetDevice(actionItem.targetid);
                            }
                            if (actionItem.codetype == 0)
                            {
                                //执行功能
                                var tsl = await context.GetTsl(actionDev.ProductId);
                                if (tsl == null)
                                {
                                    await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]的物模型不存在");
                                    continue;
                                }
                                var funcItem = tsl.Model.functions.Where(x => x.code == actionItem.code).FirstOrDefault();
                                if (funcItem == null)
                                {
                                    await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]执行的功能不存在");
                                    continue;
                                }
                                IDictionary<string, object> inputs = funcItem.CreateInputs();
                                if (!string.IsNullOrEmpty(actionItem.express))
                                {
                                    try
                                    {
                                        var expdict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(actionItem.express);
                                        foreach (var kvp in expdict)
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
                                    catch (Exception ex)
                                    {
                                        if (context.IsDebug)
                                        {
                                            await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]的功能{funcItem.name}解释异常：" + ex.Message);
                                        }

                                    }
                                }
                                var rs = await context.Provider.GetService<ServerBusProxy>().DownFunction(actionDev.ProductId, actionDev.DeviceId, tsl.NetworkWay, actionItem.code, inputs);
                                if (!rs.IsSuccess())
                                {
                                    if (context.IsDebug)
                                        await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]的功能{funcItem.name}执行失败：" + rs.Message);

                                    if (actionItem.errbreak)
                                    {
                                        isExe = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (context.IsDebug)
                                        await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]的功能{funcItem.name}执行成功");
                                }
                            }
                            else if (actionItem.codetype == 1)
                            {
                                //执行参数赋值
                                try
                                {
                                    string tmptt;
                                    if (context.ParamTypeDict.TryGetValue(actionItem.code, out tmptt))
                                    {
                                        switch (tmptt)
                                        {
                                            case "int":
                                                {
                                                    var res = new Engine()
                                                    .SetValue("param", new Func<string, object>(getParamObj))
                                                    .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                                                    .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                                                    .SetValue("data", context.GetParam(actionItem.code))
                                                    .Evaluate(actionItem.express);
                                                    context.SetParam(actionItem.code, TAConverter.Cast<long>(res.ToObject()));
                                                }
                                                break;
                                            case "float":
                                                {
                                                    var res = new Engine()
                                                  .SetValue("param", new Func<string, object>(getParamObj))
                                                  .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                                                  .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                                                  .SetValue("data", context.GetParam(actionItem.code))
                                                  .Evaluate(actionItem.express);
                                                    context.SetParam(actionItem.code, TAConverter.Cast<double>(res.ToObject()));
                                                }
                                                break;
                                            case "boolean":
                                                {
                                                    var res = new Engine()
                                                  .SetValue("param", new Func<string, object>(getParamObj))
                                                  .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                                                  .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                                                  .SetValue("data", context.GetParam(actionItem.code))
                                                  .Evaluate(actionItem.express);
                                                    context.SetParam(actionItem.code, TAConverter.Cast<bool>(res.ToObject()));
                                                }
                                                break;
                                            default:
                                                {
                                                    var res = new Engine()
                                                    .SetValue("param", new Func<string, object>(getParamObj))
                                                    .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                                                    .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                                                    .SetValue("data", context.GetParam(actionItem.code))
                                                    .Evaluate(actionItem.express);
                                                    context.SetParam(actionItem.code, TAConverter.Cast<string>(res.ToObject()));
                                                }
                                                break;
                                        }

                                        if (context.IsDebug)
                                            await context.Print($"参数{actionItem.code}赋值成功");
                                    }
                                    else
                                    {
                                        if (context.IsDebug)
                                            await context.Print($"参数{actionItem.code}不存在");
                                    }
                                }
                                catch
                                {
                                    if (context.IsDebug)
                                        await context.Print($"参数{actionItem.code}赋值失败");
                                }
                            }
                            else if (actionItem.codetype == 2)
                            {
                                //执行标签赋值
                                try
                                {
                                    var tsl = await context.GetTsl(actionDev.ProductId);
                                    if (tsl == null)
                                    {
                                        await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]的物模型不存在");
                                        continue;
                                    }
                                    var tagItem = tsl.Model.tags.Where(x => x.code == actionItem.code).FirstOrDefault();
                                    if (tagItem == null)
                                    {
                                        await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]的标签不存在");
                                        continue;
                                    }
                                    var tagdict = await context.DeviceTag(actionDev.Id);
                                    object rawTagValue;
                                    tagdict.TryGetValue(actionItem.code, out rawTagValue);
                                    switch (tagItem.option.type)
                                    {
                                        case "int":
                                            {
                                                var res = new Engine()
                                                .SetValue("param", new Func<string, object>(getParamObj))
                                                .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                                                .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                                                .SetValue("data", rawTagValue)
                                                .Evaluate(actionItem.express);

                                                await context.UpdateTag(actionDev, tagItem, TAConverter.Cast<long>(res.ToObject()));
                                            }
                                            break;
                                        case "float":
                                            {
                                                var res = new Engine()
                                              .SetValue("param", new Func<string, object>(getParamObj))
                                              .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                                              .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                                              .SetValue("data", rawTagValue)
                                              .Evaluate(actionItem.express);

                                                await context.UpdateTag(actionDev, tagItem, TAConverter.Cast<double>(res.ToObject()));
                                            }
                                            break;
                                        case "boolean":
                                            {
                                                var res = new Engine()
                                              .SetValue("param", new Func<string, object>(getParamObj))
                                              .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                                              .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                                              .SetValue("data", rawTagValue)
                                              .Evaluate(actionItem.express);

                                                await context.UpdateTag(actionDev, tagItem, TAConverter.Cast<bool>(res.ToObject()));
                                            }
                                            break;
                                        default:
                                            {
                                                var res = new Engine()
                                                .SetValue("param", new Func<string, object>(getParamObj))
                                                .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                                                .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                                                .SetValue("data", rawTagValue)
                                                .Evaluate(actionItem.express);
                                                await context.UpdateTag(actionDev, tagItem, TAConverter.Cast<string>(res.ToObject()));
                                            }
                                            break;
                                    }

                                    if (context.IsDebug)
                                        await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]的标签{actionItem.code}赋值成功");
                                }
                                catch
                                {
                                    if (context.IsDebug)
                                        await context.Print($"{actionDev.Name}[{actionDev.DeviceId}]的标签{actionItem.code}赋值失败");
                                }
                            }
                        }

                        if (isExe)
                        {
                            this.IsActive = true;
                            ++ac;
                        }
                    }
                    else
                    {
                        if (context.IsDebug)
                        {
                            await context.Print($"{device.Name}[{device.DeviceId}]不满足条件");
                        }
                    }
                }
            }

            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
