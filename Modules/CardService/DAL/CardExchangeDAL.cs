using CardService.Model;
using Common;
using Common.Share;
using MyAccess.Aop;
using MyAccess.DB;
using System;
using System.Threading.Tasks;

namespace CardService.DAL
{
    public class CardExchangeDAL : BaseDbSupport
    {
        public virtual async Task<MZ_CardExchange> SelectById(long id)
        {
            var docmd = await new SqlBuilder(help).Append("select * from mz_card_exchange where Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_CardExchange>>();
            return docmd.ToFirst();
        }
        public async Task<bool> ExistExchanging(long recUserId, long sendCardId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select count(1) from mz_card_exchange where ReceiveUserId=").AppendParam(recUserId).Append(" and SendCardId=").AppendParam(sendCardId).Append(" and Status='0' and create_time>").AppendParam(DateTime.Now.AddDays(-3)).Append(" limit 1")
                   .DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
            }
        }
        public async Task<PageObject<MZ_CardExchange>> SelectList(In_CardExchange query)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_CardExchange>().Append("select e.*,sc.RealName as SendRealName,sc.Avatar as SendAvatar,sc.PostName as SendPostName,sc.OrgName as SendOrgName,rc.RealName as RecvRealName,rc.OrgName as RecvOrgName from mz_card_exchange e left join mz_card_v sc on e.SendCardId=sc.Id left join mz_card_v rc on e.ReceiveCardId=rc.Id where 1=1")
                    .Then(query.beginTime != null, sql =>
                    {
                        sql.Append(" and e.create_time >= ").AppendParam(query.beginTime);
                    })
                    .Then(query.endTime != null, sql=> {
                        sql.Append(" and e.create_time <= ").AppendParam(query.endTime);
                    })
                    .Then(query.ReceiveUserId!=null,sql=> {
                        sql.Append(" and e.ReceiveUserId = ").AppendParam(query.ReceiveUserId);
                    })
                    .GeneratePageObjectAsync(query, "e.create_time desc");
            }
        }

        public async Task<int> SelectPendingCount(long recvId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select count(1) from mz_card_exchange where ReceiveUserId=").AppendParam(recvId).Append(" and Status='0'")
                         .DoAsync<DoQueryScalar>()).GetValueInt(0);
            }
        }

        public async Task<int> Insert(MZ_CardExchange exchange)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(exchange).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<int> Update(MZ_CardExchange exchange)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Update(exchange)
                         .DoAsync<DoExecSql>()).RowCount;
            }
        }
        [Trans]
        public virtual async Task<int> UpdateTrans(MZ_CardExchange exchange)
        {
            return (await new SqlBuilder(help).Update(exchange)
                     .DoAsync<DoExecSql>()).RowCount;
        }
    }
}
