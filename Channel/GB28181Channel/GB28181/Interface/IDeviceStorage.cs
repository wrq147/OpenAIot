using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Interface
{
    /// <summary>
    /// 设备存储接口
    /// </summary>
    public interface IDeviceStorage
    {
        /// <summary>
        /// 获取设备密码
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>设备密码</returns>
        Task<string> GetDevicePassword(string deviceId);
        /// <summary>
        /// 保存设备信息
        /// </summary>
        /// <param name="device">设备信息</param>
        /// <returns>是否成功</returns>
        bool SaveDevice(DeviceInfo device);
        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="status">新状态</param>
        /// <param name="lastHeartbeatTime">最后心跳时间</param>
        /// <returns>是否成功</returns>
        bool UpdateDeviceStatus(string deviceId, DeviceStatus status, DateTime lastHeartbeatTime);

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>设备信息</returns>
        DeviceInfo GetDevice(string deviceId);

        /// <summary>
        /// 获取所有设备
        /// </summary>
        /// <returns>设备列表</returns>
        List<DeviceInfo> GetAllDevices();

        /// <summary>
        /// 保存通道信息
        /// </summary>
        /// <param name="channels">通道列表</param>
        /// <returns>是否成功</returns>
        bool SaveChannels(List<ChannelInfo> channels);

        /// <summary>
        /// 获取设备的通道列表
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>通道列表</returns>
        List<ChannelInfo> GetChannelsByDeviceId(string deviceId);
    }
}
