using System;

namespace AuthService.Model
{
    /// <summary>
    /// 用户的组织信息
    /// </summary>
    public class MZ_User_Org_V : MZ_Org
    {
        public long? UserId { get; set; }
        public long? dept_id { get; set; }
        public string dept_name { get; set; }
        public string post_name { get; set; }
    }
}
