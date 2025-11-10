using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Model
{
    public class PushAuthResponse
    {
        public int code { get; set; }
        public string msg { get; set; }
        public TokenData data { get; set; }
    }
    public class TokenData
    {
        public string expire_time { get; set; }
        public string token { get; set; }
    }
}
