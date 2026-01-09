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
        /// 移除设备信息
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>是否成功</returns>
        bool RemoveDevice(string deviceId);
        /// <summary>
        /// 更新设备的媒体信息
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="dtuId"></param>
        /// <param name="pushKey"></param>
        /// <returns></returns>
        bool UpdateDeviceMediaInfo(string deviceId, string dtuId, string pushKey);
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
        /// <param name="deviceId">设备ID</param>
        /// <param name="channels">通道列表</param>
        /// <returns>是否成功</returns>
        Task<bool> SaveChannels(string deviceId, List<ChannelInfo> channels);

        /// <summary>
        /// 获取设备的通道列表
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>通道列表</returns>
        List<ChannelInfo> GetChannelsByDeviceId(string deviceId);
    }
}
