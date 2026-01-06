using GB28181.App;
using SIPSorcery.SIP;

namespace GB28181.Servers
{
    public interface ISIPRegistrarCore
    {
        void SetAuthenticateRequestDelegate(SIPAuthenticateRequestDelegate ac);
        void ProcessRegisterRequest();

        void AddRegisterRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest registerRequest);

        /// <summary>
        /// 设备注册到DMS
        /// </summary>
        event DmsRegisterDelegate DmsRegisterReceived;

        event DeviceAlarmSubscribeDelegate DeviceAlarmSubscribe;
    }
}
