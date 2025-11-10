using Common.Share;
using System;

namespace AuthService
{
    public class In_LoginLogList : BaseQueryParam
    {
        public string ipaddr { get; set; }
        public byte? status { get; set; }
        public string userName { get; set; }
    }
}
