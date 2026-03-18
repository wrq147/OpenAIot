using AfterService.DAL;
using AfterService.Model;
using AuthService;
using AuthService.Business;
using AuthService.DAL;
using AuthService.Model;
using ChannelUtility.Tsl;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AfterService.Business
{
    public class KFDeviceBLL
    {
        private ITAServiceProvider _provider;
        private KFDeviceDAL _deviceDAL;
        public KFDeviceBLL(KFDeviceDAL deviceDAL, ITAServiceProvider serviceProvider)
        {
            _deviceDAL = deviceDAL;
            _provider = serviceProvider;
        }
        public virtual async Task<PageObject<Out_KFProtocalName>> SelectKFProtocalList(In_KFProtocalPage query)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return new PageObject<Out_KFProtocalName>();
            }
            return await _deviceDAL.SelectKFProtocalList(user.OrgId, query);
        }
        public virtual async Task<PageObject<Out_KFProductName>> SelectKFProductList(In_KFProductPage query)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return new PageObject<Out_KFProductName>();
            }
            return await _deviceDAL.SelectKFProductList(user.OrgId, query);
        }
        public virtual async Task<Dictionary<string, int>> SelectKFDevAreaInfo(string parentCode, string dstates, IUserInfo user, DataScope scope)
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            if (parentCode == "100000")
            {
                var tInfo = await _deviceDAL.SelectKFDevAreaInfo(string.Empty, user, scope);
                ret.Add("onCount", tInfo.onCount);
                ret.Add("offCount", tInfo.offCount);
                ret.Add("unCount", tInfo.unCount);
                if (string.IsNullOrEmpty(dstates))
                {
                    return ret;
                }
                var dstatearr = dstates.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var ditem in dstatearr)
                {
                    var tmpcc = await _deviceDAL.SelectKFDevAreaDStateCount(string.Empty, ditem, user, scope);
                    ret.Add(ditem, tmpcc);
                }
                return ret;
            }
            else
            {
                var tmparea = await _provider.GetService<CodeDAL>().SelectArea(parentCode);
                if (tmparea != null)
                {
                    var tInfo = await _deviceDAL.SelectKFDevAreaInfo(tmparea.ParentPath, user, scope);
                    ret.Add("onCount", tInfo.onCount);
                    ret.Add("offCount", tInfo.offCount);
                    ret.Add("unCount", tInfo.unCount);
                    if (string.IsNullOrEmpty(dstates))
                    {
                        return ret;
                    }
                    var dstatearr = dstates.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var ditem in dstatearr)
                    {
                        var tmpcc = await _deviceDAL.SelectKFDevAreaDStateCount(string.Empty, ditem, user, scope);
                        ret.Add(ditem, tmpcc);
                    }
                    return ret;
                }
                else
                {
                    return null;
                }
            }

        }


        public virtual async Task<List<Out_DevAreaData>> SelectAreaDataList(string parentCode, IUserInfo user, DataScope scope)
        {
            List<Out_DevAreaData> tareaList;
            if (parentCode == "100000")
            {
                tareaList = await _deviceDAL.SelectKFDevAreaDataList(string.Empty, user, scope);
            }
            else
            {
                var tmparea = await _provider.GetService<CodeDAL>().SelectArea(parentCode);
                if (tmparea != null)
                {
                    tareaList = await _deviceDAL.SelectKFDevAreaDataList(tmparea.ParentPath, user, scope);
                }
                else
                {
                    tareaList = new List<Out_DevAreaData>();
                }
            }

            var tcodes = tareaList.Select(x => x.AreaCode).ToList();
            if (tcodes.Count > 0)
            {
                var tareas = await _provider.GetService<CodeDAL>().SelectCodeListByCode(tcodes);
                var tareaDict = tareas.ToDictionary(x => x.Id);
                foreach (var tarea in tareaList)
                {
                    if (tareaDict.TryGetValue(tarea.AreaCode, out var area))
                    {
                        tarea.AreaName = area.Name;
                        tarea.Lng = Convert.ToDouble(area.Lng);
                        tarea.Lat = Convert.ToDouble(area.Lat);
                    }
                }
            }
            return tareaList;
        }
        public virtual async Task<List<Out_DevAreaData>> SelectKFDevAreaDataListByRange(In_DevRangeAreaList query, IUserInfo user, DataScope scope)
        {
            var tareaList = await _deviceDAL.SelectKFDevAreaDataListByRange(query, user, scope);
            var tcodes = tareaList.Select(x => x.AreaCode).ToList();
            if (tcodes.Count > 0)
            {
                var tareas = await _provider.GetService<CodeDAL>().SelectCodeListByCode(tcodes);
                var tareaDict = tareas.ToDictionary(x => x.Id);
                foreach (var tarea in tareaList)
                {
                    if (tareaDict.TryGetValue(tarea.AreaCode, out var area))
                    {
                        tarea.AreaName = area.Name;
                        tarea.Lng = Convert.ToDouble(area.Lng);
                        tarea.Lat = Convert.ToDouble(area.Lat);
                    }
                }
            }
            return tareaList;
        }
        public virtual async Task<List<Out_KfDevice>> SelectKFDeviceListByRange(In_DevRangeList query, IUserInfo user, DataScope scope)
        {
            return await _deviceDAL.SelectKFDeviceListByRange(query, user, scope);
        }
        public virtual async Task<PageObject<Out_KfDevice>> ListPage(In_KFDevListPage query, IUserInfo user, DataScope scope)
        {
            var tpageList = await _deviceDAL.SelectWithGroupPage(query, user, scope);
            var ids = tpageList.List.Select(x => x.Id).ToList();
            if (ids.Count > 0)
            {
                var warnList = await _provider.GetService<IotWarningDAL>().SelectWarningDeviceList(ids, user.OrgId);
                foreach (var item in tpageList.List)
                {
                    item.HavWarn = warnList.Contains(item.Id);
                }
            }

            return tpageList;
        }
        public virtual async Task<BusResponse<int>> UpdateDevice(In_UpdateDevice data)
        {
            if (data.Name == "")
            {
                return BusResponse<int>.Error(110, "设备名称不能为空");
            }
            var user = _provider.GetUser();
            var device = await _deviceDAL.Select(data.Id);
            if (device == null)
            {
                return BusResponse<int>.Error(111, "设备不存在");
            }
            if (user.OrgId != device.OrgId && user.OrgId != device.OwnerOrgId && user.OrgId != device.UseOrgId && user.UserId != device.UseUserId)
            {
                return BusResponse<int>.Error(112, "无修改该设备的权限");
            }
            MZ_IotDevice updateDevice = new MZ_IotDevice();
            updateDevice.Id = data.Id;
            updateDevice.Name = data.Name;
            updateDevice.PhotoUrl = data.PhotoUrl;
            updateDevice.Remark = data.Remark;
            updateDevice.DState = data.DState;
            if (data.Lat != null && data.Lng != null)
            {
                updateDevice.Lat = data.Lat;
                updateDevice.Lng = data.Lng;
                updateDevice.GeoHash = MyAccess.Core.GeoHash.Encode(data.Lat.Value, data.Lng.Value);
                if (string.IsNullOrEmpty(updateDevice.AreaCode))
                {
                    var areaInfo = await _provider.GetService<CodeBLL>().SelectAreaByLatLng(data.Lng.Value, data.Lat.Value);
                    if (areaInfo != null)
                    {
                        updateDevice.AreaCode = areaInfo.Id;
                    }
                    else
                    {
                        updateDevice.AreaCode = string.Empty;
                    }
                }
            }
            return BusResponse<int>.Success(await _deviceDAL.Update(updateDevice));
        }

        public virtual async Task<BusResponse<int>> TransferTo(In_TransferTo data)
        {
            var roomDeviceDAL = _provider.GetService<RoomDeviceDAL>();
            if (await roomDeviceDAL.Some(x => x.Id == data.RoomId && x.TargetId == data.DeviceId))
            {
                return BusResponse<int>.Error(111, "设备已在房间中");
            }
            var user = _provider.GetUser();
            try
            {
                MZ_RoomDevice roomDevice = new MZ_RoomDevice();
                roomDevice.Id = data.RoomId;
                roomDevice.TargetId = data.DeviceId;
                roomDevice.OrgId = user.OrgId;
                int rs = await roomDeviceDAL.Insert(roomDevice);
                return BusResponse<int>.Success(rs);
            }
            catch
            {
                return BusResponse<int>.Error(122, "操作异常,请重新尝试");
            }
        }
        public virtual async Task<BusResponse<Out_DeviceOfRoom>> DevInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BusResponse<Out_DeviceOfRoom>(1, string.Empty, Out_DeviceOfRoom.DemoData());
            }

            var device = await _deviceDAL.Select(id);
            if (device == null)
            {
                return BusResponse<Out_DeviceOfRoom>.Error(111, "设备不存在");
            }
            Out_DeviceOfRoom devNR = new Out_DeviceOfRoom();
            devNR.Id = device.Id;
            devNR.Name = device.Name;

            var toutlist = await _provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoom(id);
            if (toutlist.Count > 0)
            {
                var tmplist = toutlist.Select(x => x.RoomName).ToList();
                devNR.RoomNames = string.Join(',', tmplist);
            }

            devNR.DeviceNumber = device.DeviceNumber;
            devNR.OwnerOrgId = device.OwnerOrgId;
            devNR.OrgId = device.OrgId;
            devNR.DeviceId = device.DeviceId;
            return BusResponse<Out_DeviceOfRoom>.Success(devNR);
        }

        public virtual async Task<List<Out_MyDevice>> SelectMyDevices(IUserInfo user)
        {
            var tmplist = await _deviceDAL.SelectMyDevices(user);
            var protocolIds = tmplist.Select(x => x.ProtocolId).ToArray();
            var protocolList = await _provider.GetService<IotProductDAL>().SelectProductTSL(protocolIds);
            Dictionary<string, List<DevFun>> funDict = new Dictionary<string, List<DevFun>>();
            Dictionary<string, List<DevProp>> propDict = new Dictionary<string, List<DevProp>>();
            foreach (var protocol in protocolList)
            {
                #region 定义协议功能
                var tsl = TslModel.CreateFrom(protocol.ModelTSL);
                List<DevFun> tmpfuns = new List<DevFun>();
                foreach (var funitem in tsl.functions)
                {
                    var tmpfunitem = new DevFun()
                    {
                        code = funitem.code,
                        name = funitem.name,
                        description = funitem.description
                    };
                    tmpfunitem.inputs = new List<DevFunParam>();
                    foreach (var funparam in funitem.inputs)
                    {
                        tmpfunitem.inputs.Add(new DevFunParam()
                        {
                            name = funparam.name,
                            code = funparam.code,
                            type = funparam.type,
                            remark = funparam.remark,
                            elements = funparam.elements
                        });
                    }
                    tmpfuns.Add(tmpfunitem);
                }
                funDict.Add(protocol.Id, tmpfuns);
                #endregion

                #region 定义协议属性
                List<DevProp> tmpprops = new List<DevProp>();
                foreach (var propitem in tsl.properties)
                {
                    tmpprops.Add(new DevProp()
                    {
                        name = propitem.name,
                        code = propitem.code,
                        description = propitem.description,
                        type = propitem.option.type
                    });
                }
                propDict.Add(protocol.Id, tmpprops);
                #endregion

            }

            foreach (var mydev in tmplist)
            {
                if (funDict.TryGetValue(mydev.ProtocolId, out var tmpff))
                {
                    mydev.Funs = tmpff;
                }
                if (propDict.TryGetValue(mydev.ProtocolId, out var tmppp))
                {
                    mydev.Props = tmppp;
                }
            }
            return tmplist;
        }
    }
}
