using CardService.Model;
using Common;
using Common.Share;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardService.DAL
{
    public class CardCaseDAL : BaseDbSupport
    {
        public async Task<MZ_Card_Case> SelectById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_card_case where Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Card_Case>>();
                return docmd.ToFirst();
            }
        }
        public async Task<PageObject<MZ_Card_Case>> SelectList(In_Card_Case query)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_Card_Case>().Append("select * from mz_card_case where 1=1 ")
                .Then(query.OrgId != null, sql =>
                 {
                     sql.Append(" and OrgId=").AppendParam(query.OrgId);
                 })
                .Then(query.beginTime != null, sql =>
                {
                    sql.Append(" and create_time >= ").AppendParam(query.beginTime);
                })
                .Then(query.endTime != null, sql =>
                {
                    sql.Append(" and create_time <= ").AppendParam(query.endTime);
                })
                .GeneratePageObjectAsync(query, "update_time desc");
            }
        }
        public async Task<PageObject<MZ_Card_Case>> SelectListM(In_Card_Case_M query)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_Card_Case>().Append("select * from mz_card_case where 1=1 ")
                .Then(query.OrgId != null, sq => sq.Append(" and OrgId=").AppendParam(query.OrgId))
                .Then(!string.IsNullOrEmpty(query.key), sq => sq.Append(" and Title like ").AppendParam('%' + query.key + '%'))
                .Then(query.beginTime != null, sq => sq.Append(" and create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and create_time <= ").AppendParam(query.endTime))
                .GeneratePageObjectAsync(query, "update_time desc");
            }
        }
        public async Task<List<MZ_Card_Case>> SelectListInIds(List<long> ids)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_card_case where Id in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<MZ_Card_Case>>();
                return docmd.ToList();
            }
        }

        public async Task<int> Insert(MZ_Card_Case data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<int> Update(MZ_Card_Case data)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Update(data)
                         .DoAsync<DoExecSql>()).RowCount;
            }
        }
        public async Task<int> Delete(long id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Delete<MZ_Card_Case>("Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
    }
}
