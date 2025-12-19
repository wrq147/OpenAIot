using ChannelUtility.Message;
using System;
namespace FixVideoChannel
{
    public interface IDeviceEventListener
    {

        /// <summary>
        /// 处理上线事件
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task OnEventOnline(VideoCaptureItem item);

        /// <summary>
        /// 处理离线事件
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task OnEventOffline(VideoCaptureItem item);

        /// <summary>
        /// AI检测请求
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="item"></param>
        /// <param name="rgbFrame"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        Task OnSendAIDetectRequest(string videoId, AIDetectItem item, byte[] rgbFrame, int width, int height);

        /// <summary>
        /// 处理平台下发消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task OnDeviceDownMessage(BaseDeviceMessage msg);
    }
}
