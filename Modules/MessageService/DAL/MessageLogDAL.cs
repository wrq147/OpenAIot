using AuthService;
using Common;
using MessageService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.DAL
{
    public class MessageLogDAL : BaseRepository<MZ_Message_Log>
    {
        public virtual async Task<int> ReadAll(Data_ServerTokenInfo user)
        {
            var sql = new SqlBuilder(help).Append("update mz_message_log set status=1,read_time='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where receiver_id=" + user.UserId + " and status=0 and EXISTS(select Id from mz_message where mz_message_log.messsage_id=id and (OrgId=0 or OrgId=" + user.OrgId + "))");
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }
    }
}
