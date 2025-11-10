using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Share
{
    /// <summary>
    /// 虚假用户
    /// </summary>
    public class ArtificialUser : IUserInfo
    {
        private long _uid;
        private long _orgId;
        public ArtificialUser(long uid, long orgId)
        {
            _uid = uid;
            _orgId = orgId;
        }
        public long OrgId
        {
            get { return _orgId; }
            set { _orgId = value; }
        }
        public long UserId
        {
            get { return _uid; }
            set { _uid = value; }
        }
    }
}
