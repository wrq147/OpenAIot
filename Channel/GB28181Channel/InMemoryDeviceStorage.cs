using ChannelUtility;
using ChannelUtility.Message;
using GB28181Channel.GB28181;
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
using System.Threading.Channels;
using System.Threading.Tasks;
namespace GB28181Channel
{
    public class InMemoryDeviceStorage : IDeviceStorage
    {
        private readonly ConcurrentDictionary<string, string> _keyToDeviceIds = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, DeviceInfo> _devices = new ConcurrentDictionary<string, DeviceInfo>();
        private readonly ConcurrentDictionary<string, List<ChannelInfo>> _channels = new ConcurrentDictionary<string, List<ChannelInfo>>();

        private IServiceProvider _serviceProvider;
        public InMemoryDeviceStorage(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ChannelInfo GetChannelFrom(string streamId)
        {
            int tsidx = streamId.IndexOf('_');
            if (tsidx != -1)
            {
                string deviceId = streamId.Substring(0, tsidx);
                var channels = GetChannelsByDeviceId(deviceId);
                if (channels.Count == 0)
                {
                    return null;
                }
                int idx = int.Parse(streamId.Substring(tsidx + 1));
                return channels.Where(x => x.Index == idx).FirstOrDefault();
            }
            else
            {
                string deviceId = GetDeviceIdFrom(streamId);
                var channels = GetChannelsByDeviceId(deviceId);
                if (channels.Count == 0)
                {
                    return null;
                }
                return channels.Where(x => x.Index == 0).FirstOrDefault();
            }
        }
        public string GetDeviceIdFrom(string pushKey)
        {
            if (_keyToDeviceIds.TryGetValue(pushKey, out var deviceId))
            {
                return deviceId;
            }
            else
            {
                return null;
            }
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
            if (_channels.TryRemove(deviceId, out var channels))
            {
                foreach (var ch in channels)
                {
                    if (!string.IsNullOrEmpty(ch.Ssrc))
                    {
                        GB28181Util.ReleaseSsrc(ch.Ssrc);
                    }
                }
            }
            if (_devices.TryRemove(deviceId, out var currentDevice))
            {
                _keyToDeviceIds.TryRemove(currentDevice.VideoData.Item.PushKey, out string tdvid);
            }
            return true;
        }

        public bool UpdateDeviceMediaInfo(string deviceId, VideoData data)
        {
            if (!_devices.TryGetValue(deviceId, out var currentDevice))
                return false;

            var updatedDevice = currentDevice.Clone();
            updatedDevice.VideoData = data;
            if (_devices.TryUpdate(deviceId, updatedDevice, currentDevice))
            {
                _keyToDeviceIds.AddOrUpdate(data.Item.PushKey, _ => deviceId, (_, existingChannels) => deviceId);
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
            List<ChannelData> dataList = new List<ChannelData>();
            foreach (var channel in channels)
            {
                dataList.Add(new ChannelData()
                {
                    Index = channel.Index,
                    ChannelId = channel.ChannelId,
                    Name = channel.ChannelName
                });
            }
            var channelIds = channels.Select(x => x.ChannelId).ToList();
            var channelNames = channels.Select(x => x.ChannelName).ToList();
            eventBus.PublishMediaChannels(option.sip_service_id, deviceId, dataList);
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
