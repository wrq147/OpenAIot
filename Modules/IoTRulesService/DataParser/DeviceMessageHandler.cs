using AuthService;
using AuthService.Business;
using AuthService.DAL;
using ChannelUtility;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using Common;
using Common.EventBus;
using Common.Json;
using IoTRulesService.Flow.Builder;
using IoTService;
using IoTService.Business;
using IoTService.DAL;
using IoTService.Models;
using Jint;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.DataParser
{
    public class DeviceMessageHandler
    {
        private RuleWheelRuner _runner;
        private ILogger<DeviceMessageHandler> _log;
        private ITAServiceProvider _provider;
        private static readonly string[] GroupNames = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public DeviceMessageHandler(ILoggerFactory factory, ITAServiceProvider provider)
        {
            _provider = provider;
            _runner = _provider.GetService<RuleWheelRuner>();
            _log = factory.CreateLogger<DeviceMessageHandler>();
        }
        public async Task ParseMessage(RawUpDataMessage rs)
        {
            _runner.PushConcurrentTask(rs.DeviceId, async () =>
            {
                await _provider.GetService<PackParser>().rawDataTo(rs.DeviceId, rs.Data, rs.prefix, true).ConfigureAwait(false);
            });
        }
        public async Task ExeMessage(BaseUpDeviceMessage rs)
        {
            _runner.PushConcurrentTask(rs.DeviceId, async () =>
            {
                await _provider.GetService<DeviceMessageHandler>().AnalyseDeviceUp(rs).ConfigureAwait(false);
            });
        }
        private async Task AnalyseDeviceUp(BaseUpDeviceMessage rs)
        {
            try
            {
                DateTime nowTime = DateTime.Now;
                if (rs.Timestamp > 0)
                {
                    nowTime = MyAccess.Core.TypeConvert.Unix2Time(rs.Timestamp);
                }

                TslReturn model = null;
                var redis = _provider.GetService<IotRedisHelper>();
                switch (rs.MsgType)
                {
                    case "Online":
                        {
                            var deviceDAL = _provider.GetService<IotDeviceDAL>();
                            var serverBus = _provider.GetService<ServerBusProxy>();

                            DeviceOnlineMessage rdmsg = (DeviceOnlineMessage)rs;

                            var devicelist = await deviceDAL.SelectList(x => x.DeviceId == rs.DeviceId);
                            if (devicelist.Count > 0)
                            {
                                var product = await _provider.GetService<IotProductDAL>().Select(devicelist[0].ProductId);
                                if (product == null)
                                {
                                    await serverBus.Print(rs.DeviceId, "设备初始化", $"协议{devicelist[0].ProductId}不存在");
                                    return;
                                }
                                rs.ProductId = product.Id;

                                //存储设备在线状态
                                MZ_IotDevice device = new MZ_IotDevice();
                                device.Online = 1;
                                await deviceDAL.Update(device, x => x.DeviceId == rs.DeviceId);



                                //存储历史数据
                                if (!string.IsNullOrEmpty(product.StorageConfig))
                                {
                                    InfluxOption storageConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<InfluxOption>(product.StorageConfig);
                                    if (storageConfig != null && storageConfig.enable == "1")
                                    {
                                        await _provider.GetService<IotInfluxBLL>().SaveOnline(rs.ProductId, rs.DeviceId, devicelist[0].Id, storageConfig, nowTime);
                                    }
                                }

                                //查询设备是否需要下发查询ICCID（2G网络、3G网络、4G网络、5G网络的设备未绑定物联网卡时会自动下发查询）
                                if (product.PhysicsWay.IndexOf("G") != -1)
                                {
                                    await serverBus.DownICCID(product.Id, rs.DeviceId, product.NetworkWay);
                                }

                                //获取物模型
                                model = await TslCache.GetTslModel(product.Id, redis, _provider);

                                //处理在线事件
                                if (model.Model.events != null)
                                {
                                    var onlineEvts = model.Model.events.Where(x => x.CondType == 1);
                                    if (onlineEvts.Any())
                                    {
                                        var emptydata = new Dictionary<string, object>();
                                        foreach (var evtitem in onlineEvts)
                                        {
                                            await serverBus.SendEvent(rs.ProductId, rs.DeviceId, evtitem.code, emptydata);
                                        }
                                    }
                                }

                                #region 开始更新设备信息
                                Dictionary<string, string> dic = new Dictionary<string, string>();
                                dic.Add("$Id", devicelist[0].Id);
                                dic.Add("$ProductId", devicelist[0].ProductId);
                                dic.Add("$DeviceOrgIds", string.Format("{0},{1},{2}", devicelist[0].OrgId, devicelist[0].OwnerOrgId, devicelist[0].UseOrgId));
                                try
                                {
                                    if (model.Model.tags != null)
                                    {
                                        var needTags = model.Model.tags.Where(x => !string.IsNullOrEmpty(x.mapcode)).Select(x => x.code).ToList();
                                        if (needTags.Count > 0)
                                        {
                                            var initTags = await _provider.GetService<IotDeviceBLL>().SelectTagsDict(devicelist[0].Id, model.Model, needTags);
                                            foreach (var tagitem in initTags)
                                            {
                                                var tmptagii = model.Model.tags.Where(x => x.code == tagitem.Key).FirstOrDefault();
                                                if (tmptagii != null && !dic.ContainsKey(tmptagii.mapcode))
                                                {
                                                    dic.Add(tmptagii.mapcode, Newtonsoft.Json.JsonConvert.SerializeObject(new DevicePropertyValue()
                                                    {
                                                        val = tagitem.Value,
                                                        date = nowTime
                                                    }));
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _log.LogError(ex.StackTrace);
                                }
                                //读取所有绑定了属性的标签，并初始化属性
                                await redis.HashSetAsync("Device:" + rs.DeviceId, dic);
                                #endregion 结束更新设备信息

                                //如果设备位置标签未绑定属性，则使用Ip更新设备位置信息
                                if (!string.IsNullOrEmpty(rdmsg.IpAddress))
                                {
                                    var taginfo = model.Model.tags.Where(x => x.code == "position").FirstOrDefault();
                                    if (taginfo != null && string.IsNullOrEmpty(taginfo.mapcode))
                                    {
                                        var tagBLL = _provider.GetService<IotTagBLL>();
                                        var codeBLL = _provider.GetService<CodeBLL>();
                                        var iploca = await codeBLL.IpToArea(rdmsg.IpAddress);
                                        if (iploca != null)
                                        {
                                            await tagBLL.TagUpdateValue(taginfo, iploca, devicelist[0].Id);
                                        }
                                    }
                                }

                                if (string.IsNullOrEmpty(rdmsg.RedirectFromProductId))
                                {
                                    //下发初始化属性信息的modbus
                                    if (model.Model.modbus != null && model.Model.modbus.Matches != null)
                                    {
                                        var matches = model.Model.modbus.Matches;
                                        _runner.PushConcurrentTask(rs.DeviceId, async () =>
                                        {
                                            //初始化设备信息
                                            foreach (var matchItem in matches)
                                            {
                                                await serverBus.DownModbusMessage(product.Id, rs.DeviceId, product.NetworkWay, matchItem.Name).ConfigureAwait(false);
                                            }

                                        }, TimeSpan.FromSeconds(3));
                                    }
                                    //发送协议升级
                                    var channelConfig = await _provider.GetService<IotProductBLL>().GetChannel(product.NetworkWay);
                                    if (channelConfig.CanBind && (devicelist[0].ProductVer < product.Version))
                                    {
                                        var updateDAL = _provider.GetService<IotUpdateDAL>();
                                        await updateDAL.InsertDeviceUpdate(devicelist[0].Id, product.Version.Value, 10, product.OrgId.Value);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(rdmsg.RedirecDtuId))
                                    {
                                        //向源设备注册转发dtuid
                                        string tsourcekey = "Device:" + rdmsg.RedirecDtuId;
                                        var tids = await redis.HashGetAsync<string>(tsourcekey, "$targetDtuIds");
                                        HashSet<string> tmpset;
                                        if (string.IsNullOrEmpty(tids))
                                        {
                                            tmpset = new HashSet<string>();
                                        }
                                        else
                                        {
                                            tmpset = System.Text.Json.JsonSerializer.Deserialize<HashSet<string>>(tids, MyDefaultTextJsonConfig.DefaultOptions);
                                        }
                                        tmpset.Add(rdmsg.DeviceId);
                                        await redis.HashSetAsync<string>(tsourcekey, "$targetDtuIds", System.Text.Json.JsonSerializer.Serialize(tmpset, MyDefaultTextJsonConfig.DefaultOptions));
                                    }

                                }

                                //通知前端状态变更
                                await serverBus.NoticeChange(rs.DeviceId, "online");
                            }
                            else
                            {
                                await serverBus.Print(rs.DeviceId, "设备初始化", $"设备编号{rs.DeviceId}不存在");
                                return;
                            }

                        }
                        break;
                    case "Offline":
                        {
                            var deviceDAL = _provider.GetService<IotDeviceDAL>();

                            //通知转发设备离线
                            string tids = await redis.HashGetAsync<string>("Device:" + rs.DeviceId, "$targetDtuIds");
                            //存储设备离线状态
                            await redis.KeyDeleteAsync("Device:" + rs.DeviceId);
                            await redis.KeyDeleteAsync("DeviceSys:" + rs.DeviceId);

                            var serverBus = _provider.GetService<ServerBusProxy>();

                            await serverBus.PublishKeyDel("Offline:" + rs.DeviceId);


                            MZ_IotDevice device = new MZ_IotDevice();
                            device.Online = 0;
                            device.LastOnline = DateTime.Now;
                            await deviceDAL.Update(device, x => x.DeviceId == rs.DeviceId);

                            var devicelist = await deviceDAL.SelectList(x => x.DeviceId == rs.DeviceId);
                            if (devicelist.Count > 0)
                            {
                                rs.ProductId = devicelist[0].ProductId;

                                //通知前端状态变更
                                await serverBus.NoticeChange(rs.DeviceId, "Offline");

                                //处理离线事件
                                model = await TslCache.GetTslModel(rs.ProductId, redis, _provider);
                                if (model != null && model.Model.events != null)
                                {
                                    var offlineEvts = model.Model.events.Where(x => x.CondType == 2);
                                    if (offlineEvts.Any())
                                    {
                                        var emptydata = new Dictionary<string, object>();
                                        foreach (var evtitem in offlineEvts)
                                        {
                                            await serverBus.SendEvent(rs.ProductId, rs.DeviceId, evtitem.code, emptydata);
                                        }
                                    }
                                }


                                //存储历史数据
                                InfluxOption storageConfig = await redis.HashGetAsync<InfluxOption>("ProductSys:" + rs.ProductId, "$Storage");
                                if (storageConfig != null && storageConfig.enable == "1")
                                {
                                    await _provider.GetService<IotInfluxBLL>().SaveOffline(rs.ProductId, rs.DeviceId, devicelist[0].Id, storageConfig, nowTime);
                                }

                                //向转发设备发送离线
                                if (!string.IsNullOrEmpty(tids))
                                {
                                    HashSet<string> tmpset = System.Text.Json.JsonSerializer.Deserialize<HashSet<string>>(tids, MyDefaultTextJsonConfig.DefaultOptions);
                                    foreach (var tid in tmpset)
                                    {
                                        await serverBus.SendDisconnect(string.Empty, tid, rs.ProductId, rs.RuleIds, rs.DeviceId);
                                    }
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        break;
                    case "PropReply":
                        {
                            ReadPropertyMessageReply rdmsg = (ReadPropertyMessageReply)rs;
                            //根据物模型转换设备属性
                            model = await TslCache.GetTslModel(rs.ProductId, redis, _provider);
                            var deviceCahce = _provider.GetService<DeviceCache>();
                            //获取所有旧属性数据
                            var allDict = await deviceCahce.GetDevice(rdmsg.DeviceId);
                            if (allDict == null)
                            {
                                if (rdmsg.IsTagSync)
                                {
                                    //离线保存历史数据
                                    InfluxOption storageConfig = await redis.HashGetAsync<InfluxOption>("ProductSys:" + rs.ProductId, "$Storage");
                                    if (storageConfig != null && storageConfig.enable == "1")
                                    {
                                        Dictionary<string, DevicePropertyValue> saveDict = new Dictionary<string, DevicePropertyValue>();
                                        foreach (var kvp in rdmsg.Properties)
                                        {
                                            saveDict.Add(kvp.Key, new DevicePropertyValue()
                                            {
                                                val = kvp.Value,
                                                date = nowTime
                                            });
                                        }
                                        var deviceDAL = _provider.GetService<IotDeviceDAL>();
                                        var devicelist = await deviceDAL.SelectList(x => x.DeviceId == rs.DeviceId);
                                        if (devicelist.Count > 0)
                                        {
                                            await _provider.GetService<IotInfluxBLL>().SaveHistory(rdmsg.ProductId, rdmsg.DeviceId, devicelist[0].Id, storageConfig, nowTime, saveDict, model.Model.properties);
                                        }

                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(rdmsg.RedirectFromProductId))
                                    {
                                        //转发数据时，设备离线则发送上线报文
                                        await _provider.GetService<ServerBusProxy>().SendConnect(rdmsg.ProductId, rdmsg.DeviceId, "", rdmsg.RedirectFromProductId, rdmsg.RuleIds, rdmsg.RedirecDtuId);
                                    }
                                }
                                return;
                            }

                            //触发计算当前属性
                            if (string.IsNullOrEmpty(rdmsg.RedirectFromProductId))
                            {
                                if (!rdmsg.IsTagSync)
                                {
                                    rdmsg.Properties = model.Model.RawToProp(rdmsg.Properties, (tmpkk) =>
                                    {
                                        object tmpnewval = null;
                                        if (allDict.TryGetValue(tmpkk, out DevicePropertyValue newcalpop))
                                        {
                                            tmpnewval = newcalpop.val;
                                        }
                                        return tmpnewval;
                                    });
                                }
                            }
                            else
                            {
                                foreach (var item in rdmsg.Properties)
                                {
                                    var tprop = model.Model.properties.Where(x => x.code == item.Key).FirstOrDefault();
                                    if (tprop != null)
                                    {
                                        switch (tprop.option.type)
                                        {
                                            case "date":
                                                {
                                                    rdmsg.Properties[item.Key] = Convert.ToInt64(item.Value);
                                                }
                                                break;
                                            case "float":
                                                {
                                                    rdmsg.Properties[item.Key] = Convert.ToDouble(item.Value);
                                                }
                                                break;
                                            case "int":
                                                {
                                                    rdmsg.Properties[item.Key] = Convert.ToInt32(item.Value);
                                                }
                                                break;
                                            default:
                                                continue;
                                        }
                                    }
                                }
                            }
                            if (rdmsg.Properties.Count == 0)
                            {
                                return;
                            }

                            //触发其它计算属性
                            var propcals = model.Model.properties.Where(x => !string.IsNullOrEmpty(x.option.express) && x.option.express.Contains("prop") && !rdmsg.Properties.ContainsKey(x.code));
                            foreach (var propcalitem in propcals)
                            {
                                if (rdmsg.Properties.Keys.Any(x => propcalitem.option.express.Contains(x)))
                                {
                                    object tmpval = null;
                                    if (allDict.TryGetValue(propcalitem.code, out DevicePropertyValue calpop))
                                    {
                                        tmpval = calpop.val;
                                    }

                                    rdmsg.Properties[propcalitem.code] = propcalitem.option.RawTo(tmpval, (tmpkk) =>
                                    {
                                        object tmpnewval = null;
                                        if (!rdmsg.Properties.TryGetValue(tmpkk, out tmpnewval))
                                        {
                                            if (allDict.TryGetValue(tmpkk, out DevicePropertyValue newcalpop))
                                            {
                                                tmpnewval = newcalpop.val;
                                            }
                                        }
                                        return tmpnewval;
                                    });
                                }
                            }

                            string id = (string)allDict["$Id"].val;
                            var cache = _provider.GetService<CacheHelper>();
                            //显示用变化属性
                            Dictionary<string, DevicePropertyValue> showChangeDict = new Dictionary<string, DevicePropertyValue>();
                            foreach (var kvp in rdmsg.Properties)
                            {
                                List<BaseTagInfo> taglist = null;
                                if (model.Model.tags != null)
                                {
                                    taglist = model.Model.tags.Where(x => x.enable && x.mapcode == kvp.Key).ToList();
                                }

                                string overkey = "over#" + rdmsg.DeviceId + "#" + kvp.Key;
                                //数据有变化则更新，没有不更新
                                DevicePropertyValue tmpval;
                                if (allDict.TryGetValue(kvp.Key, out tmpval))
                                {
                                    string compareval = System.Text.Json.JsonSerializer.Serialize(kvp.Value ?? "", JsonMessageSerializerConfig.SerializeOptions);
                                    string tmpsourceValStr = System.Text.Json.JsonSerializer.Serialize(tmpval.val ?? "", JsonMessageSerializerConfig.SerializeOptions);

                                    if (tmpsourceValStr != compareval)
                                    {
                                        cache.SetCache(overkey, nowTime.ToString("o"), DateTime.Now.AddSeconds(60));

                                        allDict[kvp.Key] = new DevicePropertyValue()
                                        {
                                            val = kvp.Value,
                                            date = nowTime
                                        };
                                        if (showChangeDict.ContainsKey(kvp.Key))
                                        {
                                            showChangeDict[kvp.Key] = allDict[kvp.Key];
                                        }
                                        else
                                        {
                                            showChangeDict.Add(kvp.Key, allDict[kvp.Key]);
                                        }

                                    }
                                    else
                                    {
                                        if (tmpval.val is string)
                                        {
                                            continue;
                                        }

                                        var cacheVal = cache.GetCache<string>(overkey);
                                        if (cacheVal == null)
                                        {
                                            cache.SetCache(overkey, nowTime.ToString("o"), DateTime.Now.AddSeconds(60));
                                            if (showChangeDict.ContainsKey(kvp.Key))
                                            {
                                                showChangeDict[kvp.Key] = tmpval;
                                            }
                                            else
                                            {
                                                showChangeDict.Add(kvp.Key, tmpval);
                                            }
                                        }
                                        else
                                        {
                                            bool isovercache = false;
                                            if (DateTime.TryParse(cacheVal, out DateTime tmpdt))
                                            {
                                                if (tmpdt.AddSeconds(60) < DateTime.Now)
                                                {
                                                    cache.SetCache(overkey, nowTime.ToString("o"), DateTime.Now.AddSeconds(60));
                                                    if (showChangeDict.ContainsKey(kvp.Key))
                                                    {
                                                        showChangeDict[kvp.Key] = tmpval;
                                                    }
                                                    else
                                                    {
                                                        showChangeDict.Add(kvp.Key, tmpval);
                                                    }
                                                    isovercache = true;
                                                }
                                            }
                                            if (!isovercache)
                                            {
                                                if (taglist != null && !rdmsg.IsTagSync)
                                                {
                                                    if (cache.GetCache<string>(overkey + "#tag") != null)
                                                    {
                                                        taglist = null;
                                                    }
                                                    else
                                                    {
                                                        cache.SetCache(overkey + "#tag", "", DateTime.Now.AddMinutes(5));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    cache.SetCache(overkey, nowTime.ToString("o"), DateTime.Now.AddSeconds(60));
                                    var propitem = new DevicePropertyValue()
                                    {
                                        val = kvp.Value,
                                        date = nowTime
                                    };
                                    if (showChangeDict.ContainsKey(kvp.Key))
                                    {
                                        showChangeDict[kvp.Key] = propitem;
                                    }
                                    else
                                    {
                                        showChangeDict.Add(kvp.Key, propitem);
                                    }
                                    allDict.Add(kvp.Key, propitem);
                                }

                                //更新自定义标签
                                if (taglist != null && !rdmsg.IsTagSync)
                                {
                                    foreach (var tag in taglist)
                                    {
                                        //更新Tag信息
                                        var tagBLL = _provider.GetService<IotTagBLL>();
                                        if (!string.IsNullOrEmpty(id))
                                        {
                                            await tagBLL.TagUpdateValue(tag, kvp.Value, id);
                                        }
                                    }
                                }
                            }

                            //确认属性返回
                            var allnneList = deviceCahce.GetStartReadAll(rdmsg.DeviceId);
                            if (allnneList != null)
                            {
                                var loopvalues = allnneList.Values;
                                foreach (var allnne in loopvalues)
                                {
                                    foreach (var tmp in rdmsg.Properties)
                                    {
                                        if (allnne.needs.Contains(tmp.Key))
                                        {
                                            allnne.needs.Remove(tmp.Key);
                                        }
                                    }
                                    if (allnne.needs == null || allnne.needs.Count == 0)
                                    {
                                        ReadPropertyMessageReply readmm = new ReadPropertyMessageReply();
                                        readmm.ProductId = rdmsg.ProductId;
                                        readmm.DeviceId = rdmsg.DeviceId;
                                        readmm.Timestamp = rdmsg.Timestamp;
                                        Dictionary<string, object> retall = new Dictionary<string, object>();
                                        foreach (var tmp in allnne.props)
                                        {
                                            if (allDict.TryGetValue(tmp, out DevicePropertyValue dpv))
                                            {
                                                retall.Add(tmp, dpv.val);
                                            }
                                        }
                                        readmm.Properties = retall;
                                        await _provider.GetService<ServerBusProxy>().ConfirmPropertyReply(readmm);
                                        string tmppropkey = string.Join('#', allnne.props);
                                        allnneList.Remove(tmppropkey);
                                    }

                                }

                            }
                            var serverBus = _provider.GetService<ServerBusProxy>();
                            //有变更或超60秒数据变更
                            if (showChangeDict.Count > 0)
                            {
                                //触发属性事件
                                if (model.Model.events != null)
                                {
                                    var propsEvts = model.Model.events.Where(x => x.PropConditions != null && x.PropConditions.Length > 0 && x.CondType == 3);
                                    if (propsEvts.Any())
                                    {
                                        HashSet<string> ppkeys = new HashSet<string>();
                                        foreach (var evtitem in propsEvts)
                                        {
                                            foreach (var evtitemitem in evtitem.PropConditions)
                                            {
                                                if (!ppkeys.Contains(evtitemitem.code))
                                                {
                                                    ppkeys.Add(evtitemitem.code);
                                                }
                                            }
                                        }

                                        if (showChangeDict.Keys.Any(element => ppkeys.Contains(element)))
                                        {
                                            var emptydata = new Dictionary<string, object>();
                                            foreach (var evtitem in propsEvts)
                                            {
                                                Dictionary<string, bool> dic = new Dictionary<string, bool>();
                                                for (int i = 0; i < evtitem.PropConditions.Length; i++)
                                                {
                                                    dic.Add(GroupNames[i], evtitem.PropConditions[i].Check(allDict));
                                                }
                                                string txtGroup = evtitem.GroupTxt;
                                                if (string.IsNullOrEmpty(evtitem.GroupTxt))
                                                {
                                                    for (int i = 0; i < evtitem.PropConditions.Length; i++)
                                                    {
                                                        if (i == 0)
                                                        {
                                                            txtGroup = GroupNames[i];
                                                        }
                                                        else
                                                        {
                                                            txtGroup = txtGroup + "&" + GroupNames[i];
                                                        }
                                                    }
                                                }
                                                StringBuilder sb = new StringBuilder();
                                                foreach (char c in txtGroup)
                                                {
                                                    if (char.IsLetter(c))
                                                    {
                                                        sb.Append(dic[c.ToString()] ? "true" : "false");
                                                    }
                                                    else if (c == '&')
                                                    {
                                                        sb.Append("&&");
                                                    }
                                                    else if (c == '|')
                                                    {
                                                        sb.Append("||");
                                                    }
                                                    else
                                                    {
                                                        sb.Append(c);
                                                    }
                                                }
                                                var res = new Engine().Evaluate(sb.ToString());
                                                if (res.AsBoolean())
                                                {
                                                    await serverBus.SendEvent(rs.ProductId, rs.DeviceId, evtitem.code, emptydata);
                                                }
                                            }
                                        }


                                    }
                                }

                                //存储设备属性
                                string tkey = "Device:" + rdmsg.DeviceId;
                                if (!string.IsNullOrEmpty(rdmsg.RedirecDtuId))
                                {
                                    bool regissource = false;
                                    if (allDict.ContainsKey("$rawDtuId") && allDict.ContainsKey("$rawProductId"))
                                    {
                                        if (((string)allDict["$rawDtuId"].val) != rdmsg.RedirecDtuId || ((string)allDict["$rawProductId"].val) != rdmsg.RedirectFromProductId)
                                        {
                                            Dictionary<string, string> tmpdict = new Dictionary<string, string>();
                                            tmpdict.Add("$rawDtuId", rdmsg.RedirecDtuId);
                                            tmpdict.Add("$rawProductId", rdmsg.RedirectFromProductId);
                                            await redis.HashSetAsync(tkey, tmpdict);
                                            allDict["$rawDtuId"] = new DevicePropertyValue()
                                            {
                                                val = rdmsg.RedirecDtuId,
                                                date = nowTime
                                            };
                                            allDict["$rawProductId"] = new DevicePropertyValue()
                                            {
                                                val = rdmsg.RedirectFromProductId,
                                                date = nowTime
                                            };

                                            regissource = true;
                                        }
                                    }
                                    else
                                    {
                                        Dictionary<string, string> tmpdict = new Dictionary<string, string>();
                                        tmpdict.Add("$rawDtuId", rdmsg.RedirecDtuId);
                                        tmpdict.Add("$rawProductId", rdmsg.RedirectFromProductId);
                                        await redis.HashSetAsync(tkey, tmpdict);
                                        allDict.Add("$rawDtuId", new DevicePropertyValue()
                                        {
                                            val = rdmsg.RedirecDtuId,
                                            date = nowTime
                                        });

                                        allDict.Add("$rawProductId", new DevicePropertyValue()
                                        {
                                            val = rdmsg.RedirectFromProductId,
                                            date = nowTime
                                        });

                                        regissource = true;
                                    }

                                    if (regissource)
                                    {
                                        //向源设备注册转发dtuid
                                        string tsourcekey = "Device:" + rdmsg.RedirecDtuId;
                                        var tids = await redis.HashGetAsync<string>(tsourcekey, "$targetDtuIds");
                                        HashSet<string> tmpset;
                                        if (string.IsNullOrEmpty(tids))
                                        {
                                            tmpset = new HashSet<string>();
                                        }
                                        else
                                        {
                                            tmpset = System.Text.Json.JsonSerializer.Deserialize<HashSet<string>>(tids, MyDefaultTextJsonConfig.DefaultOptions);
                                        }
                                        tmpset.Add(rdmsg.DeviceId);
                                        await redis.HashSetAsync<string>(tsourcekey, "$targetDtuIds", System.Text.Json.JsonSerializer.Serialize(tmpset, MyDefaultTextJsonConfig.DefaultOptions));
                                    }
                                }

                                Dictionary<string, string> saveProps = new Dictionary<string, string>();

                                //通知前端更新数据，绑定了标签的不通知
                                HashSet<string> tagMapcode = new HashSet<string>();
                                if (model.Model.tags != null)
                                {
                                    foreach (var tag in model.Model.tags)
                                    {
                                        if (!string.IsNullOrEmpty(tag.mapcode))
                                        {
                                            tagMapcode.Add(tag.mapcode);
                                        }
                                    }
                                }
                                Dictionary<string, DevicePropertyValue> noticeProps = new Dictionary<string, DevicePropertyValue>();
                                foreach (var kvp in showChangeDict)
                                {
                                    saveProps.Add(kvp.Key, System.Text.Json.JsonSerializer.Serialize(kvp.Value, MyDefaultTextJsonConfig.DefaultOptions));
                                    if (!tagMapcode.Contains(kvp.Key) && kvp.Value.date == nowTime)
                                    {
                                        noticeProps.Add(kvp.Key, kvp.Value);
                                    }
                                }
                                saveProps.Add("$UpDate", nowTime.ToString("o"));
                                //redis保存实时数据
                                await redis.HashSetAsync(tkey, saveProps);

                                if (noticeProps.Count > 0)
                                {
                                    var waitdliis = await model.Model.PropertyList(noticeProps);
                                    await serverBus.NoticeChange(rs.DeviceId, System.Text.Json.JsonSerializer.Serialize(waitdliis, MyDefaultTextJsonConfig.DefaultOptions));
                                }


                                //存储历史数据
                                InfluxOption storageConfig = await redis.HashGetAsync<InfluxOption>("ProductSys:" + rs.ProductId, "$Storage");
                                if (storageConfig != null && storageConfig.enable == "1")
                                {
                                    var influxrs = await _provider.GetService<IotInfluxBLL>().SaveHistory(rdmsg.ProductId, rdmsg.DeviceId, id, storageConfig, nowTime, showChangeDict, model.Model.properties);
                                    if (!influxrs.IsSuccess())
                                    {
                                        await serverBus.Print(rs.DeviceId, "存储历史异常", influxrs.Message);
                                    }
                                }
                            }

                            //本地缓存
                            _provider.GetService<DeviceCache>().SetDevice(rdmsg.DeviceId, allDict);
                        }
                        break;
                    case "Event":
                        {
                            DeviceEventMessage rdmsg = (DeviceEventMessage)rs;
                            if (string.IsNullOrEmpty(rs.ProductId))
                            {
                                await _provider.GetService<ServerBusProxy>().Print(rs.DeviceId, "设备事件异常", "事件的协议Id不能为空");
                                return;
                            }
                            model = await TslCache.GetTslModel(rs.ProductId, redis, _provider);
                            if (model == null)
                            {
                                await _provider.GetService<ServerBusProxy>().Print(rs.DeviceId, "设备事件异常", "找不到对应的物模型");
                                return;
                            }
                            var evtinfo = model.Model.events.Where(x => x.code == rdmsg.EventId).FirstOrDefault();
                            if (evtinfo == null)
                            {
                                await _provider.GetService<ServerBusProxy>().Print(rs.DeviceId, "设备事件异常", "找不到对应的事件标识符");
                                return;
                            }

                            //生成事件描述信息
                            var allDict = await _provider.GetService<DeviceCache>().GetDevice(rdmsg.DeviceId);
                            string extInfo = evtinfo.description;
                            if (allDict != null)
                            {
                                var rpllist = allDict.OrderByDescending(x => x.Key.Length);
                                foreach (var output in rpllist)
                                {
                                    if (output.Key.StartsWith("$")) continue;
                                    extInfo = extInfo.Replace($"${output.Key}", (output.Value.val == null ? "" : output.Value.val.ToString()));
                                }
                                //清除$符号
                                extInfo = Regex.Replace(extInfo, "\\$[a-zA-Z0-9_]+", "");
                            }
                            if (string.IsNullOrEmpty(extInfo))
                            {
                                extInfo = "无描述信息";
                            }

                            //获取设备信息
                            var deviceList = await _provider.GetService<IotDeviceDAL>().SelectList(x => x.DeviceId == rdmsg.DeviceId);
                            if (evtinfo.Level >= 0)
                            {
                                var product = await _provider.GetService<IotProductDAL>().Select(rs.ProductId);
                                //根据事件的沉默周期限制报警频率
                                if (evtinfo.SilenceTime > 0)
                                {
                                    string devQuickKey = "IotQuick::" + rdmsg.DeviceId + "::" + rdmsg.EventId;
                                    string redisval = await redis.StringGetAsync(devQuickKey);
                                    if (redisval != null)
                                    {
                                        if (evtinfo.SilenceTime.ToString() != redisval)
                                        {
                                            await redis.KeyDeleteAsync(devQuickKey);
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                    await redis.StringSetAsync(devQuickKey, evtinfo.SilenceTime.ToString(), TimeSpan.FromSeconds(evtinfo.SilenceTime));
                                }

                                foreach (var device in deviceList)
                                {
                                    //添加告警
                                    var warnlist = await _provider.GetService<IotWarningBLL>().Insert(evtinfo, rdmsg, device, extInfo);
                                    //添加通知
                                    if (product != null && !string.IsNullOrEmpty(product.NoticeWay))
                                    {
                                        var noticeList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WarningNoticeItem>>(product.NoticeWay);
                                        await ExcuteNotice(_provider, evtinfo, device, noticeList, warnlist, extInfo);
                                    }
                                }
                            }
                            foreach (var device in deviceList)
                            {
                                //向其它服务通知事件
                                await BusUtility.Dispatch("DeviceEvent", new
                                {
                                    DtuId = device.DeviceId,
                                    DevId = device.Id,
                                    DevNumber = device.DeviceNumber,
                                    OrgId = device.OrgId,
                                    OwnerOrgId = device.OwnerOrgId,
                                    UseOrgId = device.UseOrgId,
                                    EventName = evtinfo.name,
                                    EventCode = evtinfo.code,
                                    EventInfo = extInfo,
                                    EventOutput = rdmsg.Outputs
                                });

                            }

                        }
                        break;
                    case "QueryICCIDReply":
                        {
                            QueryICCIDMessageReply rdmsg = (QueryICCIDMessageReply)rs;
                            var cardBLL = _provider.GetService<IotCardBLL>();
                            var deviceDAL = _provider.GetService<IotDeviceDAL>();
                            var devicelist = await deviceDAL.SelectList(x => x.DeviceId == rs.DeviceId);
                            if (devicelist.Count > 0)
                            {
                                var tmprsp = await cardBLL.BindDevice(rdmsg.iccid, devicelist[0]);
                                if (!tmprsp.IsSuccess())
                                {
                                    await _provider.GetService<ServerBusProxy>().Print(rs.DeviceId, "物联网卡异常", tmprsp.Message);
                                }
                            }
                        }
                        return;
                    case "ChangeProduct":
                        {
                            ChangeProductMessage rdmsg = (ChangeProductMessage)rs;
                            await _provider.GetService<IotDeviceBLL>().ChangeProductId(rdmsg.DeviceId, rdmsg.TargetProductId);
                        }
                        return;
                    case "TempProduct":
                        {
                            var deviceDAL = _provider.GetService<IotDeviceDAL>();
                            var devicelist = await deviceDAL.SelectList(x => x.DeviceId == rs.DeviceId);
                            if (devicelist.Count > 0)
                            {
                                await redis.StringSetAsync("TempDevice:" + rs.DeviceId, devicelist[0].ProductId, TimeSpan.FromSeconds(10));
                            }
                        }
                        return;
                    case "StartReadAll":
                        {
                            StartReadAllMessage rdmsg = (StartReadAllMessage)rs;
                            rdmsg.props.Sort();
                            string tmppropkey = string.Join('#', rdmsg.props);
                            if (string.IsNullOrEmpty(tmppropkey))
                            {
                                return;
                            }
                            var cahceDev = _provider.GetService<DeviceCache>();
                            var allnneList = cahceDev.GetStartReadAll(rdmsg.DeviceId);
                            if (allnneList == null)
                            {
                                allnneList = new Dictionary<string, WaitCache>();
                                var allnne = new WaitCache();
                                allnne.props = rdmsg.props;
                                allnne.needs = new HashSet<string>();
                                foreach (var tmp in allnne.props)
                                {
                                    allnne.needs.Add(tmp);
                                }

                                allnneList.Add(tmppropkey, allnne);
                                cahceDev.SetStartReadAll(rdmsg.DeviceId, allnneList);
                            }
                            else
                            {
                                if (allnneList.ContainsKey(tmppropkey))
                                {
                                    cahceDev.SetStartReadAll(rdmsg.DeviceId, allnneList);
                                }
                                else
                                {
                                    var allnne = new WaitCache();
                                    allnne.props = rdmsg.props;
                                    allnne.needs = new HashSet<string>();
                                    foreach (var tmp in allnne.props)
                                    {
                                        allnne.needs.Add(tmp);
                                    }
                                    allnneList.Add(tmppropkey, allnne);
                                    cahceDev.SetStartReadAll(rdmsg.DeviceId, allnneList);
                                }
                            }
                        }
                        return;
                    case "Empty":
                        return;
                }


                //异步执行规则
                await ExecuteRules(rs, model, nowTime, _provider, redis);
            }
            catch (Exception ex)
            {
                if (rs != null && !string.IsNullOrEmpty(rs.DeviceId))
                {
                    await _provider.GetService<ServerBusProxy>().Print(rs.DeviceId, "设备消息异常", ex.Message);
                }
                else
                {
                    _log.LogError(ex.Message + " -- 堆栈信息：" + ex.StackTrace);
                }
            }
        }



        /// <summary>
        /// 执行规则
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="model"></param>
        /// <param name="nowTime"></param>
        /// <param name="_provider"></param>
        /// <param name="redis"></param>
        /// <returns></returns>
        private async Task ExecuteRules(BaseUpDeviceMessage rs, TslReturn model, DateTime nowTime, ITAServiceProvider _provider, IotRedisHelper redis)
        {
            string productPath = "/" + rs.ProductId + "/-1";
            string devicePath = "/" + rs.ProductId + "/" + rs.DeviceId;
            string triggerMsg = rs.MsgType;
            if (rs.MsgType == "Event")
            {
                triggerMsg = rs.MsgType + "#" + ((DeviceEventMessage)rs).EventId;
            }
            var ruleCahce = _provider.GetService<RuleCache>();
            var ruleList = await ruleCahce.SelectByTriggers(productPath, devicePath, triggerMsg);
            ruleList.Sort((x, y) =>
            {
                return x.Sort.Value.CompareTo(y.Sort.Value);
            });
            List<StreamData> initdata = new List<StreamData>();
            switch (rs.MsgType)
            {
                case "PropReply":
                    initdata.Add(StreamData.Create(((ReadPropertyMessageReply)rs).Properties, nowTime));
                    break;
                case "FunctionReply":
                    initdata.Add(StreamData.Create(((FunctionInvokeMessageReply)rs).Outputs, nowTime));
                    break;
                case "Event":
                    initdata.Add(StreamData.Create(((DeviceEventMessage)rs).Outputs, nowTime));
                    break;
                default:
                    initdata.Add(StreamData.Create(new Dictionary<string, object>(), nowTime));
                    break;
            }
            foreach (var rule in ruleList)
            {
                //禁止递归触发规则
                if (rs.RuleIds != null && rs.RuleIds.Contains(rule.Id.Value))
                {
                    continue;
                }
                Dictionary<string, string> newparamType = new Dictionary<string, string>();
                Dictionary<string, object> newparams = new Dictionary<string, object>();

                //验证是否有输入参数
                var tmpstate = ruleCahce.GetRuleParamState(rule.Id.Value, rs.DeviceId);
                if (tmpstate == null || tmpstate == 0)
                {
                    var inputs = await redis.GetRuleVal(rule.Id.Value, "dtu-" + rs.DeviceId);
                    if (inputs != null && inputs.Count > 0)
                    {
                        foreach (var key in inputs.Keys)
                        {
                            newparams.Add(key, inputs[key]);
                        }
                    }
                    else
                    {
                        ruleCahce.SetRuleParamState(rule.Id.Value, rs.DeviceId, 1);
                    }
                }


                if (!string.IsNullOrEmpty(rule.HttpParams))
                {
                    var paramlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BaseInputValue>>(rule.HttpParams);
                    if (paramlist.Count > 0)
                    {
                        foreach (var item in paramlist)
                        {
                            newparamType.Add(item.code, item.type);
                            if (!newparams.ContainsKey(item.code))
                            {
                                object val;
                                if (item.defval != null)
                                {
                                    val = item.defval;
                                }
                                else
                                {
                                    continue;
                                }
                                newparams.Add(item.code, val);
                            }
                        }
                    }
                }

                var executor = await _provider.GetService<RuleExecutorBuilder>().Build(rule.RuleJson);
                var context = executor.CreateContext(rs, rule.Id.Value, newparams);
                context.IsDebug = ruleCahce.IsDebug(rule.Id.Value);
                context.ParamTypeDict = newparamType;
                context.Tsl = model;
                context.Data = initdata;
                context.ExecuteTime = nowTime;
                await executor.Execute(context);
                var outDict = context.GetGlobalParams();
                if (outDict != null && outDict.Count > 0)
                {
                    await redis.SaveRuleVal(rule.Id.Value, "dtu-" + rs.DeviceId, outDict);
                    ruleCahce.SetRuleParamState(rule.Id.Value, rs.DeviceId, 0);
                }
            }

        }



        /// <summary>
        /// 执行设备通知
        /// </summary>
        /// <param name="_provider"></param>
        /// <param name="evt"></param>
        /// <param name="device"></param>
        /// <param name="notices"></param>
        /// <param name="warnids"></param>
        /// <param name="extInfo"></param>
        /// <returns></returns>
        private async Task ExcuteNotice(ITAServiceProvider _provider, BaseEvent evt, MZ_IotDevice device, List<WarningNoticeItem> notices, List<Out_WarnTargetId> warnids, string extInfo)
        {
            long targetOrgId = 0;
            bool useNo = false;
            foreach (WarningNoticeItem noticeItem in notices)
            {
                var userDAL = _provider.GetService<UserDAL>();
                List<MZ_AdminInfo> users = new List<MZ_AdminInfo>();
                switch (noticeItem.TargetType)
                {
                    case 0:
                        targetOrgId = device.OrgId.Value;
                        users.Add(await userDAL.GetAdminByOrgId(Convert.ToInt64(noticeItem.TargetValue), device.OrgId.Value));
                        break;
                    case 1:
                        targetOrgId = device.OrgId.Value;
                        users = await userDAL.SelectUsersFrom(Convert.ToInt64(noticeItem.TargetValue), device.OrgId.Value);
                        break;
                    case 2:
                        targetOrgId = device.OwnerOrgId == null ? 0 : device.OwnerOrgId.Value;
                        if (targetOrgId > 0)
                        {
                            users = await userDAL.SelectManUsers(targetOrgId);
                        }
                        break;
                    case 3:
                        if (device.UseUserId != null && device.UseUserId > 0)
                        {
                            users.Add(await userDAL.GetAdminById(Convert.ToInt64(device.UseUserId)));
                        }
                        else
                        {
                            targetOrgId = device.UseOrgId == null ? 0 : device.UseOrgId.Value;
                            if (targetOrgId > 0 && !useNo)
                            {
                                var tnolist = await _provider.GetService<OrgExtDAL>().SelectList(x => x.OrgId == targetOrgId && x.ExtField == "IsNoRecv");
                                if (tnolist.Count > 0)
                                {
                                    useNo = Convert.ToBoolean(tnolist[0].ExtValue);
                                }
                                if (!useNo)
                                {
                                    users = await userDAL.SelectManUsers(targetOrgId);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (noticeItem.TargetType == 4)
                {
                    List<TargetUser> targets = new List<TargetUser>();
                    targets.Add(new TargetUser()
                    {
                        email = noticeItem.TargetValue,
                        phone = noticeItem.TargetValue
                    });
                    NoticeEvent nt = new NoticeEvent(2, targets.ToArray(), new string[] { noticeItem.NoticeWay });
                    nt.TargetType = "设备告警";
                    nt.Label = $"{device.Name}-{device.DeviceNumber}发生事件{evt.name}";
                    nt.Level = evt.Level;
                    nt.Content = extInfo;
                    nt.OrgId = targetOrgId;
                    var targetwarn = warnids.Where(x => x.OrgId == targetOrgId).FirstOrDefault();
                    if (targetwarn != null)
                    {
                        nt.TargetUrl = "/after/deviceManage/physicalModel/warnList?id=" + targetwarn.Id;
                    }
                    else
                    {
                        nt.TargetUrl = null;
                    }

                    await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                }
                else
                {
                    List<TargetUser> targets = new List<TargetUser>();
                    foreach (var us in users)
                    {
                        targets.Add(new TargetUser()
                        {
                            uid = us.Id.Value,
                            email = us.Email,
                            phone = us.Mobile
                        });
                    }
                    if (targets.Count > 0)
                    {
                        NoticeEvent nt = new NoticeEvent(2, targets.ToArray(), new string[] { noticeItem.NoticeWay });
                        nt.TargetType = "设备告警";
                        nt.OrgId = targetOrgId;
                        nt.Label = $"{device.Name}-{device.DeviceNumber}发生事件{evt.name}";
                        nt.Level = evt.Level;
                        nt.Content = extInfo;
                        var targetwarn = warnids.Where(x => x.OrgId == targetOrgId).FirstOrDefault();
                        if (targetwarn != null)
                        {
                            nt.TargetUrl = "/after/deviceManage/physicalModel/warnList?id=" + targetwarn.Id;
                        }
                        else
                        {
                            nt.TargetUrl = null;
                        }
                        await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                    }

                }
            }
        }
    }
}
