using AuthService;
using AuthService.Business;
using AuthService.Model;
using ChannelUtility;
using ChannelUtility.Config;
using ChannelUtility.Tsl;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using JiebaNet.Segmenter;
using Jint;
using Microsoft.Extensions.Logging;
using MonitorService;
using MonitorService.DAL;
using MyAccess.Aop;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotDeviceBLL
    {
        private IotWarningDAL _warningDAL;
        private IotDeviceDAL _deviceDAL;
        private IotDeviceTagDAL _deviceTagDAL;
        private SnowflakeHelper _snowflake;
        private ITAServiceProvider _provider;
        private ILogger<IotDeviceBLL> _log;
        public IotDeviceBLL(IotWarningDAL warningDAL, IotDeviceDAL deviceDAL, IotDeviceTagDAL deviceTagDAL, SnowflakeHelper snowflake, ITAServiceProvider serviceProvider, ILoggerFactory logFactory)
        {
            _warningDAL = warningDAL;
            _deviceDAL = deviceDAL;
            _deviceTagDAL = deviceTagDAL;
            _snowflake = snowflake;
            _provider = serviceProvider;
            _log = logFactory.CreateLogger<IotDeviceBLL>();
        }

        public virtual async Task<BusResponse<string>> Import(List<MZ_IotDevice> devList, bool isUpdateSupport, IUserInfo operName)
        {
            if (devList == null || devList.Count == 0)
            {
                return BusResponse<string>.Error(12, "导入设备数据不能为空");
            }
            int failureNum = 0;
            StringBuilder failureMsg = new StringBuilder();

            foreach (MZ_IotDevice dev in devList)
            {
                try
                {
                    // 验证是否存在这台设备
                    var devlist = await _deviceDAL.SelectList(x => x.DeviceNumber == dev.DeviceNumber && x.OrgId == operName.OrgId);
                    var sourceDevice = devlist.FirstOrDefault();
                    if (sourceDevice == null)
                    {
                        BusResponse<string> rt = await this.Insert(dev, operName);
                        if (!rt.IsSuccess())
                        {
                            throw new Exception(rt.Message);
                        }
                    }
                    else if (isUpdateSupport)
                    {
                        dev.Id = sourceDevice.Id;
                        BusResponse<string> rt = await this.Update(dev, operName, sourceDevice);
                        if (!rt.IsSuccess())
                        {
                            throw new Exception(rt.Message);
                        }
                    }
                    else
                    {
                        failureNum++;
                        failureMsg.Append("<br/>" + failureNum + "、设备 " + dev.Name + " : 已存在");
                    }
                }
                catch (Exception e)
                {
                    failureNum++;
                    failureMsg.Append("<br/>" + failureNum + "、设备 " + dev.Name + " : " + e.Message);
                }
            }
            if (failureNum > 0)
            {
                failureMsg.Insert(0, "很抱歉，共 " + failureNum + " 条数据格式不正确，错误如下：");
                return BusResponse<string>.Error(23, failureMsg.ToString());
            }
            else
            {
                return BusResponse<string>.Success(null, "恭喜您，数据已全部导入成功！");
            }
        }

        public async Task<List<Out_DeviceRunStatistics>> RunStatisticsInfo(In_RunStatisticsInfo data)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            List<string> keys = new List<string>();
            keys.Add(user.OrgId.ToString());
            List<Out_DeviceRunStatistics> tlist = new List<Out_DeviceRunStatistics>();
            foreach (string item in data.StateList)
            {
                if (data.OrgId > 0 && data.OrgId != user.OrgId)
                {
                    var tmpcount = await _deviceDAL.SelectDStatusByOrgId(user, data.OrgId, item);
                    tlist.Add(new Out_DeviceRunStatistics()
                    {
                        state = item,
                        count = tmpcount
                    });
                }
                else
                {
                    string tmpstr = item;
                    var tmpcount = await _deviceDAL.Count(x => (x.OrgId == user.OrgId || SonSqlFun.FullSearch("OwnerOrgPath", keys) || x.UseOrgId == user.OrgId || x.UseUserId == user.UserId) && x.DState == tmpstr);
                    tlist.Add(new Out_DeviceRunStatistics()
                    {
                        state = item,
                        count = tmpcount
                    });
                }
            }
            return tlist;
        }
        public async Task<Out_DeviceStatistics> StatisticsInfo(long orgId)
        {
            Out_DeviceStatistics res = new Out_DeviceStatistics();
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);

            if (orgId > 0)
            {
                var staticInfo = await _deviceDAL.SelectStatusByOrgId(user, orgId);
                res.TotalCount = staticInfo.TotalCount;
                res.OnlineCount = staticInfo.OnlineCount;
                res.OfflineCount = staticInfo.OfflineCount;
                res.UnknowCount = staticInfo.UnknowCount;
                res.EventCount = await _warningDAL.SelectWaitCountByOrgId(user, orgId);
            }
            else
            {
                List<string> keys = new List<string>();
                keys.Add(user.OrgId.ToString());
                res.OnlineCount = await _deviceDAL.Count(x => (x.OrgId == user.OrgId || SonSqlFun.FullSearch("OwnerOrgPath", keys) || x.UseOrgId == user.OrgId || x.UseUserId == user.UserId) && x.Online == 1);
                res.OfflineCount = await _deviceDAL.Count(x => (x.OrgId == user.OrgId || SonSqlFun.FullSearch("OwnerOrgPath", keys) || x.UseOrgId == user.OrgId || x.UseUserId == user.UserId) && x.Online == 0);
                res.UnknowCount = await _deviceDAL.Count(x => (x.OrgId == user.OrgId || SonSqlFun.FullSearch("OwnerOrgPath", keys) || x.UseOrgId == user.OrgId || x.UseUserId == user.UserId) && x.Online == 2);
                res.TotalCount = res.OnlineCount + res.OfflineCount + res.UnknowCount;
                res.EventCount = await _warningDAL.SelectWaitCount(user);
            }

            return res;
        }

        public async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("SB");
        }
        public virtual async Task<PageObject<MZ_IotDevice>> ListPage(In_DeviceListPage query, IUserInfo user)
        {
            string classPath = null;
            if (query.ClassId != null)
            {
                MZ_IotClass classItem = await this._provider.GetService<IotClassDAL>().Select(query.ClassId);
                if (classItem != null)
                {
                    classPath = classItem.Path;
                }
            }

            var devpage = await _deviceDAL.SelectWithGroupPage(query, classPath, user);
            if (devpage.List.Count > 0)
            {
                var tmpcodeDict = await _provider.GetService<CodeBLL>().SelectAreaDict();
                foreach (var device in devpage.List)
                {
                    if (!string.IsNullOrEmpty(device.AreaCode))
                    {
                        if (device.AreaCode.EndsWith("0000"))
                        {
                            device.AreaCodeName = tmpcodeDict.MultiToName(device.AreaCode);
                        }
                        else if (device.AreaCode.EndsWith("00"))
                        {
                            device.AreaCodeName = tmpcodeDict.MultiToName(device.AreaCode.Substring(0, 2) + "0000," + device.AreaCode);
                        }
                        else
                        {
                            device.AreaCodeName = tmpcodeDict.MultiToName(device.AreaCode.Substring(0, 2) + "0000," + device.AreaCode.Substring(0, 4) + "00," + device.AreaCode);
                        }
                    }
                }
            }
            if (query.ShowTags == true)
            {
                var tagBLL = this._provider.GetService<IotTagBLL>();
                var productIds = devpage.List.Select(x => x.ProductId).Distinct().ToList();
                var devIds = devpage.List.Select(x => x.Id).ToList();
                if (productIds.Count > 0 && devIds.Count > 0)
                {
                    //获取物联协议
                    var productList = await this._provider.GetService<IotProductDAL>().SelectList(x => productIds.Contains(x.Id));
                    //获取设备标签值
                    var tagvalues = await this._provider.GetService<IotDeviceTagDAL>().SelectList(x => devIds.Contains(x.Id));

                    foreach (var dev in devpage.List)
                    {
                        var pro = productList.Where(x => x.Id == dev.ProductId).FirstOrDefault();
                        var tagVals = tagvalues.Where(x => x.Id == dev.Id).ToDictionary(x => x.Code);
                        if (pro != null)
                        {
                            dev.TagsView = new List<Out_TagItem>();
                            var model = TslModel.CreateFrom(pro.ModelTSL);
                            if (model != null)
                            {
                                foreach (var tg in model.tags)
                                {
                                    if (!tg.enable) continue;
                                    if (tagVals.TryGetValue(tg.code, out MZ_IotDeviceTag tmpval))
                                    {
                                        var item = tagBLL.ToTagItem(dev.Id, tg, tmpval);
                                        if (item != null)
                                        {
                                            dev.TagsView.Add(new Out_TagItem()
                                            {
                                                Name = tg.name,
                                                DisplayValue = item.DisplayValue
                                            });
                                        }
                                    }

                                }

                            }


                        }

                    }

                }
            }

            return devpage;
        }
        private async Task<MZ_IotDevice> _InitDevice(MZ_IotDevice device)
        {
            var product = await _provider.GetService<IotProductDAL>().Select(device.ProductId);
            if (product != null)
            {
                device.ProductName = product.Name;
            }

            if (device.OwnerOrgId != null && device.OwnerOrgId > 0)
            {
                var ownerOrg = await _provider.GetService<OrgDAL>().SelectById(device.OwnerOrgId.Value);
                if (ownerOrg != null)
                {
                    device.OwnerOrgName = ownerOrg.OrgName;
                }
            }
            if (!string.IsNullOrEmpty(device.AreaCode))
            {
                var tmpcodeDict = await _provider.GetService<CodeBLL>().SelectAreaDict();
                if (device.AreaCode.EndsWith("0000"))
                {
                    device.AreaCodeName = tmpcodeDict.MultiToName(device.AreaCode);
                }
                else if (device.AreaCode.EndsWith("00"))
                {
                    device.AreaCodeName = tmpcodeDict.MultiToName(device.AreaCode.Substring(0, 2) + "0000," + device.AreaCode);
                }
                else
                {
                    device.AreaCodeName = tmpcodeDict.MultiToName(device.AreaCode.Substring(0, 2) + "0000," + device.AreaCode.Substring(0, 4) + "00," + device.AreaCode);
                }
            }

            return device;
        }
        public virtual async Task<MZ_IotDevice> InfoByKey(string key)
        {
            var devlist = await _deviceDAL.SelectList(x => x.DeviceId == key || x.DeviceNumber == key);
            var device = devlist.FirstOrDefault();
            if (device == null)
            {
                return device;
            }
            return await _InitDevice(device);
        }
        public virtual async Task<MZ_IotDevice> InfoByDtuId(string dtuId, bool noInit = false)
        {
            var devlist = await _deviceDAL.SelectList(x => x.DeviceId == dtuId);
            var device = devlist.FirstOrDefault();
            if (device == null)
            {
                return device;
            }
            if (!noInit)
            {
                return await _InitDevice(device);
            }
            else
            {
                return device;
            }
        }
        public virtual async Task<MZ_IotDevice> InfoByNumber(string number, bool init = false)
        {
            var devlist = await _deviceDAL.SelectList(x => x.DeviceNumber == number);
            var device = devlist.FirstOrDefault();
            if (device == null)
            {
                return device;
            }
            if (init)
            {
                return await _InitDevice(device);
            }
            else
            {
                return device;
            }
        }
        public virtual async Task<MZ_IotDevice> Info(string id, bool init = false)
        {
            var device = await _deviceDAL.Select(id);
            if (device == null)
            {
                return device;
            }
            if (init)
            {
                return await _InitDevice(device);
            }
            else
            {
                return device;
            }
        }
        public virtual async Task<List<Out_DeviceFunc>> FuncList(string id)
        {
            var dev = await _deviceDAL.Select(id);
            if (dev == null)
            {
                return new List<Out_DeviceFunc>();
            }

            var model = await TslCache.GetTslModel(dev.ProductId, _provider);
            if (model == null)
            {
                return new List<Out_DeviceFunc>();
            }
            var context = _provider.GetService<ITAContext>();
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(context);
            List<Out_DeviceFunc> outList = new List<Out_DeviceFunc>();
            if (model.Model == null || model.Model.functions == null)
            {
                return new List<Out_DeviceFunc>();
            }


            Dictionary<string, object> devPropDict = null;
            foreach (var item in model.Model.functions)
            {
                bool canshow = false;
                if (item.showway == null)
                {
                    canshow = true;
                }
                else
                {
                    if (user.OrgId > 0)
                    {
                        if (user.OrgId == dev.OrgId)
                        {
                            if (item.showway.IndexOf("org") != -1)
                            {
                                canshow = true;
                            }
                        }
                        else if (user.OrgId == dev.OwnerOrgId)
                        {
                            if (item.showway.IndexOf("own") != -1)
                            {
                                canshow = true;
                            }
                        }
                        else if (user.OrgId == dev.UseOrgId)
                        {
                            if (item.showway.IndexOf("use") != -1)
                            {
                                canshow = true;
                            }
                        }
                        else
                        {
                            if (item.showway.IndexOf("person") != -1)
                            {
                                canshow = true;
                            }
                        }
                    }
                    else
                    {
                        if (item.showway.IndexOf("person") != -1)
                        {
                            canshow = true;
                        }
                    }
                }

                if (canshow)
                {
                    bool isDisabled = false;
                    if (item.conditions != null && item.conditions.Length > 0)
                    {
                        //首次初始化属性
                        if (devPropDict == null)
                        {
                            var redis = _provider.GetService<IotRedisHelper>();
                            var redisdict = await redis.HashGetAllAsync<string>("Device:" + dev.DeviceId);
                            if (redisdict != null && redisdict.Count > 0)
                            {
                                devPropDict = DevicePropertyValue.ToDict(DevicePropertyValue.FromDictStr(redisdict));
                            }
                            else
                            {
                                devPropDict = new Dictionary<string, object>();
                            }
                        }
                        string[] groupNames = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
                        Dictionary<string, bool> dic = new Dictionary<string, bool>();
                        for (int i = 0; i < item.conditions.Length; i++)
                        {
                            dic.Add(groupNames[i], item.conditions[i].Check(devPropDict));
                        }
                        string txtGroup = item.GroupTxt;
                        if (string.IsNullOrEmpty(item.GroupTxt))
                        {
                            for (int i = 0; i < item.conditions.Length; i++)
                            {
                                if (i == 0)
                                {
                                    txtGroup = groupNames[i];
                                }
                                else
                                {
                                    txtGroup = txtGroup + "&" + groupNames[i];
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
                        isDisabled = !res.AsBoolean();
                    }

                    if (isDisabled)
                    {
                        if (item.actionway == 0)
                        {
                            continue;
                        }
                        else if (item.actionway == 1)
                        {
                            outList.Add(new Out_DeviceFunc()
                            {
                                name = item.name,
                                code = item.code,
                                disabled = true,
                                description = item.description,
                                inputs = item.inputs
                            });
                        }
                    }
                    else
                    {
                        outList.Add(new Out_DeviceFunc()
                        {
                            name = item.name,
                            code = item.code,
                            disabled = false,
                            description = item.description,
                            inputs = item.inputs
                        });
                    }

                }
                else
                {
                    if (item.actionway == 0)
                    {
                        continue;
                    }
                    else if (item.actionway == 1)
                    {
                        outList.Add(new Out_DeviceFunc()
                        {
                            name = item.name,
                            code = item.code,
                            disabled = true,
                            description = item.description,
                            inputs = item.inputs
                        });
                    }
                }
            }
            return outList;
        }
        public virtual async Task<BusResponse<string>> UnUseDevice(string id)
        {
            var device = await _deviceDAL.Select(id);
            if (device == null)
            {
                return BusResponse<string>.Error(112, "设备不存在");
            }
            if (device.UseOrgId == 0 && device.UseUserId == 0)
            {
                return BusResponse<string>.Error(113, "设备未使用");
            }

            var context = _provider.GetService<ITAContext>();
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(context);
            if (device.OrgId != user.OrgId && device.OwnerOrgId != user.OrgId && device.UseOrgId != user.OrgId && device.UseUserId != user.UserId)
            {
                return BusResponse<string>.Error(114, "无解除设备使用的权限");
            }
            MZ_IotDevice newdevice = new MZ_IotDevice();
            newdevice.Id = id;
            newdevice.UseOrgId = 0;
            newdevice.UseUserId = 0;
            await _deviceDAL.Update(newdevice);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> UseDevice(string id, int t, IUserInfo user)
        {
            MZ_IotDevice device = null;
            if (t == 1)
            {
                var deviceList = await _deviceDAL.SelectList(x => x.DeviceId == id);
                device = deviceList.FirstOrDefault();
            }
            else if (t == 2)
            {
                var deviceList = await _deviceDAL.SelectList(x => x.DeviceNumber == id);
                device = deviceList.FirstOrDefault();
            }
            else
            {
                device = await _deviceDAL.Select(id);
            }
            if (device == null)
            {
                return BusResponse<string>.Error(112, "设备不存在");
            }
            var context = _provider.GetService<ITAContext>();
            if (user.OrgId > 0)
            {
                if (device.OwnerOrgId == user.OrgId || device.OrgId == user.OrgId)
                {
                    return BusResponse<string>.Success();
                }
                if (device.UseOrgId > 0)
                {
                    if (user.OrgId == device.UseOrgId)
                    {
                        return BusResponse<string>.Success();
                    }
                    In_JoinUser jusr = new In_JoinUser();
                    jusr.uid = user.UserId;
                    jusr.depId = 0;
                    jusr.postName = string.Empty;
                    var rs = await _provider.GetService<UserBLL>().Join(jusr, device.UseOrgId.Value);
                    if (!rs.IsSuccess())
                    {
                        return rs;
                    }
                    return new BusResponse<string>(1, string.Empty, device.UseOrgId.Value.ToString());
                }
                else
                {
                    MZ_IotDevice newdevice = new MZ_IotDevice();
                    newdevice.Id = device.Id;
                    newdevice.UseOrgId = user.OrgId;
                    await _deviceDAL.Update(newdevice);

                    Task xtt = _provider.GetService<IotRedisHelper>().HashDeleteAsync("Device:" + device.DeviceId, "$DeviceOrgIds");
                }
            }
            else
            {
                if (device.UseUserId > 0)
                {
                    if (user.UserId == device.UseUserId)
                    {
                        return BusResponse<string>.Success();
                    }
                    return BusResponse<string>.Error(113, "设备已被使用");
                }
                MZ_IotDevice newdevice = new MZ_IotDevice();
                newdevice.Id = device.Id;
                newdevice.UseUserId = user.UserId;
                await _deviceDAL.Update(newdevice);

                Task xtt = _provider.GetService<IotRedisHelper>().HashDeleteAsync("Device:" + device.DeviceId, "$DeviceOrgIds");
            }
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<IDictionary<string, object>>> ExeFunc(In_ExeFunc data, IUserInfo user, MZ_IotDevice device = null)
        {
            if (device == null)
            {
                device = await _deviceDAL.Select(data.Id);
            }
            if (device == null)
            {
                return BusResponse<IDictionary<string, object>>.Error(112, "设备不存在");
            }

            if (device.Online != 1)
            {
                return BusResponse<IDictionary<string, object>>.Error(121, "非在线设备无法执行功能");
            }
            if (string.IsNullOrEmpty(device.ProductId))
            {
                return BusResponse<IDictionary<string, object>>.Error(113, "设备未绑定协议");
            }

            var model = await TslCache.GetTslModel(device.ProductId, _provider);
            if (model == null)
            {
                return BusResponse<IDictionary<string, object>>.Error(114, "设备物模型不存在");
            }
            var context = _provider.GetService<ITAContext>();
            var func = model.Model.functions.Where(x => x.code == data.FunctionId).FirstOrDefault();
            if (func == null)
            {
                return BusResponse<IDictionary<string, object>>.Error(111, "设备没有这项功能");
            }
            bool canshow = false;
            if (user.OrgId > 0)
            {
                if (user.OrgId == device.OrgId)
                {
                    if (func.showway.IndexOf("org") != -1)
                    {
                        canshow = true;
                    }
                }
                else if (user.OrgId == device.OwnerOrgId)
                {
                    if (func.showway.IndexOf("own") != -1)
                    {
                        canshow = true;
                    }
                }
                else if (user.OrgId == device.UseOrgId)
                {
                    if (func.showway.IndexOf("use") != -1)
                    {
                        canshow = true;
                    }
                }
                else
                {
                    if (func.showway.IndexOf("person") != -1)
                    {
                        canshow = true;
                    }
                }
            }
            else
            {
                if (func.showway.IndexOf("person") != -1)
                {
                    canshow = true;
                }
            }

            if (!canshow)
            {
                return BusResponse<IDictionary<string, object>>.Error(115, "无权限执行这个功能");
            }

            IDictionary<string, object> inputs = func.CreateInputs();
            foreach (var kvp in data.Inputs)
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
            var res = await _provider.GetService<ServerBusProxy>().DownFunction(device.ProductId, device.DeviceId, data.FunctionId, inputs);

            #region 记录日志
            if (res.Data != null)
            {
                _provider.GetService<OperLogThread>().PushLog($"执行{device.Name}的功能{func.name}", System.Text.Json.JsonSerializer.Serialize(res.Data, MyDefaultTextJsonConfig.DefaultOptions));
            }
            else
            {
                _provider.GetService<OperLogThread>().PushLog($"执行{device.Name}的功能{func.name}", string.Empty);
            }
            #endregion

            return res;
        }

        public virtual async Task<bool> IsOnline(string id)
        {
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            return await redis.KeyExistsAsync("Device:" + id);
        }

        /// <summary>
        /// 获取设备实时数据（离线返回null）
        /// </summary>
        /// <param name="user">当前用户</param>
        /// <param name="id"></param>
        /// <param name="needTag">是否显示标签绑定的属性</param>
        /// <param name="sendWay">0不发送，1为只发送读取属性消息，2为同时等待属性返回</param>
        /// <param name="waitProps">需要等待的属性（不传则等待全部，多个用逗号隔开）</param>
        /// <returns></returns>
        public virtual async Task<BusResponse<List<DeviceProperty>>> Live(IUserInfo user, string id, bool needTag = false, int sendWay = 0, string waitProps = "")
        {
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            string deviceKey = "Device:" + id;
            string devOrgIds = null;
            Func<BaseProperty, Task<bool>> valfun = async (bp) =>
            {
                bool canshow = false;
                if (bp.showway == null)
                {
                    canshow = true;
                }
                else
                {
                    bool isorg = bp.showway.IndexOf("org") != -1;
                    bool isown = bp.showway.IndexOf("own") != -1;
                    bool isuse = bp.showway.IndexOf("use") != -1;
                    bool isother = bp.showway.IndexOf("person") != -1;
                    if (isother && isorg && isown && isuse)
                    {
                        canshow = true;
                    }
                    else if (!isother && !isorg && !isown && !isuse)
                    {
                        canshow = false;
                    }
                    else
                    {
                        if (devOrgIds == null)
                        {
                            MZ_IotDevice dev = (await _deviceDAL.SelectList(x => x.DeviceId == id)).FirstOrDefault();
                            if (dev == null)
                            {
                                return canshow;
                            }
                            devOrgIds = string.Format("{0},{1},{2}", dev.OrgId, dev.OwnerOrgId, dev.UseOrgId);
                            await redis.HashSetAsync(deviceKey, "$DeviceOrgIds", devOrgIds);
                        }
                        string[] torgids = devOrgIds.Split(',', StringSplitOptions.RemoveEmptyEntries);

                        if (user.OrgId == Convert.ToInt64(torgids[0]))
                        {
                            if (isorg)
                            {
                                canshow = true;
                            }
                        }
                        else if (user.OrgId == Convert.ToInt64(torgids[1]))
                        {
                            if (isown)
                            {
                                canshow = true;
                            }
                        }
                        else if (user.OrgId == Convert.ToInt64(torgids[2]))
                        {
                            if (isuse)
                            {
                                canshow = true;
                            }
                        }
                        else
                        {
                            if (isother)
                            {
                                canshow = true;
                            }
                        }
                    }
                }
                return canshow;
            };
            TslReturn model;
            //发送读取属性消息
            if (sendWay == 2)
            {
                List<string> tdatakeys = new List<string>();
                tdatakeys.Add("$ProductId");
                tdatakeys.Add("$rawProductId");
                tdatakeys.Add("$rawDtuId");
                tdatakeys.Add("$DeviceOrgIds");
                var tmpdict = await redis.HashGetListAsync<string>(deviceKey, tdatakeys);
                if (tmpdict.Count == 0)
                {
                    return BusResponse<List<DeviceProperty>>.Success(null);
                }
                string productId;
                if (!tmpdict.TryGetValue("$ProductId", out productId))
                {
                    return BusResponse<List<DeviceProperty>>.Success(null);
                }
                model = await TslCache.GetTslModel(productId, redis, _provider);
                if (model == null)
                {
                    return BusResponse<List<DeviceProperty>>.Error(111, "物模型不存在");
                }

                var bus = _provider.GetService<ServerBusProxy>();
                Dictionary<string, DevicePropertyValue> dict = new Dictionary<string, DevicePropertyValue>();
                List<string> tlist;
                if (string.IsNullOrEmpty(waitProps))
                {
                    tlist = model.Model.properties.Select(x => x.code).ToList();
                    if (tlist.Count <= 0)
                    {
                        return BusResponse<List<DeviceProperty>>.Success(new List<DeviceProperty>());
                    }
                }
                else
                {
                    tlist = waitProps.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                await bus.StartReadAllMessage(productId, id, tlist);
                string rawProductId = null;
                tmpdict.TryGetValue("$rawProductId", out rawProductId);
                string rawDtuId = null;
                tmpdict.TryGetValue("$rawDtuId", out rawProductId);
                if (rawProductId != null && rawDtuId != null)
                {
                    var rawmodel = await TslCache.GetTslModel(rawProductId, redis, _provider);
                    await _provider.GetService<ServerBusProxy>().DownReadProperty(rawProductId, rawDtuId, rawmodel.Model.properties.Select(x => x.code).ToList());
                }

                var prs = await _provider.GetService<ServerBusProxy>().WaitDownReadProperty(productId, id, tlist);
                //合并最新属性
                if (!prs.IsSuccess())
                {
                    return BusResponse<List<DeviceProperty>>.Error(113, prs.Message);
                }
                foreach (var kvp in prs.Data)
                {
                    if (dict.ContainsKey(kvp.Key))
                    {
                        dict[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        dict.Add(kvp.Key, kvp.Value);
                    }
                }

                HashSet<string> tagMapcode = new HashSet<string>();
                if (model.Model.tags != null && !needTag)
                {
                    foreach (var tag in model.Model.tags)
                    {
                        if (!string.IsNullOrEmpty(tag.mapcode) && tag.enable != false)
                        {
                            tagMapcode.Add(tag.mapcode);
                        }
                    }
                }

                foreach (var kvp in dict)
                {
                    if (tagMapcode.Contains(kvp.Key))
                    {
                        dict.Remove(kvp.Key);
                    }
                }

                //初始化设备所属信息
                tmpdict.TryGetValue("$DeviceOrgIds", out devOrgIds);
                return BusResponse<List<DeviceProperty>>.Success(await model.Model.PropertyList(dict, valfun));
            }
            else
            {
                Dictionary<string, string> dict = await redis.HashGetAllAsync<string>(deviceKey);
                if (dict.Count == 0)
                {
                    return BusResponse<List<DeviceProperty>>.Success(null);
                }
                string productId;
                if (!dict.TryGetValue("$ProductId", out productId))
                {
                    return BusResponse<List<DeviceProperty>>.Success(null);
                }
                string updatatime = string.Empty;
                dict.TryGetValue("$UpDate", out updatatime);

                model = await TslCache.GetTslModel(productId, redis, _provider);
                if (model == null)
                {
                    return BusResponse<List<DeviceProperty>>.Error(111, "物模型不存在");
                }


                if (sendWay == 1)
                {
                    string rawProductId = null;
                    dict.TryGetValue("$rawProductId", out rawProductId);
                    string rawDtuId = null;
                    dict.TryGetValue("$rawDtuId", out rawProductId);
                    if (rawProductId != null && rawDtuId != null)
                    {
                        var rawmodel = await TslCache.GetTslModel(rawProductId, redis, _provider);
                        if (rawmodel == null)
                        {
                            return BusResponse<List<DeviceProperty>>.Error(122, "原物模型不存在");
                        }
                        await _provider.GetService<ServerBusProxy>().DownReadProperty(rawProductId, rawDtuId, rawmodel.Model.properties.Select(x => x.code).ToList());
                    }
                    else
                    {
                        await _provider.GetService<ServerBusProxy>().DownReadProperty(productId, id, model.Model.properties.Select(x => x.code).ToList());
                    }
                }

                HashSet<string> tagMapcode = new HashSet<string>();
                if (model.Model.tags != null && !needTag)
                {
                    foreach (var tag in model.Model.tags)
                    {
                        if (!string.IsNullOrEmpty(tag.mapcode))
                        {
                            tagMapcode.Add(tag.mapcode);
                        }
                    }
                }

                Dictionary<string, DevicePropertyValue> inputs = new Dictionary<string, DevicePropertyValue>();
                foreach (var kvp in dict)
                {
                    if (kvp.Key.StartsWith("$") || tagMapcode.Contains(kvp.Key))
                    {
                        continue;
                    }
                    inputs.Add(kvp.Key, System.Text.Json.JsonSerializer.Deserialize<DevicePropertyValue>(kvp.Value, MyDefaultTextJsonConfig.DefaultOptions));
                }

                //初始化设备所属信息
                dict.TryGetValue("$DeviceOrgIds", out devOrgIds);

                return BusResponse<List<DeviceProperty>>.Success(await model.Model.PropertyList(inputs, valfun), updatatime);
            }
        }
        private async Task<BusResponse<string>> _saveTags(MZ_IotDevice device, MZ_IotProduct product, List<In_TagItem> list, DateTime? indate = null)
        {
            try
            {
                var model = TslModel.CreateFrom(product.ModelTSL);
                Dictionary<string, BaseTagInfo> tags = new Dictionary<string, BaseTagInfo>();
                if (model.tags != null)
                {
                    foreach (var t in model.tags)
                    {
                        tags.Add(t.code, t);
                    }
                    var tagBll = _provider.GetService<IotTagBLL>();
                    Dictionary<string, object> newvals = new Dictionary<string, object>();
                    foreach (var item in list)
                    {
                        BaseTagInfo tmpbaseInfo = tags[item.Code];
                        await tagBll.TagUpdateValue(tmpbaseInfo, item.Value, device.Id);
                        if (!string.IsNullOrEmpty(tmpbaseInfo.mapcode) && tmpbaseInfo.enable != false)
                        {
                            newvals.Add(tmpbaseInfo.mapcode, item.Value);
                        }
                    }

                    if (newvals.Count > 0)
                    {
                        await _provider.GetService<ServerBusProxy>().SendPropertyReply(device.ProductId, device.DeviceId, newvals, null, true, null, null, indate);
                    }
                }
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(115, ex.Message);
            }
        }
        public virtual async Task<BusResponse<string>> SaveTags(MZ_IotDevice device, List<In_TagItem> list, DateTime? indate)
        {
            var product = await _provider.GetService<IotProductDAL>().Select(device.ProductId);
            return await _saveTags(device, product, list, indate);
        }
        public virtual async Task<BusResponse<string>> SaveProps(string id, Dictionary<string, object> newvals, DateTime? indate)
        {
            MZ_IotDevice device = await _provider.GetService<IotDeviceDAL>().Select(id);
            if (device == null)
            {
                return BusResponse<string>.Error(111, "设备不存在");
            }

            await _provider.GetService<ServerBusProxy>().SendPropertyReply(device.ProductId, device.DeviceId, newvals, null, true, null, null, indate);
            return BusResponse<string>.Success();
        }
        /// <summary>
        /// 定时清除同步日志
        /// </summary>
        /// <returns></returns>
        public virtual async Task ClearSyncDeviceLog()
        {
            var jobLogDAL = _provider.GetService<JobLogDAL>();
            await jobLogDAL.DeleteJobLogByName("DeviceSystemUpdate", "SYSTEM");

            //更新关键词
            bool needUpdate = true;
            while (needUpdate)
            {
                needUpdate = await UpdateDeviceKeywords();
            }
        }
        /// <summary>
        /// 定时同步设备
        /// </summary>
        /// <returns></returns>
        public virtual async Task SyncDevice()
        {
            var updateDAL = _provider.GetService<IotUpdateDAL>();
            var tlist = await updateDAL.SelectUpdateList(200);
            foreach (var item in tlist)
            {
                var device = await _deviceDAL.Select(item.Id);
                if (device != null)
                {
                    try
                    {
                        var rs = await _provider.GetService<ServerBusProxy>().DownSyncDevice(device, item.Version.Value);
                        if (!rs.IsSuccess())
                        {
                            //更新失败的降级
                            MZ_IotUpdate newupdate = new MZ_IotUpdate();
                            newupdate.UpdateErr = rs.Message;
                            newupdate.UpdateCount = item.UpdateCount + 1;
                            newupdate.Level = item.Level + 1;
                            newupdate.UpdatedOn = DateTime.Now;
                            if (item.UpdateCount < 10)
                            {
                                newupdate.Status = 0;
                                await updateDAL.Update(newupdate, x => x.Id == item.Id);
                                continue;
                            }
                            else
                            {

                                if (device.OrgId != null)
                                {
                                    newupdate.Status = 2;
                                    await updateDAL.Update(newupdate, x => x.Id == item.Id);

                                    //发送错误提示组织的管理员
                                    var recvList = await _provider.GetService<UserDAL>().SelectManUsers(device.OrgId.Value);
                                    List<TargetUser> targets = new List<TargetUser>();
                                    foreach (var recvId in recvList)
                                    {
                                        targets.Add(new TargetUser()
                                        {
                                            uid = recvId.Id.Value,
                                            email = recvId.Email
                                        });
                                    }
                                    var nt = new NoticeEvent(2, targets.ToArray());
                                    nt.OrgId = device.OrgId.Value;
                                    nt.TargetType = "DeviceUpdate";
                                    nt.TargetUrl = "/iot/deviceManage/updateList";
                                    nt.Content = $"设备{device.Name}[{device.DeviceNumber}]下发协议更新失败，请尽快处理";
                                    nt.Label = "物联设备消息";
                                    await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                                    continue;
                                }


                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var logger = _provider.GetService<ILoggerFactory>().CreateLogger<IotDeviceBLL>();
                        logger.LogError(ex.Message);
                    }

                }
                await updateDAL.Delete(x => x.Id == item.Id);
            }

        }
        public async Task ChangeProductId(string dtuId, string targetProductId)
        {
            var devicelist = await _deviceDAL.SelectList(x => x.DeviceId == dtuId);
            var targetProduct = await _provider.GetService<IotProductDAL>().Select(targetProductId);
            if (devicelist.Count > 0 && targetProduct != null)
            {
                MZ_IotDevice dev = new MZ_IotDevice();
                dev.Id = devicelist[0].Id;
                dev.ProductId = targetProduct.Id;
                await _deviceDAL.Update(dev);
                devicelist[0].ProductId = targetProductId;
                await InitBindProduct(devicelist[0], targetProduct);
            }
        }
        private async Task InitBindProduct(MZ_IotDevice data, MZ_IotProduct product)
        {
            //同步物模型
            if (!string.IsNullOrEmpty(data.DeviceId))
            {
                var redis = _provider.GetService<IotRedisHelper>();
                var issuccess = await redis.KeyDeleteAsync("Device:" + data.DeviceId);
                if (issuccess)
                {
                    await _provider.GetService<ServerBusProxy>().PublishKeyDel("Device:" + data.DeviceId);
                    var updateDAL = _provider.GetService<IotUpdateDAL>();


                    var channelstr = await redis.HashGetAsync<string>("IotChannels", product.NetworkWay);
                    if (channelstr != null)
                    {
                        var config = System.Text.Json.JsonSerializer.Deserialize<ChannelConfig>(channelstr, MyDefaultTextJsonConfig.DefaultOptions);
                        if (config.CanBind)
                        {
                            await updateDAL.InsertDeviceUpdate(data.Id, product.Version.Value, 10, product.OrgId.Value);
                        }
                    }
                }
            }

        }
        public virtual async Task<BusResponse<string>> Update(MZ_IotDevice data, IUserInfo user, MZ_IotDevice old = null, bool enableEvt = true)
        {
            if (old == null)
            {
                old = await _deviceDAL.Select(data.Id);
            }
            if (old == null)
            {
                return BusResponse<string>.Error(114, "设备不存在");
            }
            if (old.OrgId != user.OrgId && old.OwnerOrgId != user.OrgId && old.UseOrgId != user.OrgId && old.UseUserId != user.UserId)
            {
                return BusResponse<string>.Error(116, "设备不在您的企业下");
            }
            bool dtuIdChange = false;
            if (!string.IsNullOrEmpty(data.DeviceId))
            {
                data.DeviceId = data.DeviceId.Trim();
                var tdevli = await _deviceDAL.SelectList(x => x.DeviceId == data.DeviceId && x.Id != data.Id);
                if (tdevli.Count > 0)
                {
                    return BusResponse<string>.Error(115, "设备编码已被使用");
                }
                if (old.DeviceId != data.DeviceId)
                {
                    dtuIdChange = true;
                }
            }
            if (string.IsNullOrEmpty(data.DeviceNumber))
            {
                data.DeviceNumber = null;
            }
            else
            {
                data.DeviceNumber = data.DeviceNumber.Trim();
                if (old.DeviceNumber == data.DeviceNumber)
                {
                    data.DeviceNumber = null;
                }
                else
                {
                    if (await _deviceDAL.Some(x => x.DeviceNumber == data.DeviceNumber))
                    {
                        return BusResponse<string>.Error(117, "设备编码已被使用");
                    }
                }
            }
            data.OrgId = null;
            data.OwnerOrgId = null;
            data.UseOrgId = null;
            data.UseUserId = null;
            if (data.DState == "")
            {
                data.DState = "正常";
            }
            if (!string.IsNullOrEmpty(data.DeviceId))
            {
                var serverBus = _provider.GetService<ServerBusProxy>();
                data.DeviceUpIdx = serverBus.GetIdx(data.DeviceId);
            }
            if ((data.Lat != null && data.Lng != null) && (data.Lat != 0 && data.Lng != 0))
            {
                data.GeoHash = MyAccess.Core.GeoHash.Encode(data.Lat.Value, data.Lng.Value);
                if (string.IsNullOrEmpty(data.AreaCode))
                {
                    var areaInfo = await _provider.GetService<CodeBLL>().SelectAreaByLatLng(data.Lng.Value, data.Lat.Value);
                    if (areaInfo != null)
                    {
                        data.AreaCode = areaInfo.Id;
                    }
                    else
                    {
                        data.AreaCode = string.Empty;
                    }
                }
            }

            bool isUpdateVersion = (data.ProductId != null && data.ProductId != old.ProductId) || dtuIdChange;
            if (isUpdateVersion)
            {
                data.ProductVer = 0;
                data.Online = 2;
            }

            var product = await _provider.GetService<IotProductDAL>().SelectProductView(old.ProductId);
            if ((data.Name != null && data.Name != old.Name) || (data.Remark != null && data.Remark != old.Remark))
            {
                var tclassDAL = _provider.GetService<IotClassDAL>();
                if (!string.IsNullOrEmpty(product.Path))
                {
                    var tmpPathList = product.Path.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var tmpParentClassList = await tclassDAL.SelectList(x => tmpPathList.Contains(x.Id));
                    var classNames = tmpParentClassList.Select(x => x.Name);
                    classNames = classNames.Concat(new JiebaSegmenter().CutForSearch(data.Name + " " + product.Name + " " + data.Remark));
                    data.KeyWords = string.Join(",", classNames.Where(x => x != " " && x != ""));
                }
                else
                {
                    data.KeyWords = string.Join(",", new JiebaSegmenter().CutForSearch(data.Name + " " + product.Name + " " + data.Remark));
                }
            }

            await _deviceDAL.Update(data);

            if (isUpdateVersion)
            {
                old.ProductId = data.ProductId;
                if (data.DeviceId != null)
                {
                    old.DeviceId = data.DeviceId;
                }
                await InitBindProduct(old, product);
            }
            else
            {
                if (data.DeviceId != old.DeviceId)
                {
                    var redis = _provider.GetService<IotRedisHelper>();
                    await redis.KeyDeleteAsync("Device:" + old.DeviceId);
                    await _provider.GetService<ServerBusProxy>().PublishKeyDel("Device:" + old.DeviceId);
                }
            }

            if (enableEvt)
            {
                //触发事件
                await BusUtility.Dispatch("FromIOTDevice", new
                {
                    Id = data.Id,
                    OrgId = old.OrgId,
                    Number = data.DeviceNumber ?? old.DeviceNumber,
                    Name = data.Name ?? old.Name,
                    PhotoUrl = data.PhotoUrl ?? old.PhotoUrl,
                    MesProductId = data.MesProductId ?? old.MesProductId
                });
            }


            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<string>> Insert(MZ_IotDevice data, IUserInfo user, bool enableEvt = true)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无法添加设备");
            }
            if (string.IsNullOrEmpty(data.MesProductId))
            {
                return BusResponse<string>.Error(133, "请选择产品");
            }
            if (string.IsNullOrEmpty(data.ProductId))
            {
                return BusResponse<string>.Error(116, "协议Id不能为空");
            }

            if (string.IsNullOrEmpty(data.DeviceNumber))
            {
                data.DeviceNumber = await GenerateNumber();
            }
            else
            {
                if (await _deviceDAL.Some(x => x.DeviceNumber == data.DeviceNumber))
                {
                    return BusResponse<string>.Error(117, "设备编码已被使用");
                }
            }
            if (!string.IsNullOrEmpty(data.DeviceId))
            {
                var tdevli = await _deviceDAL.SelectList(x => x.DeviceId == data.DeviceId);
                if (tdevli.Count > 0)
                {
                    return BusResponse<string>.Error(115, "通讯编码已被使用");
                }
            }

            var serverBus = _provider.GetService<ServerBusProxy>();
            data.DeviceUpIdx = serverBus.GetIdx(data.DeviceId);
            MZ_IotProduct product = await _provider.GetService<IotProductDAL>().SelectProductView(data.ProductId);
            if (product == null)
            {
                return BusResponse<string>.Error(118, "协议Id不存在");
            }

            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.OwnerOrgId = 0;
            data.UseOrgId = 0;
            data.UseUserId = 0;
            data.Online = 2;
            data.CreateOn = DateTime.Now;
            data.ProductVer = 0;
            data.PhotoUrl ??= string.Empty;
            data.Remark ??= string.Empty;
            data.OwnerOrgPath ??= string.Empty;
            if (string.IsNullOrEmpty(data.DState))
            {
                data.DState = "正常";
            }
            data.DeviceId ??= string.Empty;
            data.DeviceId = data.DeviceId.Trim();
            data.DeviceNumber = data.DeviceNumber.Trim();
            data.NeedUpdateKey = false;
            data.dBm ??= 0;


            var tclassDAL = _provider.GetService<IotClassDAL>();
            if (!string.IsNullOrEmpty(product.Path))
            {
                var tmpPathList = product.Path.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var tmpParentClassList = await tclassDAL.SelectList(x => tmpPathList.Contains(x.Id));
                var classNames = tmpParentClassList.Select(x => x.Name);
                classNames = classNames.Concat(new JiebaSegmenter().CutForSearch(data.Name + " " + product.Name + " " + data.Remark));
                data.KeyWords = string.Join(",", classNames.Where(x => x != " " && x != ""));
            }
            else
            {
                data.KeyWords = string.Join(",", new JiebaSegmenter().CutForSearch(data.Name + " " + product.Name + " " + data.Remark));
            }

            await _deviceDAL.Insert(data);


            if (enableEvt)
            {
                //触发事件
                await BusUtility.Dispatch("FromIOTDevice", new
                {
                    Id = data.Id,
                    OrgId = user.OrgId,
                    Number = data.DeviceNumber,
                    Name = data.Name,
                    PhotoUrl = data.PhotoUrl,
                    MesProductId = data.MesProductId
                });

            }
            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<string>> Remove(string id, IUserInfo user)
        {
            var old = await _deviceDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(114, "设备不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(116, "当前用户无权限");
            }
            if (old.UseOrgId > 0 || old.UseUserId > 0 || old.OwnerOrgId > 0)
            {
                return BusResponse<string>.Error(115, "设备已入库或使用无法删除");
            }
            if (!string.IsNullOrEmpty(old.DeviceId))
            {
                var redis = _provider.GetService<IotRedisHelper>();
                await redis.KeyDeleteAsync("Device:" + old.DeviceId);
                await _provider.GetService<ServerBusProxy>().PublishKeyDel("Device:" + old.DeviceId);
            }


            var tagBLL = _provider.GetService<IotTagBLL>();

            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await tagBLL.Remove(id);
                    await _deviceDAL.Delete(id);
                    await scope.CompleteAsync();
                }

                //向其它服务通知事件
                await BusUtility.Dispatch("IOTDeviceDel", new
                {
                    DevId = old.Id,
                    OrgId = old.OrgId
                });

                return BusResponse<string>.Success();
            }
            catch
            {
                return BusResponse<string>.Error(118, "服务繁忙，请重试");
            }
        }

        public virtual async Task<Dictionary<string, object>> SelectTagsDict(string id, TslModel model, List<string> codeList = null)
        {
            List<MZ_IotDeviceTag> list = null;
            if (!string.IsNullOrEmpty(id))
            {
                if (codeList == null || codeList.Count == 0)
                {
                    list = await _deviceTagDAL.SelectList(x => x.Id == id);
                }
                else
                {
                    list = await _deviceTagDAL.SelectList(x => x.Id == id && codeList.Contains(x.Code));
                }
            }
            Dictionary<string, MZ_IotDeviceTag> tagSaves = new Dictionary<string, MZ_IotDeviceTag>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    tagSaves.Add(item.Code, item);
                }
            }

            Dictionary<string, object> tagVals = new Dictionary<string, object>();
            foreach (var tag in model.tags)
            {
                if (tag.code == "position" || tag.code == "state" || tag.code == "signal")
                {
                    continue;
                }
                MZ_IotDeviceTag tmptagval;
                if (!tagSaves.TryGetValue(tag.code, out tmptagval))
                {
                    MZ_IotDeviceTag deftag = new MZ_IotDeviceTag();
                    deftag.Name = tag.name;
                    deftag.Id = id;
                    switch (tag.option.type)
                    {
                        case "int":
                            tagVals.Add(tag.code, Convert.ToInt64(tag.value));
                            break;
                        case "float":
                            tagVals.Add(tag.code, Convert.ToDouble(tag.value));
                            break;
                        case "boolean":
                            tagVals.Add(tag.code, Convert.ToBoolean(tag.value));
                            break;
                        case "date":
                            tagVals.Add(tag.code, Convert.ToInt64(tag.value));
                            break;
                        case "enum":
                            tagVals.Add(tag.code, (tag.value ?? "").ToString());
                            break;
                        case "string":
                            tagVals.Add(tag.code, tag.value.ToString());
                            break;
                        default:
                            tagVals.Add(tag.code, System.Text.Json.JsonSerializer.Deserialize<object>(tag.value.ToString(), MyDefaultTextJsonConfig.DefaultOptions));
                            break;
                    }
                }
                else
                {
                    switch (tag.option.type)
                    {
                        case "int":
                            tagVals.Add(tag.code, Convert.ToInt64(tmptagval.NumValue));
                            break;
                        case "float":
                            tagVals.Add(tag.code, Convert.ToDouble(tmptagval.NumValue));
                            break;
                        case "boolean":
                            tagVals.Add(tag.code, Convert.ToBoolean(tmptagval.Value));
                            break;
                        case "date":
                            tagVals.Add(tag.code, Convert.ToInt64(tmptagval.NumValue));
                            break;
                        case "enum":
                            tagVals.Add(tag.code, tmptagval.Value);
                            break;
                        case "string":
                            tagVals.Add(tag.code, tmptagval.Value);
                            break;
                        default:
                            tagVals.Add(tag.code, System.Text.Json.JsonSerializer.Deserialize<object>(tmptagval.Value, MyDefaultTextJsonConfig.DefaultOptions));
                            break;
                    }
                }
            }
            return tagVals;
        }
        public virtual async Task<List<Out_DeviceTagItem>> SelectTagsByProduct(string id)
        {
            var product = await _provider.GetService<IotProductDAL>().Select(id);
            var model = TslModel.CreateFrom(product.ModelTSL);
            if (model.tags == null)
            {
                return new List<Out_DeviceTagItem>();
            }
            var tagBLL = _provider.GetService<IotTagBLL>();
            Dictionary<string, object> tagVals = await SelectTagsDict(null, model);
            List<Out_DeviceTagItem> rs = new List<Out_DeviceTagItem>();
            foreach (var tg in model.tags)
            {
                if (!tg.enable) continue;
                object tmpval;
                if (tagVals.TryGetValue(tg.code, out tmpval))
                {
                    var item = tagBLL.ToTagItem(string.Empty, tg, tmpval);
                    if (item != null)
                    {
                        rs.Add(item);
                    }
                }

            }
            return rs;
        }
        public virtual async Task<List<Out_DeviceTagItem>> SelectTagsByDevice(MZ_IotDevice device, TslModel model = null)
        {
            if (model == null)
            {
                var product = await _provider.GetService<IotProductDAL>().Select(device.ProductId);
                model = TslModel.CreateFrom(product.ModelTSL);
                if (model.tags == null)
                {
                    return new List<Out_DeviceTagItem>();
                }
            }
            var tagBLL = _provider.GetService<IotTagBLL>();
            Dictionary<string, object> tagVals = await SelectTagsDict(device.Id, model);
            List<Out_DeviceTagItem> rs = new List<Out_DeviceTagItem>();
            foreach (var tg in model.tags)
            {
                if (!tg.enable) continue;
                object tmpval;
                if (tagVals.TryGetValue(tg.code, out tmpval))
                {
                    var item = tagBLL.ToTagItem(device.Id, tg, tmpval);
                    if (item != null)
                    {
                        rs.Add(item);
                    }
                }

            }
            return rs;
        }

        public async Task<bool> UpdateDeviceKeywords(long? orgId = null)
        {
            var deviceDAL = this._provider.GetService<IotDeviceDAL>();
            List<MZ_IotDevice> tlist;
            if (orgId != null)
            {
                var tmppage = await deviceDAL.SelectPage(x => x.OrgId == orgId && x.NeedUpdateKey == true, new BaseQueryParam()
                {
                    pageNum = 1,
                    pageSize = 1000,
                    showAll = true
                }, string.Empty);
                tlist = tmppage.List;
            }
            else
            {
                var tmppage = await deviceDAL.SelectPage(x => x.NeedUpdateKey == true, new BaseQueryParam()
                {
                    pageNum = 1,
                    pageSize = 1000,
                    showAll = true
                }, string.Empty);
                tlist = tmppage.List;
            }
            var tclassDAL = _provider.GetService<IotClassDAL>();
            var tproductDAL = _provider.GetService<IotProductDAL>();
            var tdevGroups = tlist.GroupBy(x => x.ProductId);
            foreach (var itemgroup in tdevGroups)
            {
                MZ_IotProduct product = await tproductDAL.SelectProductView(itemgroup.Key);
                if (!string.IsNullOrEmpty(product.Path))
                {
                    var tmpPathList = product.Path.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var tmpParentClassList = await tclassDAL.SelectList(x => tmpPathList.Contains(x.Id));
                    var classNames = tmpParentClassList.Select(x => x.Name);
                    foreach (var item in itemgroup)
                    {
                        List<string> tmplist = new List<string>();
                        tmplist.AddRange(classNames);
                        tmplist.AddRange(new JiebaSegmenter().CutForSearch(item.Name + " " + product.Name + " " + item.Remark));
                        item.KeyWords = string.Join(",", tmplist.Where(x => x != " " && x != ""));
                    }
                    await deviceDAL.UpdateKeywords(itemgroup.ToList());
                }
            }
            return tlist.Count == 1000;
        }

        public async Task UpdateDeviceNodeIdx()
        {
            var nodeList = _provider.GetService<ServerBusProxy>().GetNodeList();
            if (nodeList.Count > 1)
            {
                var curMax = nodeList.Count - 1;
                var maxIdx = await _deviceDAL.GetMaxUpIdx();
                if (maxIdx != curMax)
                {
                    IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
                    await redis.WaitWriteNodeLockAsync(async () =>
                    {
                        Dictionary<int, List<Out_SimDev>> dict = new Dictionary<int, List<Out_SimDev>>();
                        for (int i = 0; i <= curMax; i++)
                        {
                            dict[i] = new List<Out_SimDev>();
                        }
                        for (int j = 0; j <= maxIdx; j++)
                        {
                            List<Out_SimDev> iotDevList = await _deviceDAL.SelectDevListByIdx(j);
                            foreach (var iotDev in iotDevList)
                            {
                                int pos = 0;
                                if (!string.IsNullOrEmpty(iotDev.DeviceId))
                                {
                                    pos = Math.Abs(iotDev.DeviceId.GetHashCode() % nodeList.Count);
                                }
                                if (pos != iotDev.DeviceUpIdx)
                                {
                                    dict[pos].Add(iotDev);
                                }
                            }
                        }
                        for (int k = 0; k <= curMax; k++)
                        {
                            if (dict[k].Count > 0)
                            {
                                int pageSize = 2000;
                                int totalCount = dict[k].Count;
                                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                                for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                                {
                                    int startIndex = pageIndex * pageSize;
                                    int endIndex = Math.Min(startIndex + pageSize, totalCount);
                                    var currentPageData = dict[k].GetRange(startIndex, endIndex - startIndex);
                                    MZ_IotDevice newDev = new MZ_IotDevice();
                                    newDev.DeviceUpIdx = k;
                                    var ids = currentPageData.Select(x => x.Id).ToList();
                                    if (ids.Count > 0)
                                    {
                                        await _deviceDAL.Update(newDev, x => ids.Contains(x.Id));
                                    }
                                }
                            }
                        }

                    });

                }
            }

        }
    }
}
