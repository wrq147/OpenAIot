using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Third.Api.SimBoss
{
    public interface SimbossRequest
    {
        String GetUri();

        SortedDictionary<String, String> GetParam();
    }
}
