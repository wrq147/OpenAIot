using ChannelUtility;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using Common;
using Common.Json;
using IoTRulesService.Flow.Builder.Step;
using IoTService;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemplateAction.Common;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder
{
    public class JsContext
    {
        private RuleExecutionContext _context;
        public JsContext(RuleExecutionContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        public void SetParam(string key, object value)
        {
            _context.SetParam(key, value);
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public object GetParam(string key)
        {
            return _context.GetParam(key);
        }
        /// <summary>
        /// 获取节点的输入数据
        /// </summary>
        /// <returns></returns>
        public List<StreamData> Input()
        {
            return _context.Data;
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="obj"></param>
        public void Print(object obj)
        {
            if (_context.IsDebug)
            {
                TAAsyncHelper.RunSync(async () =>
                {
                    await _context.Print(obj).ConfigureAwait(false);
                });
            }
        }
        /// <summary>
        /// 清除事件沉默周期
        /// </summary>
        /// <param name="code"></param>
        public void ResetSilenceTime(string code)
        {
            IotRedisHelper redis = _context.Provider.GetService<IotRedisHelper>();
            redis.KeyDelete("IotQuick::" + _context.Source.DeviceId + "::" + code);
        }
        /// <summary>
        /// 执行指定设备的功能
        /// </summary>
        /// <param name="devId">为null表示当前设备</param>
        /// <param name="functionId"></param>
        /// <param name="data"></param>
        public IDictionary<string, object> ExeFunc(string devId, string functionId, IDictionary<string, object> data)
        {
            return TAAsyncHelper.RunSync<IDictionary<string, object>>(async () =>
            {
                var targetDevice = await _context.GetDevice(devId).ConfigureAwait(false);
                if (targetDevice == null)
                {
                    return null;
                }
                var rs = await _context.Provider.GetService<ServerBusProxy>().DownFunction(targetDevice.ProductId, targetDevice.DeviceId, functionId, data).ConfigureAwait(false);
                if (rs.IsSuccess())
                {
                    if (rs.Data == null)
                    {
                        return new Dictionary<string, object>();
                    }
                    return rs.Data;
                }
                else
                {
                    return null;
                }
            });

        }
        /// <summary>
        /// 获取指定设备属性信息
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> DeviceProp(string devId = null)
        {
            return TAAsyncHelper.RunSync(async () =>
            {
                if (devId == null)
                {
                    return await _context.DeviceProp().ConfigureAwait(false);
                }
                else
                {
                    var targetDevice = await _context.GetDevice(devId).ConfigureAwait(false);
                    if (targetDevice == null)
                    {
                        return new Dictionary<string, object>();
                    }
                    return await _context.DeviceProp(targetDevice.DeviceId).ConfigureAwait(false);
                }
            });
        }
        /// <summary>
        /// 通过dtuid获取设备属性信息
        /// </summary>
        /// <param name="dtuId"></param>
        /// <returns></returns>
        public IDictionary<string, object> DevicePropByDtuId(string dtuId)
        {
            return TAAsyncHelper.RunSync(async () =>
            {
                if (dtuId == null)
                {
                    return await _context.DeviceProp().ConfigureAwait(false);
                }
                else
                {
                    return await _context.DeviceProp(dtuId).ConfigureAwait(false);
                }
            });
        }
        /// <summary>
        /// 修改设备属性值
        /// </summary>
        /// <param name="dtuId">为null表示当前设备</param>
        /// <param name="code">属性标识符</param>
        /// <param name="val">修改的值</param>
        /// <returns></returns>
        public void SetProp(string dtuId, string code, object val)
        {
            TAAsyncHelper.RunSync(async () =>
            {
                await _context.SetProp(dtuId, code, val).ConfigureAwait(false);
            });
        }
        /// <summary>
        /// 获取指定设备标签信息
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> DeviceTag(string devId = null)
        {

            return TAAsyncHelper.RunSync(async () =>
            {
                return await _context.DeviceTag(devId).ConfigureAwait(false);
            });
        }
        /// <summary>
        /// 修改设备标签
        /// </summary>
        /// <param name="devId">为null表示当前设备</param>
        /// <param name="code">标签标识符</param>
        /// <param name="val"></param>
        public void SetTag(string devId, string code, object val)
        {
            TAAsyncHelper.RunSync(async () =>
            {
                var device = await _context.GetDevice(devId).ConfigureAwait(false);
                if (device == null)
                {
                    await _context.Print("设备不存在").ConfigureAwait(false);
                    return;
                }
                var model = await _context.GetTsl(device.ProductId).ConfigureAwait(false);
                if (model == null || model.Model.tags == null)
                {
                    await _context.Print("物模型或标签不存在").ConfigureAwait(false);
                    return;
                }
                BaseTagInfo tagInfo = model.Model.tags.Where(x => x.code == code).FirstOrDefault();
                if (tagInfo == null)
                {
                    await _context.Print("标签不存在").ConfigureAwait(false);
                    return;
                }
                await _context.UpdateTag(device, tagInfo, val).ConfigureAwait(false);
            });
        }
        /// <summary>
        /// 创建一条数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public StreamData CreateData(IDictionary<string, object> data)
        {
            return StreamData.Create(data);
        }

        /// <summary>
        /// 触发指定事件
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void TouchEvent(string code, IDictionary<string, object> data)
        {
            string warnProductId = _context.Source.ProductId;
            string warnDeviceId = _context.Source.DeviceId;

            TAAsyncHelper.RunSync(async () =>
            {
                await _context.Provider.GetService<ServerBusProxy>().SendEvent(warnProductId, warnDeviceId, code, data, null, _context.GetStartRuleId()).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// 获取节拍器的指定属性集合
        /// </summary>
        /// <param name="name">节拍名称</param>
        /// <returns></returns>
        public List<object> CountList(string name)
        {
            var countStep = _context.FindStepByName(name) as CountStep;
            if (countStep == null)
            {
                return new List<object>();
            }

            var streamlist = countStep.GetData();
            if (streamlist.Count > 0 && streamlist[0].Data != null)
            {
                return streamlist.Select(x => x.Data[countStep.FieldName]).ToList();
            }
            return new List<object>();
        }
        /// <summary>
        /// 查询历史记录数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="merge">合计方式：最大值:max，最小值：min，平均值：mean，合计：sum，期初值：first，期末值：last</param>
        /// <param name="window">合计窗口：0表示按日，1表示按月，2表示按时，-1表示无窗口</param>
        /// <param name="code">属性标识</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public List<Out_MergeItem> QueryMergeList(string id, string merge, int window, string code, DateTimeOffset beginTime, DateTimeOffset endTime)
        {
            return TAAsyncHelper.RunSync<List<Out_MergeItem>>(async () =>
            {
                var iotInfluxBLL = _context.Provider.GetService<IotInfluxBLL>();
                if (string.IsNullOrEmpty(id))
                {
                    var dev = await _context.GetDevice(id).ConfigureAwait(false);
                    if (dev == null)
                    {
                        return new List<Out_MergeItem>();
                    }
                    id = dev.Id;
                }
                In_HistoryMergeList query = new In_HistoryMergeList();
                query.Ids = new List<string>();
                query.Ids.Add(id);
                query.Code = code;
                query.MergeWay = new List<string>();
                query.MergeWay.Add(merge);
                query.WindowWay = window;
                query.BeginTime = beginTime.LocalDateTime;
                query.EndTime = endTime.LocalDateTime;
                var rsp = await iotInfluxBLL.SelectMergeList(query).ConfigureAwait(false);
                if (rsp.IsSuccess())
                {
                    return rsp.Data;
                }
                else
                {
                    return new List<Out_MergeItem>();
                }
            });
        }
        /// <summary>
        /// 执行下一个节点，追加数据流
        /// </summary>
        /// <param name="obj"></param>
        public void Next(List<StreamData> obj)
        {
            _context.Data.AddRange(obj);
            TAAsyncHelper.RunSync(async () =>
            {
                await _context.ExcuteNext(RuleResult.Next()).ConfigureAwait(false);
            });
        }
        /// <summary>
        /// 执行下一个节点，追加数据
        /// </summary>
        /// <param name="data"></param>
        public void NextObject(IDictionary<string, object> data)
        {
            _context.Data.Add(StreamData.Create(data));
            TAAsyncHelper.RunSync(async () =>
            {
                await _context.ExcuteNext(RuleResult.Next()).ConfigureAwait(false);
            });
        }
        /// <summary>
        /// Http的Get请求
        /// </summary>
        /// <param name="url"></param>
        public string HttpGet(string url)
        {
            return HttpHelper.Instance.Get(url);
        }

        /// <summary>
        /// Http的Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParams"></param>
        /// <returns></returns>
        public string HttpPost(string url, IDictionary<string, string> postParams)
        {
            return HttpHelper.Instance.Post(url, postParams);
        }
        /// <summary>
        /// Http的Post请求（Json格式）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParams"></param>
        /// <returns></returns>
        public string HttpPostJson(string url, object postParams)
        {
            return HttpHelper.Instance.PostJson(url, System.Text.Json.JsonSerializer.Serialize(postParams, MyDefaultTextJsonConfig.DefaultOptions), Encoding.UTF8);
        }
    }
}
