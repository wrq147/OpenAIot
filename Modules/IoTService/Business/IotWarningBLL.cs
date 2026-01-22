using AuthService;
using ChannelUtility;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using Common;
using Common.EventBus;
using Common.Json;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotWarningBLL
    {
        private ITAServiceProvider _provider;
        private IotWarningDAL _warningDAL;
        private UserDAL _userDAL;
        public IotWarningBLL(ITAServiceProvider provider, IotWarningDAL warningDAL, UserDAL userDAL)
        {
            _provider = provider;
            _warningDAL = warningDAL;
            _userDAL = userDAL;
        }
        public virtual async Task<string> GenerateWNNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("WN");
        }
        public virtual async Task<string> QueryFlowTemplateName(long id)
        {
            return await _warningDAL.SelectFlowTemplateName(id);
        }
        public virtual async Task<BusResponse<int>> QueryCount(IUserInfo user)
        {
            return BusResponse<int>.Success(await _warningDAL.SelectWaitCount(user));
        }

        public virtual async Task<BusResponse<MZ_IotWarning>> InfoByNumber(string number)
        {
            var infoList = await _warningDAL.SelectList(x => x.WarnNumber == number);
            if (infoList.Count == 0)
            {
                return BusResponse<MZ_IotWarning>.Error(4, "告警工单不存在");
            }
            return await QueryInfo(infoList[0]);
        }
        public virtual async Task<BusResponse<MZ_IotWarning>> Info(long id)
        {
            var info = await _warningDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_IotWarning>.Error(123, "告警工单不存在");
            }
            return await QueryInfo(info);
        }
        private async Task<BusResponse<MZ_IotWarning>> QueryInfo(MZ_IotWarning info)
        {
            var deviceDAL = _provider.GetService<IotDeviceDAL>();
            var productDAL = _provider.GetService<IotProductDAL>();

            var deviceInfo = await deviceDAL.Select(info.DeviceId);
            if (deviceInfo != null)
            {
                info.DeviceName = deviceInfo.Name;
                info.DevicePhotoUrl = deviceInfo.PhotoUrl;
                var productInfo = await productDAL.Select(deviceInfo.ProductId);
                if (productInfo != null)
                {
                    info.ProductName = productInfo.Name;
                }
            }
            return BusResponse<MZ_IotWarning>.Success(info);
        }
        public virtual async Task<PageObject<MZ_IotWarning>> ListPage(In_WarningListPage query, IUserInfo user)
        {
            var tlist = await _warningDAL.SelectWithPage(query, user);

            var clearUsers = await _userDAL.NavigateDict(tlist.List, x => x.ClearId != null && x.ClearId > 0, x => x.ClearId.Value);
            foreach (var warn in tlist.List)
            {
                if (warn.ClearId != null && warn.ClearId > 0)
                {
                    MZ_AdminInfo clearUser;
                    if (clearUsers.TryGetValue(warn.ClearId.Value, out clearUser))
                    {
                        warn.ClearUser = clearUser;
                    }
                }
            }

            return tlist;
        }


        /// <summary>
        /// 添加报警记录
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="rdmsg"></param>
        /// <param name="device"></param>
        /// <param name="extInfo"></param>
        /// <returns></returns>
        public virtual async Task<List<Out_WarnTargetId>> Insert(BaseEvent evt, DeviceEventMessage rdmsg, MZ_IotDevice device, string extInfo)
        {

            if (evt.Targets == null || evt.Targets.Length <= 0)
            {
                return new List<Out_WarnTargetId>();
            }
            List<Out_WarnTargetId> rtlist = new List<Out_WarnTargetId>();
            foreach (string tv in evt.Targets)
            {
                MZ_IotWarning warning = new MZ_IotWarning();
                warning.Code = evt.code;
                warning.Description = extInfo;
                warning.DeviceId = device.Id;
                warning.WarnNumber = await GenerateWNNumber();
                warning.Level = evt.Level;
                warning.MsgInfo = System.Text.Json.JsonSerializer.Serialize(rdmsg, JsonMessageSerializerConfig.DefaultOptions);
                warning.Name = evt.name;
                warning.ClearRemark = string.Empty;
                warning.Status = 0;
                warning.CreateOn = DateTime.Now;
                warning.OrgId = 0;
                warning.FlowId = 0;
                switch (tv)
                {
                    case "org":
                        warning.OrgId = device.OrgId.Value;
                        break;
                    case "own":
                        warning.OrgId = device.OwnerOrgId.Value;
                        break;
                    case "use":
                        warning.OrgId = device.UseOrgId.Value;
                        break;
                    default:
                        break;
                }
                if (warning.OrgId <= 0)
                {
                    continue;
                }

                Out_WarnTargetId targetrt = new Out_WarnTargetId();
                targetrt.OrgId = warning.OrgId.Value;
                targetrt.Id = await _warningDAL.InsertAndReturn(warning);
                rtlist.Add(targetrt);

                //告警目标有配置流程时，则创建流程
                var twarnconfig = await _provider.GetService<IotWarnConfigBLL>().WarnConfigInfoByPro(device.ProductId);
                if (twarnconfig != null && twarnconfig.WarnFlowId > 0)
                {
                    var users = await _provider.GetService<UserDAL>().SelectManUsers(warning.OrgId.Value);
                    var flowitems = System.Text.Json.JsonSerializer.Deserialize<List<FlowItem>>(twarnconfig.WarnFlowInitJson, MyDefaultTextJsonConfig.DefaultOptions);
                    FlowCreateData flowcreate = new FlowCreateData();
                    flowcreate.templateId = twarnconfig.WarnFlowId.Value;
                    flowcreate.model = new Dictionary<string, object>();
                    flowcreate.model.Add("@from", warning.WarnNumber);
                    flowcreate.model.Add("@fromtype", "告警工单");
                    flowcreate.model.Add("@FlowNumber", warning.WarnNumber);
                    flowcreate.UserId = users[0].Id.Value;
                    Dictionary<string, MZ_Org> tmpdict = new Dictionary<string, MZ_Org>();
                    foreach (var fitem in flowitems)
                    {
                        if (!flowcreate.model.ContainsKey(fitem.id))
                        {
                            flowcreate.model.Add(fitem.id, await fitem.GetRealValue(_provider, warning, device, tmpdict));
                        }
                    }

                    var rsp = await BusUtility.Call("NewFlowTask", flowcreate);
                    if (rsp.IsSuccess())
                    {
                        MZ_IotWarning upwarn = new MZ_IotWarning();
                        upwarn.Id = targetrt.Id;
                        upwarn.FlowId = rsp.GetResult<long>();
                        await _warningDAL.Update(upwarn);
                    }
                    else
                    {
                        var recvList = await _provider.GetService<UserDAL>().SelectManUsers(warning.OrgId.Value);
                        List<TargetUser> targets = new List<TargetUser>();
                        foreach (var recvId in recvList)
                        {
                            targets.Add(new TargetUser()
                            {
                                uid = recvId.Id.Value,
                                email = recvId.Email,
                                phone = recvId.Mobile
                            });
                        }
                        var nt = new NoticeEvent(2, targets.ToArray(), new string[] { "APP" });
                        nt.OrgId = warning.OrgId.Value;
                        nt.TargetType = "WarningFlow";
                        nt.TargetUrl = string.Empty;
                        nt.Content = $"配置的告警流程无法初始化,错误内容：{rsp.Message}";
                        nt.Label = "物联设备消息";
                        await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                    }
                }


            }
            return rtlist;
        }


        /// <summary>
        /// 标记报警已处理（批量）
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="code"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> ClearWarning(string devId, string code, string remark)
        {
            var redis = _provider.GetService<IotRedisHelper>();
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            MZ_IotWarning data = new MZ_IotWarning();
            data.Status = 1;
            data.ClearId = user.UserId;
            data.ClearOn = DateTime.Now;
            data.ClearRemark = remark;
            if (string.IsNullOrEmpty(devId) && string.IsNullOrEmpty(code))
            {
                var twarnlist = await _warningDAL.SelectWarnDevList(user.OrgId, 0);
                foreach (var warnItem in twarnlist)
                {
                    await redis.KeyDeleteAsync("IotQuick::" + warnItem.DeviceId + "::" + warnItem.Code);
                }

                return BusResponse<int>.Success(await _warningDAL.Update(data, x => x.OrgId == user.OrgId && x.Status == 0));
            }
            else if (string.IsNullOrEmpty(devId))
            {
                var twarnlist = await _warningDAL.SelectWarnDevList(user.OrgId, 0, code);
                foreach (var warnItem in twarnlist)
                {
                    await redis.KeyDeleteAsync("IotQuick::" + warnItem.DeviceId + "::" + warnItem.Code);
                }

                return BusResponse<int>.Success(await _warningDAL.Update(data, x => x.OrgId == user.OrgId && x.Status == 0 && x.Code == code));
            }
            else if (string.IsNullOrEmpty(code))
            {
                var twarnlist = await _warningDAL.SelectWarnDevListById(user.OrgId, 0, devId);
                foreach (var warnItem in twarnlist)
                {
                    await redis.KeyDeleteAsync("IotQuick::" + warnItem.DeviceId + "::" + warnItem.Code);
                }
                return BusResponse<int>.Success(await _warningDAL.Update(data, x => x.OrgId == user.OrgId && x.Status == 0 && x.DeviceId == devId));
            }
            else
            {
                var twarnlist = await _warningDAL.SelectWarnDevListByAll(user.OrgId, 0, devId, code);
                foreach (var warnItem in twarnlist)
                {
                    await redis.KeyDeleteAsync("IotQuick::" + warnItem.DeviceId + "::" + warnItem.Code);
                }
                return BusResponse<int>.Success(await _warningDAL.Update(data, x => x.OrgId == user.OrgId && x.Status == 0 && x.DeviceId == devId && x.Code == code));
            }
        }


        /// <summary>
        /// 标记报警已处理（单个）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remark"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> ClearWarning(long id, string remark, IUserInfo user)
        {
            var warnItem = await _warningDAL.Select(id);
            if (warnItem == null)
            {
                return BusResponse<int>.Error(111, "报警不存在");
            }
            if (warnItem.Status != 0)
            {
                return BusResponse<int>.Error(112, "报警已被处理");
            }

            //清除沉默周期
            var device = await _provider.GetService<IotDeviceDAL>().Select(warnItem.DeviceId);
            if (device != null)
            {
                var redis = _provider.GetService<IotRedisHelper>();
                await redis.KeyDeleteAsync("IotQuick::" + device.DeviceId + "::" + warnItem.Code);
            }


            //标记已处理
            MZ_IotWarning data = new MZ_IotWarning();
            data.Id = id;
            data.Status = 1;
            data.ClearId = user.UserId;
            data.ClearOn = DateTime.Now;
            data.ClearRemark = remark;
            return BusResponse<int>.Success(await _warningDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> ClearAllWarning(string remark, IUserInfo user)
        {
            var twarnlist = await _warningDAL.SelectWarnDevList(user.OrgId, 0);
            var redis = _provider.GetService<IotRedisHelper>();
            foreach (var warnItem in twarnlist)
            {
                await redis.KeyDeleteAsync("IotQuick::" + warnItem.DeviceId + "::" + warnItem.Code);
            }

            MZ_IotWarning data = new MZ_IotWarning();
            data.Status = 1;
            data.ClearId = user.UserId;
            data.ClearOn = DateTime.Now;
            data.ClearRemark = remark;
            return BusResponse<int>.Success(await _warningDAL.Update(data, x => x.OrgId == user.OrgId && x.Status == 0));
        }
        /// <summary>
        /// 清除超3个月的过期报警
        /// </summary>
        /// <returns></returns>
        public virtual async Task ClearOverWarning()
        {
            await _warningDAL.Delete(x => x.Status == 1 && x.CreateOn < DateTime.Now.AddMonths(3));
        }


        public virtual async Task<Dictionary<string, int>> GetWarningCountByCode(string[] codes, IUserInfo user)
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            foreach (var c in codes)
            {
                var tmpcount = await _warningDAL.Count(x => x.OrgId == user.OrgId && x.Code == c);
                ret.Add(c, tmpcount);
            }
            return ret;
        }
    }
}
