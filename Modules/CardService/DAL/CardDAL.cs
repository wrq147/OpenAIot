using CardService.Model;
using Common;
using Common.Share;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardService.DAL
{
    public class CardDAL : BaseDbSupport
    {
        public async Task<MZ_Card> SelectById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_card where del_flag='0' and Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Card>>();
                return docmd.ToFirst();
            }
        }
        public async Task<MZ_Card> SelectDetailById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select c.*,uo.dept_id as DeptId from mz_card_v c left join mz_user_org uo on c.UserId=uo.UserId and c.OrgId=uo.OrgId where c.del_flag='0' and c.Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Card>>();
                return docmd.ToFirst();
            }
        }
        public async Task<List<MZ_Card>> SelectListInIds(List<long> ids)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_card_v where Id in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<MZ_Card>>();
                return docmd.ToList();
            }
        }

        public async Task<PageObject<MZ_Card>> SelectList(In_Card_List query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<MZ_Card>().Append("select * from mz_card_v where del_flag = '0'")
             .Then(query.UserId != null, sql =>
             {
                 sql.Append(" AND UserId=").AppendParam(query.UserId);
             })
             .Then(query.OrgId != null, sql =>
             {
                 sql.Append(" AND OrgId=").AppendParam(query.OrgId);
             })
             .Then(query.beginTime != null, sql =>
             {
                 sql.Append(" and create_time >= ").AppendParam(query.beginTime);
             })
             .Then(query.endTime != null, sql =>
             {
                 sql.Append(" and create_time <= ").AppendParam(query.endTime);
             })
             .Append(" order by update_time desc");
                return await tsql.GeneratePageObjectAsync(query);
            }

        }

        public async Task<int> Add(MZ_Card data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<int> Update(MZ_Card data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        public async Task<int> Delete(long id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update mz_card set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        public async Task<int> ClearOrg(long uid, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update mz_card set OrgId=0 where UserId=").AppendParam(uid).Append(" and OrgId=").AppendParam(orgId).DoAsync<DoExecSql>()).RowCount;
            }
        }
    }
}
