using Common.Share;
using AfterService.DAL;
using AfterService.Model;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.Extensions.DependencyInjection;
using TemplateAction.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AfterService.Business
{
    public class RoomDeviceBLL
    {
        private RoomDeviceDAL _roomDeviceDAL;
        private ITAServiceProvider _serviceProvider;
        public RoomDeviceBLL(ITAServiceProvider provider, RoomDeviceDAL roomDeviceDAL)
        {
            _serviceProvider = provider;
            _roomDeviceDAL = roomDeviceDAL;
        }
        public virtual async Task<BusResponse<string>> Add(List<In_RoomDevice> data)
        {
            var roomIds = data.Select(x => x.RoomId).ToList();
            var rooms = await _serviceProvider.GetService<RoomDAL>().SelectList(x => roomIds.Contains(x.Id));
            if (rooms.Count <= 0)
            {
                return BusResponse<string>.Error(111, "房间不存在");
            }

            var orgIds = rooms.Select(x => x.OrgId.Value).Distinct().ToList();
            if (orgIds.Count > 1)
            {
                return BusResponse<string>.Error(112, "房间所属组织异常");
            }

            List<MZ_RoomDevice> roomDevices = new List<MZ_RoomDevice>();
            foreach (var r in data)
            {
                if (await _roomDeviceDAL.Some(x => x.Id == r.RoomId && x.TargetId == r.DeviceId))
                {
                    MZ_IotDevice dev = await _serviceProvider.GetService<IotDeviceDAL>().Select(r.DeviceId);
                    return BusResponse<string>.Error(114, $"设备{dev.Name}无法重复分配");
                }
                MZ_RoomDevice roomDevice = new MZ_RoomDevice();
                roomDevice.Id = r.RoomId;
                roomDevice.TargetId = r.DeviceId;
                roomDevice.OrgId = orgIds[0];
                roomDevices.Add(roomDevice);
            }

            try
            {
                await _roomDeviceDAL.Insert(roomDevices);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(112, ex.Message);
            }

        }


        public virtual async Task<BusResponse<string>> Delete(List<In_RoomDevice> data)
        {
            try
            {
                foreach (var item in data)
                {
                    await _roomDeviceDAL.Delete(x => x.Id == item.RoomId && x.TargetId == item.DeviceId);
                }
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<int>> ClearJunk(long orgId)
        {
            return BusResponse<int>.Success(await _roomDeviceDAL.ClearJunk(orgId));
        }
    }
}
