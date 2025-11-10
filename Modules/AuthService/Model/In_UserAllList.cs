using Common.Share;
using System;

namespace AuthService.Model
{
    public class In_UserAllList : BaseQueryParam
    {
        public string status { get; set; }
        public string key { get; set; }
    }
}
