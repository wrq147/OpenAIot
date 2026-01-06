using System.Collections.Generic;
using GB28181.App;

namespace GB28181
{
    public delegate void DmsRegisterDelegate(SIPTransaction sipTransaction, SIPAccount sIPAccount);
    public delegate SIPAccount GBServerConfigDelegate();
    public delegate void DeviceAlarmSubscribeDelegate(SIPTransaction sipTransaction);
}
