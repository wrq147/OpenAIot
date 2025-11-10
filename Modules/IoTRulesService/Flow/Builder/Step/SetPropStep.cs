using ChannelUtility.Tsl;
using IoTRulesService.Flow.Node;
using IoTService.Models;
using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 修改属性
    /// </summary>
    public class SetPropStep : RuleflowStep
    {
        public SetPropProps props { get; set; }
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
            var model = await context.GetTsl(context.Source.ProductId);
            if (model == null || model.Model.properties == null)
            {
                await context.Print("物模型或属性不存在");
                return;
            }

            BaseProperty propInfo = model.Model.properties.Where(x => x.code == props.PropCode).FirstOrDefault();
            if (propInfo != null)
            {
                var propdict = await context.DeviceProp(context.Source.DeviceId);
                object rawPropValue;
                propdict.TryGetValue(props.PropCode, out rawPropValue);
                switch (propInfo.option.type)
                {
                    case "int":
                        {
                            var res = new Engine()
        .SetValue("param", new Func<string, object>(getParamObj))
        .SetValue("prop", new Func<string, string, object>(getDeviceProp))
        .SetValue("tag", new Func<string, string, object>(getDeviceTag))
        .SetValue("data", rawPropValue)
        .Evaluate(props.Express);
                            long tmpint = TAConverter.Cast<long>(res.ToObject());
                            if (context.IsDebug)
                            {
                                await context.Print($"属性{propInfo.name}写入值:" + tmpint);
                            }
                            await context.SetProp(context.Source.DeviceId, props.PropCode, tmpint);
                        }
                        break;
                    case "float":
                        {
                            var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", rawPropValue)
.Evaluate(props.Express);
                            double tmpdb = TAConverter.Cast<double>(res.ToObject());
                            if (context.IsDebug)
                            {
                                await context.Print($"属性{propInfo.name}写入值:" + tmpdb);
                            }
                            await context.SetProp(context.Source.DeviceId, props.PropCode, tmpdb);
                        }
                        break;
                    case "boolean":
                        {
                            var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", rawPropValue)
.Evaluate(props.Express);
                            bool tmpbool = TAConverter.Cast<bool>(res.ToObject());
                            if (context.IsDebug)
                            {
                                await context.Print($"属性{propInfo.name}写入值:" + tmpbool);
                            }
                            await context.SetProp(context.Source.DeviceId, props.PropCode, tmpbool);
                        }
                        break;
                    default:
                        {
                            var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", rawPropValue)
.Evaluate(props.Express);
                            string tmpstr = TAConverter.Cast<string>(res.ToObject());
                            if (context.IsDebug)
                            {
                                await context.Print($"属性{propInfo.name}写入值:" + tmpstr);
                            }
                            await context.SetProp(context.Source.DeviceId, props.PropCode, tmpstr);
                        }
                        break;
                }
            }
            else
            {
                if (context.IsDebug)
                    await context.Print($"属性{props.PropCode}未定义");
            }
            await context.ExcuteNext(RuleResult.Next());

        }
    }
}
