using AuthService;
using Common;
using Common.Share;
using MessageService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService.DAL
{
    public class MessageDAL : BaseRepository<MZ_Message>
    {
        public virtual async Task<int> UnreadCount(Data_ServerTokenInfo user)
        {
            var sql = new SqlBuilder(help).Append("select count(1) from mz_message m left join mz_message_log l on m.id=l.messsage_id and l.receiver_id=" + user.UserId + " where l.status=0 and (m.OrgId=0 or m.OrgId=" + user.OrgId + ")");
            return (await sql.DoAsync<DoQueryScalar>()).GetValueInt();
        }
        public virtual async Task<List<MZ_Message>> SelectUnread(Data_ServerTokenInfo user, int top)
        {
            var sql = new SqlBuilder(help).Query<MZ_Message>().Append("select m.* from mz_message_log l left join mz_message m on m.id=l.messsage_id and l.receiver_id=" + user.UserId + " where l.status=0  and (m.OrgId=0 or m.OrgId=" + user.OrgId + ") order by m.create_time desc").Take(top);
            return await sql.ToListAsync();
        }
        public virtual async Task<PageObject<MZ_Message>> SelectPage(In_MessageList query, Data_ServerTokenInfo user)
        {
            return await new SqlBuilder(help).Query<MZ_Message>().Append("select m.*,l.status from mz_message_log l left join mz_message m on m.id=l.messsage_id and l.receiver_id=" + user.UserId + " where  (m.OrgId=0 or m.OrgId=" + user.OrgId + ")")
                .Then(query.status != null && query.status > -1, sql =>
                {
                    if (query.status == 0)
                    {
                        sql.Append(" AND l.status=0");
                    }
                    else if (query.status == 1)
                    {
                        sql.Append(" AND l.status=1");
                    }
                })
                .Then(query.t != null && query.t > -1, sql =>
                {
                    sql.Append(" AND m.click_type=").AppendParam(query.t);
                }).GeneratePageObjectAsync(query, "m.create_time desc");
        }
    }
}
