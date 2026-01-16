using ChannelUtility;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using Common;
using Common.EventBus;
using IoTRulesService.Flow.Builder.Step;
using IoTService;
using IoTService.Business;
using IoTService.DAL;
using IoTService.Models;
using Jint;
using Jint.Native;
using MonitorService.Model;
using NATS.Client.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder
{
    /// <summary>
    /// 规则执行上下文信息
    /// </summary>
    public class RuleExecutionContext
    {
        private ITAServiceProvider _provider;
        public ITAServiceProvider Provider
        {
            get { return _provider; }
        }
        private List<RuleflowStep> _steps;
        private string _instanceId;
        private long _ruleId;
        private int _start;
        private BaseDeviceMessage _source;
        private int _exeIndex;
        private List<StreamData> _dataList;
        /// <summary>
        /// 并行执行栈
        /// </summary>
        private Stack<List<StreamData>> _dataStack = new Stack<List<StreamData>>();
        /// <summary>
        /// 消息源
        /// </summary>
        public BaseDeviceMessage Source
        {
            get { return _source; }
        }
        /// <summary>
        /// 是否启用调试
        /// </summary>
        public bool IsDebug { get; set; }
        /// <summary>
        /// 节点列表
        /// </summary>
        public List<RuleflowStep> Steps
        {
            get { return _steps; }
        }
        public RuleflowStep FindStep(string id)
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                if (_steps[i].Id == id)
                {
                    return _steps[i];
                }
            }
            return null;
        }
        public RuleflowStep FindStepByName(string name)
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                if (_steps[i].Name == name)
                {
                    return _steps[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 规则流转数据
        /// </summary>
        public List<StreamData> Data
        {
            get
            {
                return _dataList;
            }
            set
            {
                _dataList = value;
            }
        }
        /// <summary>
        /// 触发时间
        /// </summary>
        public DateTime ExecuteTime { get; set; }
        /// <summary>
        /// 当前执行的索引位置
        /// </summary>
        public int ExcuteIndex
        {
            get
            {
                return _exeIndex;
            }
            set
            {
                _exeIndex = value;
            }
        }
        /// <summary>
        /// 开始位置
        /// </summary>
        public int StartIndex
        {
            get { return _start; }
        }
        /// <summary>
        /// 当前执行节点
        /// </summary>
        public RuleflowStep Step
        {
            get { return Steps[this.ExcuteIndex]; }
        }
        /// <summary>
        /// 实例Id
        /// </summary>
        public string RuleInstanceId
        {
            get { return _instanceId; }
        }
        /// <summary>
        /// 规则模板Id
        /// </summary>
        public long RuleId
        {
            get { return _ruleId; }
        }
        public HashSet<long> GetStartRuleId()
        {
            if (_source is BaseUpDeviceMessage upmsg)
            {
                if (upmsg.RuleIds != null)
                {
                    upmsg.RuleIds.Add(_ruleId);
                    return upmsg.RuleIds;
                }
            }
            return new HashSet<long>() { _ruleId };
        }
        /// <summary>
        /// 参数类型字典
        /// </summary>
        public Dictionary<string, string> ParamTypeDict { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 全局参数
        /// </summary>
        private Dictionary<string, object> _globalParam;
        public Dictionary<string, object> GetGlobalParams()
        {
            if (_globalParam != null)
            {
                return _globalParam.Where(kvp => kvp.Value != null)
                         .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            return _globalParam;
        }
        public void SetParam(string key, object value)
        {
            if (_globalParam.ContainsKey(key))
            {
                _globalParam[key] = value;
            }
            else
            {
                _globalParam.Add(key, value);
            }
        }
        public object GetParam(string key)
        {
            object rt;
            if (_globalParam.TryGetValue(key, out rt))
            {
                return rt;
            }
            return null;
        }
        public V GetParamByDefault<V>(string key, V v)
        {
            object rt;
            if (_globalParam.TryGetValue(key, out rt))
            {
                return TAConverter.Cast(rt, v);
            }
            return v;
        }
        public async Task ClearParam(string[] keys)
        {
            if (keys == null) return;
            foreach (string k in keys)
            {
                _globalParam.Remove(k);
            }
        }
        public bool FinishExecute { get; set; } = false;
        /// <summary>
        /// 定时执行任务
        /// </summary>
        public List<MZ_Job> JobList = new List<MZ_Job>();

        private TslReturn _tsl;
        public TslReturn Tsl { set { _tsl = value; } }
        private Dictionary<string, bool> _deviceOnlines = new Dictionary<string, bool>();
        private Dictionary<string, TslReturn> _tslModels = new Dictionary<string, TslReturn>();
        private Dictionary<string, MZ_IotDevice> _devices = new Dictionary<string, MZ_IotDevice>();
        private Dictionary<string, IDictionary<string, DevicePropertyValue>> _deviceProps = new Dictionary<string, IDictionary<string, DevicePropertyValue>>();
        private Dictionary<string, IDictionary<string, object>> _deviceTags = new Dictionary<string, IDictionary<string, object>>();
        public async Task<MZ_IotDevice> GetDevice(string id)
        {
            MZ_IotDevice device;
            if (string.IsNullOrEmpty(id))
            {
                device = _devices.Select(x => x.Value).Where(x => x.DeviceId == this.Source.DeviceId).FirstOrDefault();
                if (device == null)
                {
                    device = (await this._provider.GetService<IotDeviceDAL>().SelectList(x => x.DeviceId == this.Source.DeviceId)).FirstOrDefault();
                    _devices.Add(device.Id, device);
                }
            }
            else
            {
                if (!_devices.TryGetValue(id, out device))
                {
                    device = await this._provider.GetService<IotDeviceDAL>().Select(id);
                    _devices.Add(id, device);
                }
            }
            return device;
        }
        public async Task<MZ_IotDevice> GetDeviceByDtuId(string dtuId)
        {
            MZ_IotDevice device = _devices.Select(x => x.Value).Where(x => x.DeviceId == dtuId).FirstOrDefault();
            if (device == null)
            {
                var devlist = await this._provider.GetService<IotDeviceDAL>().SelectList(x => x.DeviceId == dtuId);
                if (devlist.Count > 0)
                {
                    device = devlist[0];
                    _devices.Add(device.Id, device);
                }
            }
            return device;
        }
        public async Task<bool> IsOnline(string id)
        {
            bool isonline;
            if (_deviceOnlines.TryGetValue(id, out isonline))
            {
                return isonline;
            }
            var device = await GetDevice(id);
            if (device == null)
            {
                return false;
            }

            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            isonline = await redis.KeyExistsAsync("Device:" + device.DeviceId);
            _deviceOnlines.Add(id, isonline);
            return isonline;
        }
        /// <summary>
        /// 判断是否有缓存物模型
        /// </summary>
        public async Task<TslReturn> GetTsl(string pid)
        {
            if (pid == this.Source.ProductId)
            {
                if (_tsl == null)
                {
                    _tsl = await TslCache.GetTslModel(this.Source.ProductId, this.Provider);
                }
                return _tsl;
            }
            else
            {
                TslReturn rt;
                if (!_tslModels.TryGetValue(pid, out rt))
                {
                    rt = await TslCache.GetTslModel(pid, this.Provider);
                    _tslModels.TryAdd(pid, rt);
                }
                return rt;
            }
        }

        public RuleExecutionContext(ITAServiceProvider provider, int start, long ruleId, BaseDeviceMessage source, Dictionary<string, object> globalParams, List<RuleflowStep> steps)
        {
            _ruleId = ruleId;
            _exeIndex = start;
            _dataList = new List<StreamData>();
            _source = source;
            _globalParam = globalParams;
            _start = start;
            _instanceId = MyAccess.Core.StringTool.GetGUID();
            _provider = provider;
            _steps = steps;
        }
        public async Task<IDictionary<string, DevicePropertyValue>> DevicePropTime(string dtuId = null)
        {
            IDictionary<string, DevicePropertyValue> tmpdata = null;
            if (string.IsNullOrEmpty(dtuId) || (!string.IsNullOrEmpty(this.Source.DeviceId) && dtuId == this.Source.DeviceId))
            {
                dtuId = this.Source.DeviceId;
                tmpdata = await this.Provider.GetService<DeviceCache>().GetDevice(dtuId);
            }
            else
            {
                if (!_deviceProps.TryGetValue(dtuId, out tmpdata))
                {
                    var redis = this.Provider.GetService<IotRedisHelper>();
                    var redisdict = await redis.HashGetAllAsync<string>("Device:" + dtuId);
                    if (redisdict != null)
                    {
                        tmpdata = DevicePropertyValue.FromDictStr(redisdict);
                        _deviceProps.Add(dtuId, tmpdata);
                    }
                }
            }
            if (tmpdata == null)
            {
                return new Dictionary<string, DevicePropertyValue>();
            }
            return tmpdata;
        }
        /// <summary>
        /// 获取设备属性信息
        /// </summary>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> DeviceProp(string dtuId = null)
        {
            var tmpdata = await DevicePropTime(dtuId);
            return DevicePropertyValue.ToDict(tmpdata);
        }
        /// <summary>
        /// 修改设备属性值
        /// </summary>
        /// <param name="dtuId">为null表示当前设备</param>
        /// <param name="code">属性标识符</param>
        /// <param name="val">修改的值</param>
        /// <returns></returns>
        public async Task SetProp(string dtuId, string code, object val)
        {
            if (string.IsNullOrEmpty(dtuId) || (!string.IsNullOrEmpty(this.Source.DeviceId) && dtuId == this.Source.DeviceId))
            {
                Dictionary<string, object> newvals = new Dictionary<string, object>();
                newvals.Add(code, val);
                await _provider.GetService<ServerBusProxy>().SendPropertyReply(this.Source.ProductId, this.Source.DeviceId, newvals, null, false, this.GetStartRuleId());
            }
            else
            {
                var dev = await GetDeviceByDtuId(dtuId);
                if (dev != null)
                {
                    Dictionary<string, object> newvals = new Dictionary<string, object>();
                    newvals.Add(code, val);
                    await _provider.GetService<ServerBusProxy>().SendPropertyReply(dev.ProductId, dtuId, newvals, null, false, this.GetStartRuleId());
                }
            }
        }
        public async Task ClearDevice(string[] ids)
        {
            foreach (string k in ids)
            {
                _deviceTags.Remove(k);
                _devices.Remove(k);
                var device = await GetDevice(k);
                if (device != null)
                {
                    _deviceProps.Remove(device.DeviceId);
                }
            }
        }

        /// <summary>
        /// 获取设备标签信息
        /// </summary>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> DeviceTag(string devId = null)
        {
            MZ_IotDevice device = await GetDevice(devId);
            if (device == null)
            {
                return new Dictionary<string, object>();
            }
            IDictionary<string, object> cacheDict;
            if (_deviceTags.TryGetValue(device.Id, out cacheDict))
            {
                return cacheDict;
            }

            var tsl = await GetTsl(device.ProductId);
            var rt = await this.Provider.GetService<IotDeviceBLL>().SelectTagsDict(device.Id, tsl.Model);
            _deviceTags.Add(device.Id, rt);
            return rt;
        }

        public async Task UpdateTag(MZ_IotDevice dev, BaseTagInfo tag, object val)
        {
            if (this.Source is ReadPropertyMessageReply readPropReply)
            {
                if (readPropReply.IsTagSync)
                {
                    if (readPropReply.Properties.ContainsKey(tag.mapcode))
                    {
                        return;
                    }
                }
            }


            await _provider.GetService<IotTagBLL>().TagUpdateValue(tag, val, dev.Id);

            object targetval = val;
            switch (tag.option.type)
            {
                case "int":
                    targetval = (long)((IntOption)tag.option).Limit(val);
                    break;
                case "float":
                    {
                        targetval = ((FloatOption)tag.option).Limit(val);
                    }
                    break;
            }
            //修改缓存
            IDictionary<string, object> cacheDict;
            if (_deviceTags.TryGetValue(dev.Id, out cacheDict))
            {
                if (cacheDict.ContainsKey(tag.code))
                {
                    cacheDict[tag.code] = targetval;
                }
                else
                {
                    cacheDict.Add(tag.code, targetval);
                }
            }

            //修改属性
            if (!string.IsNullOrEmpty(tag.mapcode) && tag.enable != false)
            {
                Dictionary<string, object> newvals = new Dictionary<string, object>();
                newvals.Add(tag.mapcode, targetval);
                await _provider.GetService<ServerBusProxy>().SendPropertyReply(dev.ProductId, dev.DeviceId, newvals, null, true, this.GetStartRuleId());
            }
        }

        /// <summary>
        /// 读取数据源值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<object> ReadSourceValue(string key)
        {
            if (key.StartsWith("$node"))
            {
                //解释节点变量
                string[] nodekeys = key.Split(".");
                if (nodekeys.Length == 3)
                {
                    var stp = _steps.Where(x => x.Id == nodekeys[1]).First();
                    if (stp != null)
                    {
                        if (nodekeys[2] == "IsActive")
                        {
                            return stp.IsActive;
                        }
                    }
                }
            }
            else if (key.StartsWith("$online"))
            {
                string[] nodekeys = key.Split(".");
                if (nodekeys.Length == 2)
                {
                    string devId = nodekeys[1];
                    return await IsOnline(devId);
                }
            }
            else if (key.StartsWith("$updelta"))
            {
                //获取设备属性最近一次变化间隔时间$updelta.设备Id（秒）
                string[] nodekeys = key.Split(".");
                if (nodekeys.Length == 2)
                {
                    string devId = nodekeys[1];
                    var targetDevice = await GetDevice(devId);
                    if (targetDevice == null)
                    {
                        return null;
                    }

                    var dictdata = await DevicePropTime(targetDevice.DeviceId);
                    DateTime? lasttime = null;
                    foreach (var kvp in dictdata)
                    {
                        if (lasttime == null)
                        {
                            lasttime = kvp.Value.date;
                        }
                        else
                        {
                            if (kvp.Value.date > lasttime.Value)
                            {
                                lasttime = kvp.Value.date;
                            }
                        }
                    }
                    if (lasttime != null)
                    {
                        var ts = DateTime.Now - lasttime.Value;
                        return Convert.ToInt32(ts.TotalSeconds);
                    }
                }
                return int.MaxValue;
            }
            else if (key.StartsWith("$param"))
            {
                string[] tmpkeys = key.Split(".");
                if (tmpkeys.Length == 2)
                {
                    string paramKey = tmpkeys[1];
                    object rt;
                    if (_globalParam.TryGetValue(paramKey, out rt))
                    {
                        return rt;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else if (key.StartsWith("$input"))
            {
                string[] tmpkeys = key.Split(".");
                if (tmpkeys.Length == 2)
                {
                    if (this.Data == null || this.Data.Count == 0)
                    {
                        return null;
                    }
                    string dataKey = tmpkeys[1];
                    object rt;
                    if (this.Data[0].Data.TryGetValue(dataKey, out rt))
                    {
                        return rt;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else if (key.StartsWith("$devprop"))
            {
                //获取其它设备的属性$devprop.设备Id.key
                string[] tmpkeys = key.Split(".");
                if (tmpkeys.Length == 3)
                {
                    string devId = tmpkeys[1];
                    string propKey = tmpkeys[2];
                    if (devId == _source.DeviceId && _source is ReadPropertyMessageReply)
                    {
                        key = propKey;
                    }
                    else
                    {
                        var targetDevice = await GetDevice(devId);
                        if (targetDevice == null)
                        {
                            return null;
                        }
                        IDictionary<string, object> propDict = await DeviceProp(targetDevice.DeviceId);
                        object tmpval;
                        if (propDict.TryGetValue(propKey, out tmpval))
                        {
                            return tmpval;
                        }
                        else
                        {
                            return null;
                        }
                    }


                }
            }
            else if (key.StartsWith("$devtag"))
            {
                //获取其它设备的标签$devtag.设备Id.key
                string[] tmpkeys = key.Split(".");
                if (tmpkeys.Length == 3)
                {
                    string devId = tmpkeys[1];
                    string tagKey = tmpkeys[2];

                    var tagDict = await this.DeviceTag(devId);
                    object tmpval;
                    if (tagDict.TryGetValue(tagKey, out tmpval))
                    {
                        return tmpval;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                switch (key)
                {
                    case "$now":
                        return MyAccess.Core.TypeConvert.Time2Unix(this.ExecuteTime);
                    case "$true":
                        return true;
                    case "$prop":
                        {
                            if (_source is ReadPropertyMessageReply rmmsg)
                            {
                                return rmmsg.Properties.Keys.ToArray();
                            }
                            else
                            {
                                return Array.Empty<string>();
                            }
                        }
                    case "$change":
                        {
                            if (_source.MsgType == "PropReply")
                            {
                                IDictionary<string, DevicePropertyValue> tmpdata = await this.Provider.GetService<DeviceCache>().GetDevice(this._source.DeviceId);
                                if (tmpdata == null)
                                {
                                    return Array.Empty<string>();
                                }
                                return tmpdata.Where(x => x.Value.date == this.ExecuteTime).ToList().Select(x => x.Key).ToArray();
                            }
                            else
                            {
                                return Array.Empty<string>();
                            }
                        }
                    case "$dtuId":
                        {
                            if (_source.MsgType != "Execute")
                            {
                                return _source.DeviceId;
                            }
                        }
                        break;
                    case "$devnumber":
                        {
                            if (_source.MsgType != "Execute")
                            {
                                var device = await this.GetDeviceByDtuId(_source.DeviceId);
                                if (device != null)
                                {
                                    return device.DeviceNumber;
                                }
                            }
                        }
                        break;
                }
            }
            if (!string.IsNullOrEmpty(this.Source.DeviceId))
            {
                var props = await DeviceProp(this.Source.DeviceId);
                object tmpval;
                string tmpkey = key;
                if (tmpkey.StartsWith("$"))
                {
                    tmpkey = tmpkey.Substring(1);
                }
                if (props.TryGetValue(tmpkey, out tmpval))
                {
                    return tmpval;
                }
            }
            return null;
        }

        public async Task<bool?> ReadSourceBool(string key)
        {
            object val = await ReadSourceValue(key);
            if (val == null)
            {
                return null;
            }

            return Convert.ToBoolean(val);
        }
        public async Task<double?> ReadSourceDouble(string key)
        {
            object val = await ReadSourceValue(key);
            if (val == null)
            {
                return null;
            }
            if (val is JObject obj)
            {
                return obj.ToObject<double>();
            }

            return Convert.ToDouble(val);
        }
        public async Task<long?> ReadSourceLong(string key)
        {
            object val = await ReadSourceValue(key);
            if (val == null)
            {
                return null;
            }
            if (val is JObject obj)
            {
                return obj.ToObject<long>();
            }

            return Convert.ToInt64(val);
        }
        public async Task<string> ReadSourceString(string key)
        {
            object val = await ReadSourceValue(key);
            if (val == null)
            {
                return null;
            }
            return val.ToString();
        }


        public Engine CreateJsEngine()
        {
            Engine jsEngine = new Engine();
            jsEngine.SetValue("$deviceId", _source.DeviceId);
            jsEngine.SetValue("$now", MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now));
            jsEngine.SetValue("$source", JsValue.FromObject(jsEngine, this.Source));

            return jsEngine;
        }
        public async Task Print(object msg)
        {
            var bus = _provider.GetService<NatsScope>().Bus;

            List<string> data = new List<string>();
            data.Add("console/zrule" + _ruleId);
            if (msg != null && msg is string objstr)
            {
                data.Add("节点'" + this.Step.Name + "'" + ":" + objstr);
            }
            else
            {
                data.Add("节点'" + this.Step.Name + "'" + ":" + Newtonsoft.Json.JsonConvert.SerializeObject(msg));
            }

            await bus.PublishAsync(new NatsMsg<List<string>>()
            {
                Subject = "MqttNotice.Msg",
                Data = data
            }, DefalutNatsJsonSerializer<List<string>>.Default);
        }

        public async Task ExeScript(string name, string script)
        {
            await Task.Run(() =>
            {
                var jsEngine = CreateJsEngine();
                jsEngine.Execute(script).Invoke(name, JsValue.FromObject(jsEngine, new JsContext(this)));
            });
        }


        /// <summary>
        /// 执行下一个节点
        /// </summary>
        /// <returns></returns>
        public async Task ExcuteNext(RuleResult rs)
        {

            if (rs.Directive == RuleResultDirective.EndRuleflow)
            {
                this.FinishExecute = true;
                return;
            }
            else
            {
                if (rs.ActiveChildren)
                {
                    if (rs.Parallel)
                    {
                        var children = this.Step.Children;
                        foreach (var idx in children)
                        {
                            _dataStack.Push(new List<StreamData>(_dataList.ToArray()));
                            this.ExcuteIndex = idx;
                            await this.Step.Run(this);
                            _dataList = _dataStack.Pop();

                            if (this.FinishExecute)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        this.ExcuteIndex = this.ExcuteIndex + 1;
                        await this.Step.Run(this);
                    }
                }
                else
                {
                    var nextlink = this.ExcuteIndex + 1;
                    while (nextlink < this.Steps.Count && this.Steps[nextlink].Level > this.Step.Level)
                    {
                        nextlink++;
                    }
                    this.ExcuteIndex = nextlink;
                    await this.Step.Run(this);
                }

            }
        }
    }
}
