using ChannelUtility.Tsl;
using IoTRulesService.Flow.Node;
using IoTService.Business;
using IoTService.Models;
using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 修改标签
    /// </summary>
    public class TagStep : RuleflowStep
    {
        public TagProps props { get; set; }
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
            MZ_IotDevice device = null;
            if (props.TargetType == 0)
            {
                device = await context.GetDeviceByDtuId(context.Source.DeviceId);

            }
            else if (props.TargetType == 1)
            {
                device = await context.GetDevice(props.TargetId);
            }
            else
            {
                await context.Print("目标类型错误");
                return;
            }
            if (device == null)
            {
                await context.Print("目标设备不存在");
                return;
            }


            var model = await context.GetTsl(device.ProductId);
            if (model == null || model.Model.tags == null)
            {
                await context.Print("物模型或标签不存在");
                return;
            }
            BaseTagInfo tagInfo = model.Model.tags.Where(x => x.code == props.TagId).FirstOrDefault();
            if (tagInfo != null)
            {
                var tagdict = await context.DeviceTag(device.Id);
                object rawTagValue;
                tagdict.TryGetValue(props.TagId, out rawTagValue);
                switch (tagInfo.option.type)
                {
                    case "int":
                        {
                            var res = new Engine()
        .SetValue("param", new Func<string, object>(getParamObj))
        .SetValue("prop", new Func<string, string, object>(getDeviceProp))
        .SetValue("tag", new Func<string, string, object>(getDeviceTag))
        .SetValue("data", rawTagValue)
        .Evaluate(props.Express);
                            long tmpint = TAConverter.Cast<long>(res.ToObject());
                            if (context.IsDebug)
                            {
                                await context.Print($"标签{tagInfo.name}写入值:" + tmpint);
                            }

                            await context.UpdateTag(device, tagInfo, tmpint);
                        }
                        break;
                    case "float":
                        {
                            var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", rawTagValue)
.Evaluate(props.Express);
                            double tmpdb = TAConverter.Cast<double>(res.ToObject());
                            if (context.IsDebug)
                            {
                                await context.Print($"标签{tagInfo.name}写入值:" + tmpdb);
                            }
                            await context.UpdateTag(device, tagInfo, tmpdb);
                        }
                        break;
                    case "boolean":
                        {
                            var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", rawTagValue)
.Evaluate(props.Express);
                            bool tmpbool = TAConverter.Cast<bool>(res.ToObject());
                            if (context.IsDebug)
                            {
                                await context.Print($"标签{tagInfo.name}写入值:" + tmpbool);
                            }
                            await context.UpdateTag(device, tagInfo, tmpbool);
                        }
                        break;
                    default:
                        {
                            var res = new Engine()
.SetValue("param", new Func<string, object>(getParamObj))
.SetValue("prop", new Func<string, string, object>(getDeviceProp))
.SetValue("tag", new Func<string, string, object>(getDeviceTag))
.SetValue("data", rawTagValue)
.Evaluate(props.Express);
                            string tmpstr = TAConverter.Cast<string>(res.ToObject());
                            if (context.IsDebug)
                            {
                                await context.Print($"标签{tagInfo.name}写入值:" + tmpstr);
                            }
                            await context.UpdateTag(device, tagInfo, tmpstr);
                        }
                        break;
                }
            }
            else
            {
                if (context.IsDebug)
                    await context.Print($"标签{props.TagId}未定义");
            }
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
