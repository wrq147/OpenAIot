using System;
namespace Common.Share
{
    public interface IUserInfo
    {
        long OrgId { get; set; }
        long UserId { get; set; }
    }
}
