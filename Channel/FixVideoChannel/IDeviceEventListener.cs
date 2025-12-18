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
        /// <param name="item"></param>
        /// <param name="detectType"></param>
        /// <param name="detectParams"></param>
        /// <param name="rgbFrame"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        Task OnSendAIDetectRequest(VideoCaptureItem item, string detectType, Dictionary<string, string> detectParams, byte[] rgbFrame, int width, int height);

        /// <summary>
        /// 处理平台下发消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task OnDeviceDownMessage(RequestMessage msg);
    }
}
