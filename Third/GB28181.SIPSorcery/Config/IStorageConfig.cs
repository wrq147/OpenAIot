using System.Collections.Generic;
using GB28181.App;

namespace GB28181.Config
{
    public interface ISipStorage
    {
        SIPAccount GetLocalSipAccout();
    }
}
