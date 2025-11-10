using CardService.Model;
using Common;
using Common.Share;
using MyAccess.Aop;
using MyAccess.DB;
using System.Threading.Tasks;
namespace CardService.DAL
{
    /// <summary>
    /// 通讯录操作类
    /// </summary>
    public class CardHolderDAL : BaseDbSupport
    {
        public async Task<PageObject<MZ_Card_Holder_V>> SelectList(In_Card_Holder query)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_Card_Holder_V>().Append("select h.CreatedOn,c.* from mz_card_holder h left join mz_card_v c on h.CardId = c.Id where 1=1 ")
                   .Then(query.UserId != null, sql =>
                   {
                       sql.Append(" AND h.UserId=").AppendParam(query.UserId);
                   })
                   .Then(!string.IsNullOrEmpty(query.Key), sql =>
                   {
                       sql.Append(" AND c.RealName like concat('%',").AppendParam(query.Key).Append(", '%')");
                   })
                   .Then(query.beginTime != null, sql =>
                   {
                       sql.Append(" and h.CreatedOn >= ").AppendParam(query.beginTime);
                   })
                   .Then(query.endTime != null, sql=> {
                       sql.Append(" and h.CreatedOn <= ").AppendParam(query.endTime);
                   })
                   .GeneratePageObjectAsync(query, "h.CreatedOn desc");
            }
        }
        public async Task<bool> Exist(long uid, long cardId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select count(1) from mz_card_holder where UserId=").AppendParam(uid).Append(" and CardId=").AppendParam(cardId).Append(" limit 1")
                         .DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
            }
        }
        public async Task<int> Insert(MZ_CardHolder data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        [Trans]
        public virtual async Task<int> InsertTrans(MZ_CardHolder data)
        {
            await new SqlBuilder(help).Delete<MZ_CardHolder>("UserId=").AppendParam(data.UserId).Append(" and CardId=").AppendParam(data.CardId).DoAsync<DoExecSql>();
            var docmd = await new SqlBuilder(help).Insert(data).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        public async Task<int> Delete(long uid, long cardId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Delete<MZ_CardHolder>("UserId=").AppendParam(uid).Append(" and CardId=").AppendParam(cardId).DoAsync<DoExecSql>()).RowCount;
            }
        }

    }
}
