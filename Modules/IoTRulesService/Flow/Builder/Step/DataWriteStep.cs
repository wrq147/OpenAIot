
using IoTRulesService.Flow.Node;
using IoTService.Models;
using Jint;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Common;


namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 修改参数
    /// </summary>
    public class DataWriteStep : RuleflowStep
    {
        private delegate T ParamProcess<T>(T input);
        public WriteProps props { get; set; }
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
        private async Task<object> TotalCountProp(string id, string calway)
        {
            var countStep = _context.FindStep(id);
            CountStep ccstep = countStep as CountStep;
            List<object> tobjList = ccstep.GetData().Select(x => x.Data[ccstep.FieldName]).ToList();
            var tsl = await _context.GetTsl(_context.Source.ProductId);
            var bs = tsl.Model.properties.Where(x => x.code == ccstep.FieldName).FirstOrDefault();
            if (bs != null)
            {
                switch (bs.option.type)
                {
                    case "int":
                        {
                            int totalVal = 0;
                            foreach (var val in tobjList)
                            {
                                totalVal += Convert.ToInt32(val);
                            }
                            switch (calway)
                            {
                                case "sum":
                                    return totalVal;
                                case "average":
                                    return totalVal / tobjList.Count;
                                default:
                                    return null;
                            }
                        }

                    case "float":
                        {
                            double totalVal = 0;
                            foreach (var val in tobjList)
                            {
                                totalVal += Convert.ToDouble(val);
                            }
                            switch (calway)
                            {
                                case "sum":
                                    return totalVal;
                                case "average":
                                    return totalVal / tobjList.Count;
                                default:
                                    return null;
                            }
                        }

                }
            }
            return null;
        }
        public override async Task Run(RuleExecutionContext context)
        {
            _context = context;
            try
            {
                //聚合数据统计
                if (props.Counts != null)
                {
                    foreach (var count in props.Counts)
                    {
                        var rs = await TotalCountProp(count.nodeId, count.calway);
                        if (context.IsDebug)
                        {
                            await context.Print($"参数{count.code}写入值:" + rs);
                        }
                        context.SetParam(count.code, rs);
                    }
                }
                //直接参数写入
                foreach (var exp in props.Writes)
                {
                    if (string.IsNullOrEmpty(exp.express))
                    {
                        await context.Print($"参数{exp.code}表达式为空");
                        continue;
                    }
                    string tmptt;
                    if (context.ParamTypeDict.TryGetValue(exp.code, out tmptt))
                    {
                        switch (tmptt)
                        {
                            case "int":
                                {
                                    var res = new Engine()
                .SetValue("param", new Func<string, object>(getParamObj))
                .SetValue("prop", new Func<string, string, object>(getDeviceProp))
                .SetValue("tag", new Func<string, string, object>(getDeviceTag))
                .SetValue("data", context.GetParam(exp.code))
                .Evaluate(exp.express);
                                    long tmpint = TAConverter.Cast<long>(res.ToObject());
                                    if (context.IsDebug)
                                    {
                                        await context.Print($"参数{exp.code}写入值:" + tmpint);
                                    }
                                    context.SetParam(exp.code, tmpint);
                                }
                                break;
                            case "float":
                                {
                                    var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", context.GetParam(exp.code))
.Evaluate(exp.express);
                                    double tmpdb = TAConverter.Cast<double>(res.ToObject());
                                    if (context.IsDebug)
                                    {
                                        await context.Print($"参数{exp.code}写入值:" + tmpdb);
                                    }
                                    context.SetParam(exp.code, tmpdb);

                                }
                                break;
                            case "boolean":
                                {
                                    var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", context.GetParam(exp.code))
.Evaluate(exp.express);
                                    bool tmpbool = TAConverter.Cast<bool>(res.ToObject());
                                    if (context.IsDebug)
                                    {
                                        await context.Print($"参数{exp.code}写入值:" + tmpbool);
                                    }
                                    context.SetParam(exp.code, tmpbool);
                                }
                                break;
                            default:
                                {
                                    var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", context.GetParam(exp.code))
.Evaluate(exp.express);
                                    string tmpstr = TAConverter.Cast<string>(res.ToObject());
                                    if (context.IsDebug)
                                    {
                                        await context.Print($"参数{exp.code}写入值:" + tmpstr);
                                    }
                                    context.SetParam(exp.code, tmpstr);
                                }
                                break;
                        }

                    }
                    else
                    {
                        if (context.IsDebug)
                        {
                            await context.Print($"参数{exp.code}未定义");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                await context.Print("表达式赋值异常" + ex.Message);
            }
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
