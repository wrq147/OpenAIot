using AuthService;
using Common;
using Common.Share;
using AfterService.Model;
using MyAccess.DB;
using System;
using System.Threading.Tasks;

namespace AfterService.DAL
{
    public class RoomDAL : BaseRepository<MZ_Room>
    {
        public virtual async Task<MZ_Room> GetRoomInfo(string id)
        {
            return await new SqlBuilder(help).Query<MZ_Room>().Include(x => x.OrgInfo, x => x.OrgId).Include(x => x.TargetOrgInfo, x => x.TargetOrgId).Include(x => x.LeaderInfo, x => x.LeaderId).Where(x => x.Id == id).ToFirstAsync();
        }
        public virtual async Task<string> GetRoomCustomerName(string customerId)
        {
            return (await new SqlBuilder(help).Append("select CustomerName from mz_room where Id=").AppendParam(customerId).DoAsync<DoQuerySql<string>>()).ToFirst();
        }
    }
}
