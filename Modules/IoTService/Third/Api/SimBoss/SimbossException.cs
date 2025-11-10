using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Third.Api.SimBoss
{
    public class SimbossException : System.Exception
    {
        public SimbossException() : this(string.Empty) { }

        public SimbossException(string message) : this(message, null) { }

        public SimbossException(string message, System.Exception cause) : base(message, cause) { }
    }
}
