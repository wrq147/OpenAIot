using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService
{
    public class AuthServerResult
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data_ServerTokenInfo data { get; set; }
    }
}
