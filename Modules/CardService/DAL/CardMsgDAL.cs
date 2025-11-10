using CardService.Model;
using Common;
using Common.Share;
using MyAccess.DB;
using System;
using System.Threading.Tasks;

namespace CardService.DAL
{
    /// <summary>
    /// 
    /// </summary>
    public class CardMsgDAL : BaseDbSupport
    {
        public async Task<PageObject<MZ_Card_Record_V>> SelectList(In_Record_List query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<MZ_Card_Record_V>().Append("select r.*,c.Avatar,c.RealName,c.PostName,c.OrgName from mz_card_msg r left join mz_card_v c on r.VisitType=0 and r.TargetId=c.Id where r.VisitType=0 and NOT EXISTS (select Id from mz_card_msg where TargetId=r.TargetId and Id>r.Id limit 1)")
             .Then(query.VisitCardId != null, sq => sq.Append(" AND r.VisitCardId=").AppendParam(query.VisitCardId))
             .Then(query.beginTime != null, sq => sq.Append(" and r.CreatedOn >= ").AppendParam(query.beginTime))
             .Then(query.endTime != null, sq => sq.Append(" and r.CreatedOn <= ").AppendParam(query.endTime))
             .Append(" order by r.CreatedOn desc");
                return await tsql.GeneratePageObjectAsync(query);
            }

        }

        public async Task<PageObject<MZ_Card_Visited_V>> SelectVisitedList(In_Visited_List query)
        {
            using (DbHelp db = CreateDB())
            {
                string lastFilter = " 1=1 ";
                if (query.LastVisited)
                {
                    lastFilter = "NOT EXISTS (select Id from mz_card_msg where VisitCardId=r.VisitCardId and Id>r.Id limit 1)";
                }
                var tsql = new SqlBuilder(db).Query<MZ_Card_Visited_V>().Append("select r.*,c.Avatar,c.RealName,c.PostName,c.OrgName from mz_card_msg r left join mz_card_v c on r.VisitCardId=c.Id where " + lastFilter)
             .Then(query.VisitType != null, sq => sq.Append(" AND r.VisitType=").AppendParam(query.VisitType))
             .Then(query.TargetId != null, sq => sq.Append(" AND r.TargetId=").AppendParam(query.TargetId))
             .Then(query.ReceiveUserId != null, sq => sq.Append(" AND r.ReceiveUserId=").AppendParam(query.ReceiveUserId))
             .Then(query.beginTime != null, sq => sq.Append(" and r.CreatedOn >= ").AppendParam(query.beginTime))
             .Then(query.endTime != null, sq => sq.Append(" and r.CreatedOn <= ").AppendParam(query.endTime))
             .Append(" order by r.CreatedOn desc");
                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        public async Task<MZ_Card_Msg> SelectById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_card_msg where Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Card_Msg>>();
                return docmd.ToFirst();
            }
        }
        public async Task<int> Insert(MZ_Card_Msg data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<int> CountOfVisit(long cardId, long targetId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select count(1) from mz_card_msg where VisitCardId=").AppendParam(cardId).Append(" and TargetId=").AppendParam(targetId)
                         .DoAsync<DoQueryScalar>()).GetValueInt(0);
            }
        }
        public async Task<int> Update(MZ_Card_Msg data)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Update(data)
                         .DoAsync<DoExecSql>()).RowCount;
            }
        }
        public async Task<int> ReadAll(long uid)
        {
            using (DbHelp db = CreateDB())
            {
                MZ_Card_Msg cm = new MZ_Card_Msg();
                cm.Status = "1";
                cm.ReadedOn = DateTime.Now;
                return (await new SqlBuilder(db).Update(cm, "ReceiveUserId=").AppendParam(uid)
                         .DoAsync<DoExecSql>()).RowCount;
            }
        }
        public async Task<int> UnreadAmount(long uid)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_Card_Msg>().CountAsync(x => x.ReceiveUserId == uid && x.Status == "0");
            }
        }
    }
}
