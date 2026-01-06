using System;
using GB28181.App;

namespace GB28181.Config
{
    public class SipStorage : ISipStorage
    {
        private SIPAccount _account;
        public static event GBServerConfigDelegate GBServerConfigReceived;

        public SipStorage() { }
        public SIPAccount GetLocalSipAccout()
        {
            if (_account == null)
            {
                if (GBServerConfigReceived != null)
                {
                    _account = GBServerConfigReceived.Invoke();
                }
                else
                {
                    throw new ApplicationException("Accounts is NULL,SIP not started");
                }
            }
            return _account;
        }
    }
}
