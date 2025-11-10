using Common.Share;

namespace DeveloperService
{
    public class DeveloperUserInfo : IUserInfo
    {
        private long _uid;
        private long _orgId;
        public DeveloperUserInfo(long uid, long orgId)
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
