using Common.Share;
using System;

namespace AuthService.Model
{
    public class In_StyleList : BaseQueryParam
    {
        public string Name { get; set; }
        public long? OrgId { get; set; }
    }
}
