using ChannelUtility;
using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Enum;
using GB28181Channel.GB28181.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GB28181Channel
{
    public class InMemoryDeviceStorage : IDeviceStorage
    {
        private readonly ConcurrentDictionary<string, DeviceInfo> _devices = new ConcurrentDictionary<string, DeviceInfo>();
        private readonly ConcurrentDictionary<string, List<ChannelInfo>> _channels = new ConcurrentDictionary<string, List<ChannelInfo>>();
        private IServiceProvider _serviceProvider;
        public InMemoryDeviceStorage(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<string> GetDevicePassword(string deviceId)
        {
            var option = _serviceProvider.GetService<IOptions<GB28181Option>>().Value;
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            return await eventBus.WaitPublishMediaUserVerify(option.sip_service_id, deviceId);
        }
        public bool SaveDevice(DeviceInfo device)
        {
            if (device == null || string.IsNullOrEmpty(device.DeviceId))
                return false;

            // 使用AddOrUpdate原子操作：不存在则添加，存在则更新
            _devices.AddOrUpdate(
                device.DeviceId,
                addValueFactory: _ => device,
                updateValueFactory: (_, __) => device);

            return true;
        }

        public bool RemoveDevice(string deviceId)
        {
            _channels.TryRemove(deviceId, out var channel);
            return _devices.TryRemove(deviceId, out var currentDevice);
        }

        public bool UpdateDeviceMediaInfo(string deviceId, string dtuId, string pushKey)
        {
            if (!_devices.TryGetValue(deviceId, out var currentDevice))
                return false;

            var updatedDevice = currentDevice.Clone();
            updatedDevice.DtuId = dtuId;
            updatedDevice.PushKey = pushKey;
            if (_devices.TryUpdate(deviceId, updatedDevice, currentDevice))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DeviceInfo GetDevice(string deviceId)
        {
            // 直接使用TryGetValue，无需锁
            _devices.TryGetValue(deviceId, out var device);
            return device;
        }

        public List<DeviceInfo> GetAllDevices()
        {
            return _devices.Values.ToList();
        }

        public async Task<bool> SaveChannels(string deviceId, List<ChannelInfo> channels)
        {
            if (channels == null || channels.Count == 0)
                return false;

            _channels.AddOrUpdate(
                deviceId,
                addValueFactory: _ => channels,
                updateValueFactory: (_, existingChannels) => channels);


            var option = _serviceProvider.GetService<IOptions<GB28181Option>>().Value;
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            var channelIds = channels.Select(x => x.ChannelId).ToList();
            var channelNames = channels.Select(x => x.ChannelName).ToList();
            eventBus.PublishMediaChannels(option.sip_service_id, deviceId, channelIds, channelNames);
            return true;
        }

        public List<ChannelInfo> GetChannelsByDeviceId(string deviceId)
        {
            // 获取并返回拷贝，避免外部修改内部集合
            if (_channels.TryGetValue(deviceId, out var channels))
                return new List<ChannelInfo>(channels);

            return new List<ChannelInfo>();
        }

    }

}
